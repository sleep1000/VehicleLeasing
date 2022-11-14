# Приложение "Аренда автомобилей"

## Используемые технологии:
- ASP.Net Core 6.0
- ASP.Net Core Identity
- ASP.Net SignalR
- PostgreSQL
- EntityFramework Core 6.0
- React 18
- Redux
- Redux-Thunk
- Reactstrap

## Настройка:
Настройка параметров производится в файле appsettings.json, а также через переменные окружения. При настройке параметров БД переменные окружения имеют более высокий приоритет, чем строка подключения в файле конфигурации.

### 1. Настройка appsettings.json
Параметры приложения задаются в секции AppConfig файла конфигурации. Доступны следующие параметры:
- LeasesLimitPerDriver - Максимальное количество автомобилей в аренде у одного водителя. Значение по умолчанию: 2
- ImagePathPrefix - путь к директории с изображениями на карточках относительно корневого пути. Значение по умолчанию: images

Пример секции
```
  },
  "AppConfig": {
    "LeasesLimitPerDriver":  2,
    "ImagePathPrefix": "images"
  },
```

В секции ConnectionStrings значение DefaultConnection задает строку подключения к БД.
Пример:
```
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=5432;Database=postgres;User Id=postgres;Password=pass;"
  },
```

### 2. Настройка переменных окружения
Для настройки доступны следующие переменные окружения:
- DB_HOST - адрес БД
- DB_PORT - порт БД
- DB_DATABASE - имя БД (по умолчанию: "postgres")
- DB_USER - имя пользователя БД
- DB_PASSWORD - пароль пользователя БД
- APPLY_MIGRATIONS - применять ли миграции БД при запуске приложения. Валидные значения: "1", "TRUE"

## Запуск
1. Приложение можно запустить в Visual Studio в отладочной конфигурации. При этом автоматически запустится браузер с главной страницей приложения. Если браузер не запустился, можно открыть страницу https://localhost:44422 При этом необходимо настроить сервер БД и указать его в конфигурации. Также в переменной окружения PATH должен быть указан путь к nodejs и npm.
2. Продакшен-сборка реализована в Docker. Сборка и запуск производятся следующими командами из каталога проекта:
```
docker build -t localhost/vehicleleasing .
```
```
docker run --name=vehicleleasing -d -e APPLY_MIGRATIONS=1 -e DB_HOST=postgres_host -e DB_PORT=5432 -e DB_USER=postgres -e DB_PASSWORD=changeme -p 8080:80 -p 8443:443 --restart unless-stopped localhost/vehicleleasing
```
Альтернативный способ запуска через docker-compose:
```
docker-compose up -d
```
из каталога проекта. При этом в compose файле дополнительно определен сервис БД, поэтому настраивать БД самоcтоятельно не нужно. Переменные среды для настроек БД определены в файле .env . В этом случае страница приложения доступна по адресу: http://localhost:8080 (поддержка HTTPS в контейнере не реализована).
