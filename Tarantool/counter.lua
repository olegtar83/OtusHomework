box.cfg
{
    log_level = 5
}

if box.space.counter == nil then
    box.schema.space.create('counter')
    box.space.counter:format({
        {name = 'id', type = 'string'},
        {name = 'count', type = 'number'},
        {name = 'from', type = 'string'},
        {name = 'to', type = 'string'}
    })
    
    box.space.counter:create_index('primary', { type = 'tree', parts = {'id'} })
    box.space.counter:create_index('owner_index', { type = 'tree', parts = {'from'}, unique = false })
    box.space.counter:create_index('other_index', { type = 'tree', parts = {'to'}, unique = false })
else
    print("Space 'counter' already exists.")
end

function get_counter(owner, other)
    counter =  box.execute([[SELECT * FROM "counter" WHERE "from"=:owner AND "to"=:other;]],{{[':owner']=owner},{[':other']=other}})
        if counter.rows[1] ~= nil then
                return counter
        end
        return nil
end

function increment_counter(owner, other)
    local uuid = require('uuid')
    counter =  box.execute([[SELECT * FROM "counter" WHERE "from"=:owner AND "to"=:other;]],{{[':owner']=owner},{[':other']=other}})
    if counter.rows[1] ~= nil then
        box.execute([[UPDATE "counter" SET "count" = :count WHERE "from"=:owner AND "to"=:other;]],{{[':owner']=owner},{[':other']=other},{[':count']=counter.rows[1][2] + 1}})
    else
        box.execute(string.format("INSERT INTO \"counter\" VALUES ('%s',1,'%s','%s');", uuid.str(), owner, other))
    end
end

function decrement_counter(owner, other)
    counter = box.execute([[SELECT * FROM "counter" WHERE "from"=:owner AND "to"=:other;]],{{[':owner']=owner},{[':other']=other}})
    if counter.rows[1] ~= nil then
        box.execute([[UPDATE "counter" SET "count" = :count WHERE "from"=:owner AND "to"=:other;]],{{[':owner']=owner},{[':other']=other},{[':count']=counter.rows[1][2] - 1}})  
    end  
end
