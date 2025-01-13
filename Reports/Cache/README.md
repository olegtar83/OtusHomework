# Caching
1. Для реализаци храненнии данных в кеше был выбран следующий инструментарий:

   * Kafka - для отложенной обработки.
   * Redis - для храненния кэша.
   * MessagePack -для оптимизации сериализации и объема данных .
2. Открываем `http://localhost:3000/register`, создаем нового пользователя `Michael Jordan` с паролем `string`.
3. Добавляем друзей

   ![friends](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Cache/add-friends.png)
4. Проверяем добавление друзей
   
   ![friends](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Cache/get-friends.png)
5. Создаем пост.

   ![post](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Cache/create-post.png)
6. Смотрим в redis cli на наличие ключей подписаных друзей в формате feed-{userId}.

   ![redis1](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Cache/redis-keys.png)     
7. Убираем юзера из друзей `http://localhost:7888/api/Friends/Delete/ff55dd6a-487e-4c9b-a042-2571193e2b37`.
8. Cмотрим на ключи в редисе, остается один ключ.
   ![redis2](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Cache/delete-friend-redis.png)     
9. Добавляем еще один пост, и проверяем фид
   ![feed](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Cache/get-posts.png)
 
10. Фид будет браться из кеша поcкольку ключ `feed-67226925-f615-48c2-8fe6-a0f47e9f385f` существует в 
    редисе.
11. Инвалидация кеша также при случае обновления друзей или модификаций, добавлений и удалений постов работает корректно 
    после события в брокере сообщений.
14. Также можно посмореть на события по инвалидации, добавленые в топике `update-feed-posts` :
    
    ![kafka](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Cache/kafka-cache.png)
