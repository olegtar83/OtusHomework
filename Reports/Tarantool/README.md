# Tarantool
Для реализации задачи был использован следующий инструментарий:

* Postgres
* Tarantool
* Lua
* progaudi.tarantool

Был разработан следующий алгоритм действий: данные записываются в PostgreSQL; сервиc репликатор, написанный на Lua, сравнивает, если id в Postgres больше максимального id в Tarantool то он оносуществляет копирование записи в Tarantool.

![diagram](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Tarantool/tarantool-diagram.png)

Конфигурация Tarantool прописана в файле lua, так же были созданы два поисковых индекса и прописаны там же. Однако при вызове дополнительной функции из этого файла lua для поиска по индексам, результаты не находились, что привело к решению обращаться к индексам поиска через драйвер.

1) Изначально заполняем Postgres на 899979 строчек рандомных данных, все они скопируются в Tarantool.
![rows](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Tarantool/rows_in_postgres.png)

2) Для проверки добавляем одну запись в Postgres.
![add-postgres](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Tarantool/insert-postgres.png)

3) Смотрим ее в админке тарантула, репликация произошла успешно
![replica](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Tarantool/tarantool-replicator.png)

4) Теперь делаем замеры на 1000 запросов `http://localhost:7887/api/Dialog/d1743302-2943-418b-99fd-cac5cb5ac224/List` по поиску диалога в Postgres.
![postgres](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Tarantool/postgres-dialog.png)
Запрос получился очень тяжелым поэтому сервер не выдержал нагрузку даже при индексации запросов.

5) Делаем замер на 1000 запросов с участием Tarantool
![tarantool](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Tarantool/tarantool-dialog.png)

6) При Tarantool время уменьшилось в два раза по сравнению с Postgres.
