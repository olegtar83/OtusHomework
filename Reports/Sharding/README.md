# Sharding

Выбранная стратегия - для того чтобы избежать эффекта Лейди Гага, при котором один пользователь имеет очень большое количество чатов и тем самым перегружает выбранные шарды, принято решение использовать шардинированный ключ который, создаваемый на отоснове сочетания двух участников чата. Таким образом , если человек становится популярным и получает ножество сообщений, чаты будут распределяться равномерно по шардовым кластерам.

1) Создается таблица при запуске приложения
```
CREATE TABLE IF NOT EXISTS messages (
    id CHARACTER VARYING NOT NULL,
    text TEXT NOT NULL,
    "from" CHARACTER VARYING NOT NULL,
    "to" CHARACTER VARYING NOT NULL,
	  "shardId" int NOT NULL
);
```
2) Ключ шардирования для каждого чата будет одинаковый и будет заранее подготовленной хеш функцией в коде и будет иметь 32 значения, как количество шардов в нодах -
```
 SELECT create_distributed_table('messages', 'shardId');
```
3) Переходим на `http://localhost:7888/swagger/index.html`, зарегистрировать нового юзера, забрать jwt token, потом перейти
   в новый сервис сообщений `http://localhost:7888/swagger/index.html`, авторизировать с токеном и начать слать сообщения c
   рандомным юзером `62189e29-d6cd-4296-9dc2-c345512a3204`.

4) Сделать запрос на сообщения по тому же юзеру -
```
select * from public.messages where "from" = '20289949-1461-43cf-bb4d-993932d21302';`
```
```
                  id                  |   text   |                 from                 |                  to                  | shardId
--------------------------------------+----------+--------------------------------------+--------------------------------------+---------
 14afffdb-84a1-4d50-8b5b-b1e072ddab17 | sdfsdfsd | 20289949-1461-43cf-bb4d-993932d21302 | 62189e29-d6cd-4296-9dc2-c345512a3204 |      29
 14afffdb-84a1-4d50-8b5b-b1e072ddab17 | sdfsdfsd | 20289949-1461-43cf-bb4d-993932d21302 | 62189e29-d6cd-4296-9dc2-c345512a3204 |      29
 14afffdb-84a1-4d50-8b5b-b1e072ddab17 | asdasdsa | 20289949-1461-43cf-bb4d-993932d21302 | 62189e29-d6cd-4296-9dc2-c345512a3204 |      29
 14afffdb-84a1-4d50-8b5b-b1e072ddab17 | asdasdas | 20289949-1461-43cf-bb4d-993932d21302 | 62189e29-d6cd-4296-9dc2-c345512a3204 |      29
```
5) Теперь сделать тот же запрос с `explain analyze`
```
Custom Scan (Citus Adaptive)  (cost=0.00..0.00 rows=100000 width=132) (actual time=14.492..14.493 rows=4 loops=1)
   Task Count: 32
   Tuple data received from nodes: 480 bytes
   Tasks Shown: One of 32
   ->  Task
         Tuple data received from node: 0 bytes
         Node: host=citus-worker-2 port=5432 dbname=postgres
         ->  Seq Scan on messages_102009 messages  (cost=0.00..16.38 rows=3 width=132) (actual time=0.005..0.005 rows=0 loops=1)
               Filter: (("from")::text = '20289949-1461-43cf-bb4d-993932d21302'::text)
             Planning Time: 0.041 ms
             Execution Time: 0.030 ms
 Planning Time: 0.837 ms
 Execution Time: 14.513 ms
```
5) Весь запрос прошол в рамках одного шарда `messages_102009`.
### Resharding
