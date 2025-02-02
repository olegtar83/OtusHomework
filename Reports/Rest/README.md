# Rest Api
1) Добавляем сервис чатов, который доступен по ссылке `http://localhost:7887/swagger/index.html`
2) Сервис использует версионирование в зависимости от заголовка `X-Api-Version` в запросе :
   ```
     curl -X 'GET' \
    'http://localhost:7887/api/Dialog/69203ea9-d5d6-41f2-a85d-8c6480f04ab8/list' \
    -H 'accept: */*' \
    -H 'X-Api-Version: 1' \
    -H 'Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6InN0cml4Y3p4Y25nIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZSI6InN0cmluZyBzdHJpbmciLCJleHAiOjE3Mzg0NTU3MjIsImlzcyI6ImxlZ2VuZGFyeS5zb2NpYWwubmV0d29yayIsImF1ZCI6ImxlZ2VuZGFyeS5zb2NpYWwubmV0d29yayJ9.PmPrR8EXGqv4vkXrO6PIDh7z5KyNXNxw4oFXpH3no8Q'
   ```
3) В основном сервисе `http://localhost:7888/swagger/index.html` так же создан api шлюз для взаимодействия с сервисом чатов посредством http протокола. Используется сквозное логирование заголовка `X-RequestId` для трассировки запросов.

4) Запрос на стороне главного (основного) сервера
   ![request](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Rest/x-requestid-sent.png)

5) Заголовок на стороне сервиса чатов   
   ![response](https://github.com/olegtar83/OtusHomework/blob/master/Reports/Rest/x-requestid-received.png)
