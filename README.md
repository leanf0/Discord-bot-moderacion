# 🐱 Bot de Moderación para Discord

Bot de moderación para Discord desarrollado en C# con .NET 8. Prefijo: `$`

## Comandos

| Comando | Descripción |
|---|---|
| `$clear <n>` | Borra entre 1 y 100 mensajes |
| `$log` | Muestra los últimos 10 mensajes borrados |
| `$setlog` | Configura el canal actual como canal de logs |

## Variables de entorno

| Variable | Descripción |
|---|---|
| `DISCORD_TOKEN` | Token del bot |
| `DISCORD_PREFIX` | Prefijo de comandos |
| `LOG_CHANNEL_ID` | ID del canal de logs |


## Deploy

Este bot está configurado para deployarse en **Railway**. Definí las variables de entorno desde el panel y listo.
