# CLAUDE.md - Contexto del Proyecto BaseApi

## Proyecto
- **Nombre**: BaseApi
- **Tipo**: ASP.NET Core WebAPI
- **Framework**: .NET 8
- **Arquitectura**: Controladores MVC
- **Base de datos**: Entity Framework Core con SQL Server
- **Servicio de archivos estáticos**: HTML, CSS, JavaScript, etc.
- **Componentes frontend**: Componentes reutilizables basados en Shadow DOM

## Instrucciones Importantes

### Documentación de Comandos
**IMPORTANTE**: Siempre que ejecutes comandos de terminal/bash durante el desarrollo, documenta estos comandos en el README.md del proyecto clasificándolos según su categoría. Esto incluye:

#### Clasificaciones de Comandos Bash:
- **.NET**: Comandos dotnet (build, run, restore, clean, new, add package)
- **EF-Core**: Comandos de Entity Framework (migrations, database update)  
- **Git**: Comandos de control de versiones (init, add, commit, push, clone, etc.)
- **SQL**: Comandos de base de datos (sqlcmd, conexiones, consultas)
- **Otras clasificaciones**: Crear nuevas categorías según aparezcan comandos de diferentes tecnologías

#### Tipos de comandos a documentar:
- Comandos de generación de código
- Comandos de instalación de paquetes
- Comandos de migración de base de datos
- Comandos de build y deployment
- Comandos de Git y control de versiones
- Comandos de base de datos y SQL
- Cualquier script personalizado

### Comandos de Generación del Proyecto
```bash
# Creación inicial del proyecto
mkdir base && cd base
dotnet new webapi -n BaseApi --framework net8.0 --use-controllers
```

### Comandos Útiles
```bash
# Ejecutar el proyecto
dotnet run

# Restaurar paquetes
dotnet restore

# Compilar el proyecto
dotnet build

# Limpiar build
dotnet clean
```

## Estructura del Proyecto
- Proyecto WebAPI con controladores
- Swagger integrado por defecto
- Configuración en appsettings.json
- Controlador de ejemplo: WeatherForecastController

## Convenciones
- Usar controladores con atributos [ApiController]
- Seguir patrones RESTful
- Documentar endpoints con XML comments para Swagger
- Mantener separación de responsabilidades
- **Crear tests unitarios** siempre que la ocasión o el desarrollo de alguna característica lo amerite

### Base de Datos
- Usar Entity Framework Core para acceso a datos
- Configurar conexión a SQL Server
- Implementar patrones Repository/Unit of Work cuando sea apropiado
- Aplicar migraciones para cambios de esquema

### Frontend
- Crear componentes reutilizables usando Shadow DOM
- Mantener separación entre lógica de componentes y estilos
- Organizar archivos estáticos en estructura clara (HTML, CSS, JS)
- Aplicar principios de encapsulación en componentes