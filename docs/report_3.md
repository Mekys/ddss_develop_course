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

Фунционал реализовать не получилось
