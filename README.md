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

## Comandos Bash Utilizados

### Clasificación .NET
```bash
# Generar proyecto WebAPI .NET 8 con controladores
dotnet new webapi -n BaseApi --framework net8.0 --use-controllers

# Restaurar paquetes
dotnet restore

# Compilar el proyecto
dotnet build

# Limpiar build
dotnet clean

# Ejecutar el proyecto
dotnet run

# Instalar paquetes Entity Framework Core
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Design
```

### Clasificación EF-Core
```bash
# Los comandos de EF Core se ejecutarán cuando se creen migraciones
# dotnet ef migrations add InitialCreate
# dotnet ef database update
```

### Clasificación Git
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

# Remover archivos obj/ del repositorio (ya estaban commitados)
git rm -r --cached obj/

# Agregar .gitignore y cambios en documentación
git add .gitignore CLAUDE.md README.md

# Push de cambios
git push
```

### Clasificación SQL
```bash
# Listar instancias de SQL Server en el sistema
sqlcmd -L

# Probar conexión a instancia por defecto
sqlcmd -S . -E -Q "SELECT @@SERVERNAME, @@VERSION"

# Probar conexión a localhost
sqlcmd -S localhost -E -Q "SELECT @@SERVERNAME, @@VERSION"

# Probar conexión a LocalDB con autenticación Windows
sqlcmd -S "(localdb)\\MSSQLLocalDB" -E -Q "SELECT @@SERVERNAME, @@VERSION"
```

## Tecnologías

- .NET 8
- ASP.NET Core WebAPI
- Swagger/OpenAPI
- Entity Framework Core
- SQL Server