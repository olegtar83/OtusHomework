# LegendarySocialNetwork. Инструкция по запуску

Запрос для создания таблиц лежит в папке `Postgres`.
Коллекция вызовов для Postman лежит в папке `Postman`.

0. Все состояние базы данных хранится в папке `volumes`. Для того, чтобы запустить с нуля и заново создать базу данных нужно удалить папку `rm -rf volumes`.
1. Запускаем базу данных и API командой `docker-compose up -d`.
2. Ждем когда Postgres запустится, перезагрузится, создадутся таблицы.
3. Открываем `http://localhost:7888/swagger`, создаем нового пользователя `qwerty` с паролем `qwerty`. Запрос `Register`. Получаем id пользователя. (В тестовой базе пользователь уже создан)
4. Логинимся с полученным id. Запрос `Login`. Получаем токен авторизации. Копируем его.
5. Получаем информацию о пользователе. Запрос `Get user by id`. На вкладке `Authorization` выбираем `Type: Bearer Token`, в поле `Token` вставляем скопированный токен с шага 6. В строке с адресом запроса после `http://localhost:7888/user/get/` вставляем нужный id
7. Можно перейти по адресу `http://localhost:3000`. Зарегистрироваться, разлогиниться, залогиниться, посмотреть профиль.
8. Репорты связанные с тестировкой нагруженности находятся в папке `Reports`.
9. Запрос без индекса:
    ``` Gather  (cost=1000.00..22521.61 rows=766 width=90) (actual time=904.236..2454.411 rows=700 loops=1)
       Workers Planned: 2
       Workers Launched: 2
       ->  Parallel Seq Scan on "user"  (cost=0.00..21445.01 rows=319 width=90) (actual time=881.520..2425.928 rows=233 loops=3)
             Filter: (((first_name)::text ~~ 'Ива%'::text) AND ((second_name)::text ~~ 'Т%'::text))
             Rows Removed by Filter: 333100
     Planning Time: 36.957 ms
     Execution Time: 2454.595 ms```
