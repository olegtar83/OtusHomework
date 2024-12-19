# Caching
1. Для реализаци храненнии данных в кеше был выбран следующий инструментарий:

   * Kafka - для отложенной обработки.
   * Redis - для храненния кэша.
   * MessagePack -для оптимизации сериализации и объема данных .
3. Открываем `http://localhost:7888/swagger`, создаем нового пользователя `qwerty` с паролем `qwerty`. Запрос `Register`. Получаем id пользователя. (В тестовой базе пользователь уже создан)
4. Логинимся с полученным id. Запрос `Login`. Получаем токен авторизации. Копируем его.
5. Получаем информацию о пользователе. Запрос `Get user by id`. На вкладке `Authorization` выбираем `Type: Bearer Token`, в поле `Token` вставляем скопированный токен с предыдущего шага.
6. В сваггере отправляем запрос на добавление друга `http://localhost:7888/api/Friends/Set/67226925-f615-48c2-8fe6-a0f47e9f385f`, добавляем еще одного друга из дампа `http://localhost:7888/api/Friends/Set/ff55dd6a-487e-4c9b-a042-2571193e2b37`.
7. Проверяем добавление друзей `http://localhost:7888/api/Friends/Get`
   ```
   [
    {
      "id": "67226925-f615-48c2-8fe6-a0f47e9f385f",
      "name": "Бирюков Ярослав",
      "city": "Белорецк"
    },
      {
        "id": "ff55dd6a-487e-4c9b-a042-2571193e2b37",
        "name": "Бирюков Никита",
        "city": "Мелеуз"
      }
    ]
   ```
  8. Создаем пост `http://localhost:7888/api/Post/Create`.
  9. Смотрим в redis cli на наличие ключей с постом у друзей в формате feed-{userId}.
  ![redis1](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Cache/redis-keys.png)     
  10. Убираем юзера из друзей `http://localhost:7888/api/Friends/Delete/ff55dd6a-487e-4c9b-a042-2571193e2b37`.
  11. Cмотрим на ключи в редисе, остается один ключ.
  ![redis2](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Cache/delete-friand-redis.png)     
  12. Добавляем еще один пост, и проверяем фид у оставшегося друга `http://localhost:7888/api/Post/Feed/67226925-f615-48c2- 
  8fe6-a0f47e9f385f`, фид будет браться из кеша поcкольку ключ `feed-67226925-f615-48c2-8fe6-a0f47e9f385f` существует в 
  редисе, получаем коллекцию ключей из редиса.
  ```
  [
    {
      "id": "e38334c7-85fc-4873-86a8-a0fe0f5e473e",
      "text": "string",
      "userId": "be732198-0f1a-43c8-b2dc-035fac7d6683",
      "created": "2024-12-19T09:27:21.7906913Z"
    },
    {
      "id": "76c3b784-d220-40f6-898b-59707392752a",
      "text": "ориоио",
      "userId": "be732198-0f1a-43c8-b2dc-035fac7d6683",
      "created": "2024-12-19T10:20:18.9522163Z"
    }
  ```
  13. Обновляем один из постов `http://localhost:7888/api/Post/Update`.
  14. Повторно смотри фид друга `http://localhost:7888/api/Post/Feed/67226925-f615-48c2-8fe6-a0f47e9f385f`.
  ```[
     {
      "id": "e38334c7-85fc-4873-86a8-a0fe0f5e473e",
      "text": "string",
      "userId": "be732198-0f1a-43c8-b2dc-035fac7d6683",
     "created": "2024-12-19T09:27:21.7906913Z"
    },
    {
      "id": "76c3b784-d220-40f6-898b-59707392752a",
      "text": "this post was updated",
      "userId": "be732198-0f1a-43c8-b2dc-035fac7d6683",
      "created": "2024-12-19T10:20:18.9522163Z"
    }
  ]
  ```
  15. Удаляем один из постов `http://localhost:7888/api/Post/Delete/76c3b784-d220-40f6-898b-59707392752a`, смотрим фид друга
  `http://localhost:7888/api/Post/Feed/67226925-f615-48c2-8fe6-a0f47e9f385f`, остался один пост.
  ```
  [
    {
      "id": "e38334c7-85fc-4873-86a8-a0fe0f5e473e",
      "text": "string",
      "userId": "be732198-0f1a-43c8-b2dc-035fac7d6683",
      "created": "2024-12-19T09:27:21.7906913Z"
    }
  ]
  ```
  16. Инвалидация кеша  в случае обновление друзей, модификации, добавлении и удалении постов работает корректно после 
  события в брокере сообщений.
  17. Также можно посмореть на происходящее в кафке ui:
  ```
 http://localhost:8080/ui/clusters/local/all-topics/update-feed-posts/messages?keySerde=String&valueSerde=String&limit=100
 ```
