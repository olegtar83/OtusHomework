# Sharding

Выбранная стратегия - для того чтобы избежать эффекта Лейди Гага, при котором один пользователь имеет очень большое количество чатов и тем самым перегружает выбранные шарды, принято решение использовать шардинированный ключ который создается на основе сочетания двух участников чата. Таким образом, если человек становится популярным и получает множество сообщений от разных собеседников, чаты будут распределяться равномерно по шардовым кластерам.

1) Создается таблица после запуска приложения
```
CREATE TABLE IF NOT EXISTS messages (
       id int generated always as identity,
       text TEXT NOT NULL,
       "from" CHARACTER VARYING NOT NULL,
       "to" CHARACTER VARYING NOT NULL,
       "shardId" int NOT NULL,
       constraint pk primary key(id,"shardId") );
```
2) Ключ шардирования 
```
 SELECT create_distributed_table('messages', 'shardId');
```
3) Переходим на `http://localhost:7888/swagger/index.html`, зарегистрировать нового юзера, забрать jwt token, потом перейти
   в новый сервис сообщений `http://localhost:7887/swagger/index.html`, авторизировать с токеном и начать слать сообщения c
   рандомным юзером `5e0db4db-c206-4edc-84c7-5b159030c767`.

4) Сделать запрос на сообщения по тому же юзеру, видим что все сообщения имеют одинаковый shardId, значит находятся на одном шарде -
```
select * from public.messages where "to" = '5e0db4db-c206-4edc-84c7-5b159030c767';
```
```

 postgres=# select * from public.messages;
 id |  text  |                 from                 |                  to                  | shardId
----+--------+--------------------------------------+--------------------------------------+---------
  1 | string | 6d9d6850-496c-49b0-9d31-c5d01fa0eeee | 5e0db4db-c206-4edc-84c7-5b159030c767 |      21
  2 | kjkj   | 6d9d6850-496c-49b0-9d31-c5d01fa0eeee | 5e0db4db-c206-4edc-84c7-5b159030c767 |      21
  3 | biubui | 6d9d6850-496c-49b0-9d31-c5d01fa0eeee | 5e0db4db-c206-4edc-84c7-5b159030c767 |      21
```
5) Теперь сделать тот же запрос с `explain analyze`, запрос отработал с одного шарда `messages_102026`
```
Custom Scan (Citus Adaptive)  (cost=0.00..0.00 rows=100000 width=104) (actual time=39.928..39.929 rows=3 loops=1)
   Task Count: 32
   Tuple data received from nodes: 256 bytes
   Tasks Shown: One of 32
   ->  Task
         Tuple data received from node: 0 bytes
         Node: host=localhost port=5432 dbname=postgres
         ->  Seq Scan on messages_102026 messages  (cost=0.00..17.62 rows=3 width=104) (actual time=0.005..0.005 rows=0 loops=1)
               Filter: (("to")::text = '5e0db4db-c206-4edc-84c7-5b159030c767'::text)
             Planning Time: 0.622 ms
             Execution Time: 0.034 ms
 Planning Time: 5.329 ms
 Execution Time: 40.060 ms
```
### Resharding

1) Добавим еще несколько шардов
```
set POSTGRES_PASSWORD=pass && docker-compose -p citus up --scale worker=5 -d
```
2) Проверяем видимость
```
SELECT master_get_active_worker_nodes();
SELECT nodename, count(*) FROM citus_shards GROUP BY nodename;
```
![first](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Sharding/first.png)

3) Переходим в psql и меняем wal_level
```
alter system set wal_level = logical;
SELECT run_command_on_workers('alter system set wal_level = logical');
```
![wal](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Sharding/wal.png)

4) Рестартим docker
```
set POSTGRES_PASSWORD=pass && docker-compose restart
```
5) Смотрим уровень реплики
```
show wal_level;
```
![wal](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Sharding/logical.png)

6) Запускаем перебалансировку
```
SELECT * FROM citus_rebalance_status();
```
![rebalance](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Sharding/rebalance.png)

7) Смотрим распределение
```
SELECT nodename, count(*) FROM citus_shards GROUP BY nodename;
```
![done](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Sharding/done.png)
