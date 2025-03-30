## Настроить CI-процесс для Вашей распределённой системы.

1. Проверка линтером
   Джоба в github для проверки кода линтером (и статическим анализатором)

```
      - name: Format
        run: dotnet format src/backend/SocialNetwork --verify-no-changes --verbosity diagnostic
```

[Сам линтер](https://github.com/Mekys/ddss_develop_course/blob/Lub_3/src/backend/SocialNetwork/.editorconfig)

2. Проверка статическим анализатором

Функионал заложен в коде выше

3. Автоматическое тестирование
   Джоба из github actions

```
      - name: Restore dependencies
        run: dotnet restore src/backend/SocialNetwork
      - name: Build
        run: dotnet build src/backend/SocialNetwork --no-restore
      - name: Test
        run: dotnet test src/backend/SocialNetwork --no-build -p:CollectCoverage=true -p:CoverletOutput=TestResults/ -p:CoverletOutputFormat=opencover
```

На выход выдается покрытие кода

```
+-------------+--------+--------+--------+
| Module      | Line   | Branch | Method |
+-------------+--------+--------+--------+
| Domain      | 93.06% | 85.48% | 92.75% |
+-------------+--------+--------+--------+
| Application | 0%     | 0%     | 0%     |
+-------------+--------+--------+--------+

+---------+--------+--------+--------+
|         | Line   | Branch | Method |
+---------+--------+--------+--------+
| Total   | 60.8%  | 51.96% | 66.66% |
+---------+--------+--------+--------+
| Average | 46.53% | 42.74% | 46.37% |
+---------+--------+--------+--------+
```

## Применить одну из техник для масштабирования компонентов Вашей распределённой системы

Была выбрана первая техника (Балансировщик нагрузки L4 или L7 + репликация компонента. Необходимо выбрать алгоритм балансировки. Реплицировать компонент на уровне контейнеров (через оркестратор, например, docker compose))

В качестве балансира используется `NGINX`

В качестве алгоритма был выбран алогоритм `Least Time`, тк он хорошо подходит в случае высокой нагрузки одной реплиакций. Однако он доступен лишь с подпиской `nginx Plus`, поэтому реаллизуется стандартный `Round Robin`.

В докере поднял 3 инстанса приложения и настроил nginx на балансировку между ними. Также каждому из инстансов передал его название через переменные окружения, это понадобится нам при настройке логирования.

Измененый [docker-compose.yml](https://github.com/Mekys/ddss_develop_course/blob/Lub_3/.docker-compose.yml)

[nginx.config](https://github.com/Mekys/ddss_develop_course/blob/Lub_3/nginx.conf)

## Логирование через GrayLog

1.  Добавил все зависимости `GrayLog` в docker-compose
2.  В приложение настроил логирование, чтобы оно также отправлялось в `GrayLog`. Также добавил Enricher, который вставлял в логи название сервиса.

```cs
builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(builder.Configuration);
    loggerConfiguration.Enrich.WithProperty("ServiceName", Environment.GetEnvironmentVariable("SERVICE_NAME") ?? "None");
});
```

3.  Все запустил

Смотрим, что логи доходят

![Логи приложений](img/image.png)

Также посмотрим на статистику поля "ServiceName"

![Статистика логов](img/log_stat.png)

Как видим - балансиры распределяют нагрузку и сервисы отправляют все логи в GrayLogz
