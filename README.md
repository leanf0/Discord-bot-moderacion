# Discord Moderation Bot

Bot de moderación para Discord en C# con .NET 8. Prefijo `$`.

## Comandos

`$clear <n>` borra entre 1 y 100 mensajes. `$log` muestra los últimos 10 mensajes borrados. `$setlog` configura el canal actual como canal de logs.

## Instalación

Cloná el repo, copiá `appsettings.example.json` a `appsettings.json` y completá el token. Activá el **Message Content Intent** en el Discord Developer Portal y ejecutá `dotnet run`.

## Variables de entorno

Si lo hosteás en Railway usá estas variables en lugar del `appsettings.json`: `DISCORD_TOKEN`, `DISCORD_PREFIX` y `LOG_CHANNEL_ID`.

## Importante

Nunca subas `appsettings.json` a GitHub porque contiene tu token.
