# BaseApi - WebAPI .NET 8

Proyecto WebAPI creado con .NET 8 y controladores.

## Generación del Proyecto

El proyecto fue generado usando los siguientes comandos:

```bash
# Crear carpeta del proyecto
mkdir base && cd base

# Generar proyecto WebAPI .NET 8 con controladores
dotnet new webapi -n BaseApi --framework net8.0 --use-controllers
```

## Estructura del Proyecto

```
BaseApi/
├── Controllers/
│   └── WeatherForecastController.cs
├── Properties/
│   └── launchSettings.json
├── BaseApi.csproj
├── BaseApi.http
├── Program.cs
├── WeatherForecast.cs
├── appsettings.json
└── appsettings.Development.json
```

## Ejecutar el Proyecto

```bash
cd BaseApi
dotnet run
```

## Endpoints Disponibles

- `GET /WeatherForecast` - Obtiene datos de ejemplo del clima

## Tecnologías

- .NET 8
- ASP.NET Core WebAPI
- Swagger/OpenAPI