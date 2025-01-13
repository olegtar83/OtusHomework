# Caching
1. Для реализаци храненнии данных в кеше был выбран следующий инструментарий:

   * Kafka - для отложенной обработки.
   * Redis - для храненния кэша.
   * MessagePack -для оптимизации сериализации и объема данных .
3. Открываем `http://localhost:7888/swagger`, создаем нового пользователя `Michael Jordan` с паролем `string`. Запрос `Register`. Получаем id пользователя. (В тестовой базе пользователь уже создан)
4. Логинимся с полученным id. Запрос `Login`. Получаем токен авторизации. Копируем его.
5. Получаем информацию о пользователе. Запрос `Get user by id`. На вкладке `Authorization` выбираем `Type: Bearer Token`, в поле `Token` вставляем скопированный токен с предыдущего шага.
6. В сваггере отправляем запрос на добавление друга `http://localhost:7888/api/Friends/Set/67226925-f615-48c2-8fe6-a0f47e9f385f`, добавляем еще одного друга из дампа `http://localhost:7888/api/Friends/Set/ff55dd6a-487e-4c9b-a042-2571193e2b37`.
7. Проверяем добавление друзей
   
   ![friends](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Cache/get-friends.png)
8. Создаем пост.

   ![post](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Cache/create-post.png)
9. Смотрим в redis cli на наличие ключей подписаных друзей в формате feed-{userId}.

   ![redis1](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Cache/redis-keys.png)     
11. Убираем юзера из друзей `http://localhost:7888/api/Friends/Delete/ff55dd6a-487e-4c9b-a042-2571193e2b37`.
12. Cмотрим на ключи в редисе, остается один ключ.
   ![redis2](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Cache/delete-friend-redis.png)     
13. Добавляем еще один пост, и проверяем фид
   ![feed](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Cache/get-posts.png)
 
14. Фид будет браться из кеша поcкольку ключ `feed-67226925-f615-48c2-8fe6-a0f47e9f385f` существует в 
    редисе.
13. Инвалидация кеша также при случае обновления друзей или модификаций, добавлений и удалений постов работает корректно 
    после события в брокере сообщений.
14. Также можно посмореть на события по инвалидации добавленые в топик `update-feed-posts` :
    
    ![kafka](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Cache/kafka-cache.png)
