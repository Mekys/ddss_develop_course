### Что получилось сделать

1. Подключить async-runtime к Вашей DSS.
   Пример кода с использованием async

```cs
     public async Task<Result> AddLike(Guid postId)
     {
         var post = await _postRepository.GetById(postId);
         if (post is null)
         {
             return Result.Fail("Not found");
         }

         var like = Like.Create(Guid.NewGuid(), DateTime.UtcNow);
         if (like.IsFailed)
         {
             return Result.Fail(like.Errors);
         }

         post.AddLike(like.Value);
         await _postRepository.Update(post);

         return Result.Ok();
     }
```

2. Подключить выбранную в П0 СУБД к Вашей DSS (InMemoryUserRepository -> MySQLUserRepository).
   Подключена PostgreSQL. Работа ведется через EF.Core

3. Явно выделить слои Вашей DSS и отразить их в коде:

Структура проекта

```
SocialNetwork
    ├───API
    │   ├───Controllers
    ├───Application
    │   ├───Dto
    │   ├───Repositories // Интерфейсы репозиториев
    │   └───Services // Интерфейсы сервисов и их имплиментация
    │       └───Interfaces
    ├───Domain
    │   ├───Global // Объекты, которые шарятся между Aggregate
    │   │   ├───Errors
    │   │   └───ValueObjects
    │   ├───Post
    │   │   ├───Entities
    │   │   ├───Errors
    │   │   ├───Events
    │   │   └───ValueObjects
    │   └───Profile
    │       ├───Entities
    │       ├───Errors
    │       ├───Events
    │       └───ValueObjects
    ├───Infrastructure
    └───UnitTests
```

4. Обновить или написать необходимые тесты.
   Тесты находятся в папке Tests

5. Сформировать первичные Dockerfile и docker-compose Вашей DSS и её инфраструктуры (на основе шаблона).

[Docker](/../Dockerfile)

[docker-compose.yml](/../docker-compose.yml)
