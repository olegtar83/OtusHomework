# Полусинхронная репликация
1) Создаем мастер со следующеми настройками
```listen_addresses = 'localhost,172.21.0.2'
max_connections = 100
ssl = off
shared_buffers = 1GB
dynamic_shared_memory_type = posix
max_wal_size = 1GB
min_wal_size = 80MB

wal_level = replica
synchronous_commit = on
synchronous_standby_names = 'FIRST 1 (slave1-db, slave2-db)'

max_wal_senders = 8
```
2) Создаем первую реплику
```
listen_addresses = 'localhost,172.21.0.3'
max_connections = 500
ssl = off
shared_buffers = 1GB
dynamic_shared_memory_type = posix
max_wal_size = 1GB
min_wal_size = 80MB

wal_level = replica
max_wal_senders = 8
primary_conninfo = 'host=master-db port=5432 user=replicator password=pass application_name=slave1-db'
```
3) Создаем вторую реплику
```
listen_addresses = 'localhost,172.21.0.4'
max_connections = 500
ssl = off
shared_buffers = 1GB
dynamic_shared_memory_type = posix
max_wal_size = 1GB
min_wal_size = 80MB

wal_level = replica
max_wal_senders = 8
primary_conninfo = 'host=master-db port=5432 user=replicator password=pass application_name=slave2-db'
```
4) Добавляем юзер для репликации в дамп на мастере
```
CREATE ROLE replicator WITH REPLICATION PASSWORD 'pass' LOGIN;
```   
5) Во всех файлах `pga_hba.conf` добавляем
```
host    replication     replicator      172.21.0.0/16           trust
```
6) Бекапим мастер (подождать пока сервера прогрузиться)
```
docker exec -it slave1-db /bin/bash
pg_basebackup -P -R -X stream -c fast -h 172.21.0.2 -U replicator -D /backup

docker exec -it slave2-db /bin/bash
pg_basebackup -P -R -X stream -c fast -h 172.21.0.2 -U replicator -D /backup
```
7) Останавливаем все реплики и переносим бекап 
```
docker compose stop slave1-db
docker compose stop slave2-db

rmdir /s postgres_data_slave1
rmdir /s postgres_data_slave2

xcopy .\postgres_backup_slave1\* .\postgres_data_slave1\ /E /I /Y
xcopy .\postgres_backup_slave2\* .\postgres_data_slave1\ /E /I /Y

docker compose start slave1-db
docker compose start slave2-db
```
8) Смотри в логах реплики появидась следуещея строчка
```
started streaming WAL from primary at 0/10000000 on timeline 1
```
9) Делаем соответсвующие изменения в коде и делаем нагрузочное тестирование на ' /user/search', результаты:
![master](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Replica/master.png)
![slave1](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Replica/slave1.png)
![slave2](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Replica/slave2.png)
10) Видим что на репликах вырос запрос на CPU и Memory, в то время как на мастере он не меняется.
11) Далее делаем нагрузочное тестирование на мастер и убиваем его
```
docker exec -it master-db psql -U dbuser -d legendarydb
CREATE TABLE test_table (id INT, name TEXT);
INSERT INTO test_table (id, name) SELECT generate_series(1, 1000), 'test';

docker-compose stop master-db
```
12) Промотим реплику до мастера
```
docker exec -it slave1-db psql -U dbuser -d legendarydb

select * from pg_promote();
```
 Меняем конфигурацию в файле `slave1_postgresql.conf`
```
synchronous_commit = on
synchronous_standby_names = 'FIRST 1 (master-db, slave2-db)'
```

 Перезапускаем `select * from pg_reload_conf();`

13) При повторной проверке выяснилось что все транзакции сохранились без потерь.
