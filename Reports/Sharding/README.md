# Sharding

Выбранная стратегия - ключ шардирования будет первичным ключом (id) в таблице messages. Таким образом, чаты разных людей будут  равномерно расположены по шард-серверам; это решение не идеально, однако решения о том, как эффективно шардировать по заранее готовому ключу из хеш функции и целиком хранить весь чат на одном шарде я не нашёл. Потому что при решардировании Citus потребовал наличия первичного ключа или уникального ограничения (UNIQUE), причём при стратегии shardId от 0 и до 31 который был один раз привязавался к чату, shardId не являлся бы уникальным, что делает невозможным последующее решардирование. 

1) Создается таблицу после запуска приложения
```
CREATE TABLE IF NOT EXISTS messages (
       id UUID PRIMARY KEY,
       text TEXT NOT NULL,
       "from" CHARACTER VARYING NOT NULL,
       "to" CHARACTER VARYING NOT NULL
                      );
```
2) Ключ шардирования 
```
 SELECT create_distributed_table('messages', 'id');
```
3) Переходим на `http://localhost:7888/swagger/index.html`, зарегистрировать нового юзера, забрать jwt token, потом перейти
   в новый сервис сообщений `http://localhost:7888/swagger/index.html`, авторизировать с токеном и начать слать сообщения c
   рандомным юзером `62189e29-d6cd-4296-9dc2-c345512a3204`.

4) Сделать запрос на сообщения по тому же юзеру -
```
select * from public.messages where "from" = '92ed438b-a4ba-4e78-8e49-8437e7c94545';`
```
```
 --------------------------------------+----------+--------------------------------------+--------------------------------------
 58209945-4b02-40df-aafb-8c139ad4be6b | sdfsdfd  | 92ed438b-a4ba-4e78-8e49-8437e7c94545 | a5d837c0-4a30-462c-bd14-1541bc9bb536
 0e2327f7-ffb9-481a-9f71-8fbbad9d52d1 | sdfsdfsd | 92ed438b-a4ba-4e78-8e49-8437e7c94545 | a5d837c0-4a30-462c-bd14-1541bc9bb536
 9d01b412-cc8f-44a7-b48d-2924a0e8713a | string   | 92ed438b-a4ba-4e78-8e49-8437e7c94545 | a5d837c0-4a30-462c-bd14-1541bc9bb536
```
5) Теперь сделать тот же запрос с `explain analyze`
```
 Custom Scan (Citus Adaptive)  (cost=0.00..0.00 rows=100000 width=112) (actual time=30.868..30.870 rows=3 loops=1)
   Task Count: 32
   Tuple data received from nodes: 285 bytes
   Tasks Shown: One of 32
   ->  Task
         Tuple data received from node: 0 bytes
         Node: host=localhost port=5432 dbname=postgres
         ->  Seq Scan on messages_102068 messages  (cost=0.00..17.25 rows=3 width=112) (actual time=0.024..0.024 rows=0 loops=1)
               Filter: (("from")::text = '92ed438b-a4ba-4e78-8e49-8437e7c94545'::text)
             Planning Time: 0.149 ms
             Execution Time: 0.031 ms
 Planning Time: 1.259 ms
 Execution Time: 30.896 ms
```
### Resharding

1)Добавим еще несколько шардов
```
set POSTGRES_PASSWORD=pass && docker-compose -p citus up --scale worker=5 -d
```
2) Проверяем видимость
```
SELECT master_get_active_worker_nodes();
SELECT nodename, count(*) FROM citus_shards GROUP BY nodename;
```
3) Переходим в psql и меняем wal_level
```
alter system set wal_level = logical;
SELECT run_command_on_workers('alter system set wal_level = logical');
```
4)Рестартим docker
```
set POSTGRES_PASSWORD=pass && docker-compose restart
```
5)Запускаем перебалансировку
```
SELECT * FROM citus_rebalance_status();
```
6) Смотрим распределение
```
SELECT nodename, count(*) FROM citus_shards GROUP BY nodename;

```
