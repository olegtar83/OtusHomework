# LegendarySocialNetwork. Инструкция по запуску

Запрос для создания таблиц лежит в папке `Postgres`.
Коллекция вызовов для Postman лежит в папке `Postman`.

0. Все состояние базы данных хранится в папке `volumes`. Для того, чтобы запустить с нуля и заново создать базу данных нужно удалить папку `rm -rf volumes`.
1. Запускаем базу данных и API командой `docker-compose up -d`.
2. Ждем когда Postgres запустится, перезагрузится, создадутся таблицы.
3. Открываем `http://localhost:3000/register`, создаем нового пользователя.
     
![register](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Prototype/register.png)

4.Выходим и логинимся с тем же id.

![login](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Prototype/login.png)

5. Для того что бы получить информацию о пользователе нужно перейти на страницу профиля.
   
![login](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Prototype/profile.png)
