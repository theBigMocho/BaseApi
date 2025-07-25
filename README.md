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

## Comandos Git Utilizados

### Inicialización del Repositorio
```bash
# Inicializar repositorio git local
git init

# Configurar remoto con usuario theBigMocho
git remote add origin https://github.com/theBigMocho/BaseApi.git

# Agregar todos los archivos al staging
git add .

# Verificar estado del repositorio
git status

# Crear commit inicial
git commit -m "Initial commit - BaseApi ASP.NET Core WebAPI project"

# Crear repositorio remoto usando GitHub CLI
gh repo create BaseApi --public --description "ASP.NET Core WebAPI con Entity Framework Core, archivos estáticos y componentes Shadow DOM"

# Push inicial al remoto y establecer tracking
git push -u origin master
```

## Tecnologías

- .NET 8
- ASP.NET Core WebAPI
- Swagger/OpenAPI
- Entity Framework Core
- SQL Server