# Sharding

Выбранная стратегия - для того чтобы избежать эффекта Лейди Гага, при котором один пользователь имеет очень большое количество чатов и тем самым перегружает выбранные шарды, принято решение использовать шардинированный ключ который, создаваемый на отоснове сочетания двух участников чата. Таким образом , если человек становится популярным и получает ножество сообщений, чаты будут распределяться равномерно по шардовым кластерам.

1) Создается таблица при запуске приложения
```
CREATE TABLE IF NOT EXISTS messages (
       id UUID PRIMARY KEY,
       text TEXT NOT NULL,
       "from" CHARACTER VARYING NOT NULL,
       "to" CHARACTER VARYING NOT NULL
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
5) Весь запрос прошол в рамках одного шарда `messages_102009`.
### Resharding
