# Queues

Для решения задачи был выбран следующий инструментарий

* Redis Pub/Sub
* Signalr
* Kafka
  
Redis был выбран по нескольким причинам:
1) Redis используется в качестве БД для храннения кэша.
2) с Redis мы можем отдельно запускать событие для каждого пользователя, так как при Kafka не знаем заранее количество consumerов.
3) Если будем масштабировать кэш, то и очереди масштабируются вместе с ним.
4) Кроме того , если кэш упал, то нет смысла отправлять обновления. 

Был разработан следующий алгоритм

 ![diagram](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Queues/diagram.png)    

 
1) Для проверки нужно зайти на веб приложения с двух браузеров `http://localhost:3000/` и зарегистрировать 2 разных пользователя.
2) Найти пользователя в верхнем поисковом окне
   
 ![friend](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Queues/addfriend.png)    

3) Нажать на запрос добавления в друзья.

 ![added](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Queues/addesSuccess.png)    

4) и проверить что работает как в демо

 ![demo](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Queues/demo.gif)   

5) Так же можно увидеть процесс подписки\публикации в Redis.

 ![redis](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Queues/redis-monitor.png) 
