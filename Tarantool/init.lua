#!/usr/bin/env tarantool

-- Add Taranrocks pathes. https://github.com/rtsisyk/taranrocks/blob/master/README.md
local home = os.getenv("HOME")

local log = require('log')

pg = require('pg')
driver = require('pg.driver')

local conn_str = 'postgresql://'..os.getenv('PG_USER')..':'..os.getenv('PG_PASSWORD')
        ..'@'..os.getenv('PG_HOST')..':'..os.getenv('PG_PORT')
        ..'/'..os.getenv('PG_DATABASE')
log.info(conn_str)

box.cfg
{
    pid_file = nil,
    background = false,
    log_level = 5
}

local function init()
    box.schema.user.create('tarantool', {password = 'tarantool', if_not_exists = true})
    box.schema.user.grant('tarantool', 'read,write,execute', 'universe', nil, { if_not_exists = true })

    box.schema.space.create('messages', { if_not_exists = true, temporary = true })

    -- Primary key definition (assuming 'id' as primary key)
    box.space.messages:format({
        { name = 'id', type = 'number' },
        { name = 'from', type = 'string' },
        { name = 'to', type = 'string' },
        { name = 'text', type = 'string' }
    })

    -- Create an auto-increment primary key
    box.space.messages:create_index('primary', {
        type = 'tree',
        parts = { 'id' },
        unique = true,
        if_not_exists = true
    })

    box.space.messages:create_index('from_index', {
        type = 'tree',
        parts = { 'from' },
        if_not_exists = true,
        unique = false
    })

    box.space.messages:create_index('to_index', {
      type = 'tree',
      parts = { 'to' },
      if_not_exists = true,
      unique = false
  })
    log.info("Init completed")
end

function sync()
    --log.info('Starting replication...')
    local fiber = require('fiber')
    local status, pg_conn = driver.connect(conn_str)
    
    if status < 0 then
        log.info(pg_conn)
    else
        --log.info("Connected")
        local at_least_one_message = box.space.messages:select({}, { limit = 1 })
        local max_id = 0

        if #at_least_one_message > 0 then
            local status, max_message = pcall(function()
                return box.space.messages.index.primary:max()
            end)

            if not status then
                log.error("Error retrieving max id: %s", max_message)
            else
                max_id = max_message and max_message[1] or 0
                ---log.info("Max ID in Tarantool messages %s:", max_id)
            end
        else
            log.info("No records found, max ID defaulting to 0")
        end

        local query = string.format("SELECT id, \"from\", \"to\", text FROM public.messages WHERE id > %d ORDER BY id", max_id)
        --log.info(string.format("Postgres Query: %s", query))
        local status, result = pg_conn:execute(query)

        if status < 0 then
            log.info(result)
        else
            --log.info("Start processing %s", #result[1])

            local messages = result[1]
            local total_count = 0

            for _, message in ipairs(messages) do
                local id = message['id']
                local from = message['from']
                local to = message['to']
                local text = message['text']

                -- Create a Lua table for each message
                local message_entry = {
                    id,
                    from,
                    to,
                    text
                }

                local ok, err = pcall(function()
                    --log.info("Inserting: %s", id)
                    box.space.messages:insert(message_entry)
                    fiber.yield()
                end)
                
                if not ok then
                    --log.error("Error inserting: %s. %s", id, err)
                end

                total_count = total_count + 1
            end

            pg_conn:close()
            --log.info("Loaded: %s", total_count)
            return total_count -- Return the count variable appropriately
        end
    end
end

function select_messages(param)
    local results = {}
    local index_key = { param }
    local from_results, from_err = pcall(function()
        return box.space.messages.index.from_index:select(index_key, {iterator = box.index.EQ})
    end)
  
    if not from_results then
        log.error("Error querying from_index with param '%s': %s", param, from_err)
    else
        log.info("From Result %s", from_results)
        for _, tuple in ipairs(from_results) do
            table.insert(results, tuple)
        end
    end
  
    local to_results, to_err = pcall(function()
        return box.space.messages.index.to_index:select(index_key, {iterator = box.index.EQ})
    end)
  
    if not to_results then
        log.error("Error querying to_index with param '%s': %s", param, to_err)
    else
        for _, tuple in ipairs(to_results) do
            table.insert(results, tuple)
        end
    end
  
    return results
  end

box.once('init', init)
log.info("Script ended")
print("Script ended print")