# Load Balancing

## Nginx
1) Добавляем конфигурацию в файл `nginx.conf`, теперь сайт доступен по ссылке `http://localhost/swagger/index.html`
2) Статиска по запросам и подключениям:
   ```
    Active connections: 2 
    server accepts handled requests
    46 46 42 
    Reading: 0 Writing: 1 Waiting: 1 
   ```
3) Убиваем одну из реплик сервиса
   ```
    Active connections: 1 
    server accepts handled requests
    47 47 43 
    Reading: 0 Writing: 1 Waiting: 0 
   ```
4) Если убить одну из реплик `docker kill -s 9`, nginx релоцирует на другую.

## HaProxy
1) Настраиваем 2 реплики и 1 мастер на постгре
 ![psql-f](https://github.com/olegtar83/OtusHomework/blob/master/Reports/LoadBalancing/full.png)
3) Убиваем одну из реплик и шлем запросы.
 ![psql-s](https://github.com/olegtar83/OtusHomework/blob/master/Reports/LoadBalancing/semi.png)
5) Одна из реплик идентифицирована как мертвая в результате healthchek-а, поэтому все запросы релоцируются на живую.

