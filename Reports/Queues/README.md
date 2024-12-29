# Queues

Для решения задачи были выбранны следующий инструментарий

* Redis Pub/Sub
* Signalr
* Kafka
  
Redis был выбран по нескольким причинам, он уже используются в качестве бд для храннения кэша, с Redis мы можем отдельно запускать события для каждого юзера. Кроме того если кэш упал, то нет смысла отправлять обновления.

Был разработан следующий алгоритм

 ![diagram](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Queues/diagram.png)    

 
1) Для проверки нужно зайти на веб приложения с двух браузеров `http://localhost:3000/` и зарегистрировать 2 разных пользователя.
2) Найти пользователя в верхнем поисковом окне
   
 ![friend](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Queues/addfriend.png)    

3) Нажать на запрос добавления в друзья.

 ![added](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Queues/addesSuccess.png)    

4) и проверить что работает как в демо

 [Watch Demo Video](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Queues/demo.mp4)   
