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

### Autenticación
- `POST /api/auth/login` - Iniciar sesión de usuario
- `POST /api/auth/register` - Registrar nuevo usuario

### Mantenedor de Usuarios (Requiere autenticación JWT)
- `GET /api/users` - Obtiene todos los usuarios
- `GET /api/users/{id}` - Obtiene un usuario por ID
- `POST /api/users` - Crea un nuevo usuario
- `PUT /api/users/{id}` - Actualiza un usuario existente
- `PATCH /api/users/{id}/activate` - Activa un usuario (dar de alta)
- `PATCH /api/users/{id}/deactivate` - Desactiva un usuario (dar de baja)
- `DELETE /api/users/{id}` - Elimina un usuario permanentemente

### Otros
- `GET /WeatherForecast` - Obtiene datos de ejemplo del clima

## Interfaz Web - Progressive Web App (PWA)

El proyecto incluye una **Progressive Web App** completa con:
- **Shadow DOM Components** - Componentes reutilizables encapsulados
- **HTML5, CSS3, JavaScript** - Tecnologías web estándar modernas
- **Responsive Design** - Adaptable a dispositivos móviles y tablets
- **JWT Authentication** - Integración completa con la API
- **Funcionalidad Offline** - Caché inteligente con Service Worker
- **Instalable** - Se puede instalar como aplicación nativa
- **Notificaciones Push** - Soporte para notificaciones del sistema
- **Actualizaciones Automáticas** - Gestión de versiones transparente

### Páginas Disponibles
- `/` - Página de login con componente Shadow DOM
- `/dashboard.html` - Panel de control (requiere autenticación)
- `/offline.html` - Página mostrada cuando no hay conexión
- `/generate-icons.html` - Generador de iconos PWA
- `/swagger` - Documentación de la API

### Componentes Shadow DOM
- `<login-component>` - Componente de autenticación con login y registro

### Características PWA
- **📱 Instalación**: Se puede instalar desde el navegador como app nativa
- **🔄 Service Worker**: Caché inteligente para funcionalidad offline
- **📤 Offline First**: Funciona sin conexión a internet
- **🔔 Notificaciones**: Soporte para push notifications
- **🚀 Performance**: Carga rápida con caché estratégico
- **📲 App-like**: Experiencia similar a aplicación nativa
- **🔄 Actualizaciones**: Sistema automático de actualizaciones
- **🎨 Iconos**: Iconos adaptativos para todos los dispositivos

### Instalación PWA
1. **Navegador**: Aparecerá banner de instalación automáticamente
2. **Manual**: Ctrl+Shift+I para forzar instalación (testing)
3. **Móvil**: "Agregar a pantalla de inicio" desde el menú del navegador
4. **Desktop**: "Instalar BaseApi" desde la barra de direcciones

### Uso de la Interfaz Web
1. **Registro de usuario**: Completa todos los campos incluyendo confirmación de contraseña
2. **Login**: Usa las credenciales creadas durante el registro
3. **Dashboard**: Después del login exitoso, accede al panel de control
4. **Gestión**: Usa los endpoints protegidos desde el dashboard

### Validaciones Implementadas
- **Email válido**: Formato de email correcto
- **Contraseña**: Mínimo 6 caracteres
- **Confirmación**: Las contraseñas deben coincidir
- **Campos únicos**: Username y email deben ser únicos en el sistema

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

# Instalar paquetes de autenticación JWT
dotnet add package System.IdentityModel.Tokens.Jwt --version 8.13.0
dotnet add package BCrypt.Net-Next --version 4.0.3
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.11

# Crear estructura de archivos web
mkdir -p wwwroot/components wwwroot/css wwwroot/js

# Crear estructura para PWA
mkdir -p wwwroot/images

# Comandos de mantenimiento automático
dotnet build --configuration Release
dotnet test --logger trx --collect:"XPlat Code Coverage"
dotnet format --verify-no-changes
```

### Clasificación EF-Core
```bash
# Crear migración inicial de Entity Framework
dotnet ef migrations add InitialCreate

# Aplicar migración a la base de datos
dotnet ef database update
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

# Commits automáticos con descripción basada en cambios
# (Cuando se solicite commit y push, se analizarán los cambios y se creará un mensaje descriptivo automáticamente)
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

# Probar conexión a SQL Server 2022 con usuario sa
sqlcmd -S ".\\SQLSERVER2022" -U sa -P mocho -Q "SELECT @@SERVERNAME, @@VERSION"
```

### Clasificación Mantenimiento Automático
```bash
# Testing y Calidad
dotnet test --logger trx --collect:"XPlat Code Coverage"
dotnet format --verify-no-changes
dotnet format --include "**/*.cs"

# Análisis de código
dotnet build --configuration Release --verbosity normal
dotnet clean && dotnet restore && dotnet build

# Seguridad y Dependencias
dotnet list package --outdated
dotnet list package --vulnerable
dotnet add package <PackageName> --version <LatestVersion>

# Documentación automática
dotnet build-server shutdown
dotnet tool install -g dotnet-reportgenerator-globaltool

# Base de datos - Mantenimiento
dotnet ef database drop --force
dotnet ef database update
dotnet ef migrations list

# Performance y Monitoreo
dotnet run --configuration Release
dotnet publish --configuration Release --output ./publish

# Limpieza automática
dotnet clean
dotnet nuget locals all --clear

# Health checks y validación
curl -X GET "http://localhost:5012/api/users" -H "Authorization: Bearer <token>"
curl -X POST "http://localhost:5012/api/auth/login" -H "Content-Type: application/json"
```

## Tecnologías

- .NET 8
- ASP.NET Core WebAPI
- Swagger/OpenAPI
- Entity Framework Core
- SQL Server 2022 Express
- JWT Authentication
- BCrypt Password Hashing

## Configuración de Base de Datos

El proyecto está configurado para usar SQL Server 2022 con:
- Servidor: `.\\SQLSERVER2022`
- Base de datos: `BaseApi`
- Usuario: `sa`
- Contraseña: `mocho`

## Permisos Otorgados a Claude Code

Durante el desarrollo de este proyecto, se han otorgado los siguientes permisos y capacidades a Claude Code:

### 🗂️ Sistema de Archivos
- **Crear archivos**: Crear nuevos archivos (.cs, .html, .css, .js, .json, .md)
- **Leer archivos**: Leer contenido de archivos existentes para análisis y modificación
- **Escribir archivos**: Modificar y actualizar archivos existentes
- **Crear directorios**: Crear estructura de carpetas (wwwroot, components, css, js, etc.)
- **Navegar estructura**: Explorar y analizar la estructura de directorios del proyecto

### ⚙️ Gestión de Procesos
- **Ejecutar procesos**: Iniciar aplicaciones (.NET, SQL Server, etc.)
- **Finalizar procesos**: Terminar procesos en ejecución (taskkill de BaseApi.exe)
- **Monitorear procesos**: Verificar estado de procesos (tasklist)
- **Gestión de puertos**: Controlar aplicaciones en puertos específicos (localhost:5012)

### 🗄️ Base de Datos
- **Conexiones SQL**: Conectar y probar conexiones a SQL Server 2022
- **Ejecutar consultas**: Verificar versión y estado del servidor (SELECT @@VERSION)
- **Migraciones EF Core**: Crear y aplicar migraciones de Entity Framework
- **Operaciones CRUD**: Gestionar usuarios y datos a través de la API

### 🌐 Red y APIs
- **Peticiones HTTP**: Realizar llamadas a endpoints de la API
- **Configuración de servidores**: Configurar servidor web para archivos estáticos
- **Gestión de URLs**: Configurar rutas y endpoints
- **Integración con servicios**: Configurar JWT, autenticación, CORS

### 📝 Control de Versiones (Git)
- **Inicialización**: Inicializar repositorios git
- **Gestión de archivos**: git add, git status, verificación de cambios
- **Commits**: Crear commits con mensajes descriptivos
- **Push/Pull**: Sincronizar con repositorios remotos
- **Configuración remota**: Configurar origin y upstream
- **GitHub CLI**: Crear repositorios usando gh CLI

### 💻 Terminal/Bash y Herramientas CLI
- **Comandos .NET**: dotnet build, run, restore, clean, new, add package
- **Entity Framework**: dotnet ef migrations, database update
- **SQL Server**: sqlcmd para pruebas de conexión
- **Gestión de paquetes**: Instalación de NuGet packages
- **GitHub CLI**: Comandos gh para gestión de repositorios
- **Gestión de directorios**: mkdir, navegación de carpetas

### 🔧 Herramientas de Desarrollo
- **Visual Studio/VS Code**: Trabajo con archivos de proyecto (.csproj, .sln)
- **Package Managers**: NuGet, npm (según necesidades)
- **Debugging**: Análisis de errores y logs de aplicación
- **Testing**: Capacidad para ejecutar y crear tests unitarios

### 🔒 Seguridad y Autenticación
- **Configuración JWT**: Manejo de tokens y autenticación
- **Hash de contraseñas**: Implementación de BCrypt
- **Manejo de secrets**: Configuración de connection strings y claves
- **Validaciones**: Implementación de validaciones de datos

### 📊 Monitoreo y Logs
- **Análisis de logs**: Lectura e interpretación de logs de aplicación
- **Debugging**: Identificación y resolución de errores
- **Performance**: Monitoreo de rendimiento de aplicación
- **Validación**: Verificación de funcionalidad implementada

**Nota de Transparencia**: Esta documentación se mantiene actualizada para proporcionar visibilidad completa sobre las capacidades y permisos otorgados a Claude Code durante el desarrollo del proyecto BaseApi.

## 🔄 Sistema de Mantenimiento Automático

Este proyecto implementa un sistema integral de mantenimiento automático que garantiza la actualización continua de todos los aspectos del desarrollo. Los siguientes elementos se mantienen automáticamente:

### 📚 Documentación Técnica
- ✅ **README.md actualizado automáticamente** con nuevos endpoints y comandos
- ✅ **CLAUDE.md con reglas actualizadas** según patrones implementados
- 🔄 **Comentarios XML para Swagger** en todos los controladores
- 🔄 **Changelog automático** basado en commits semánticos
- 🔄 **Documentación de arquitectura** actualizada con cambios estructurales

### 🔧 Configuración y Dependencias
- ✅ **Gestión de paquetes NuGet** documentada y versionada
- ✅ **Connection strings** configuradas por ambiente
- 🔄 **Variables de entorno** sincronizadas entre dev/staging/prod
- 🔄 **Renovación de certificados SSL** automática
- 🔄 **Configuración CORS** adaptativa según entornos

### 🗄️ Base de Datos
- ✅ **Migraciones EF Core** aplicadas automáticamente
- ✅ **Esquema de base de datos** documentado y versionado
- 🔄 **Scripts de datos iniciales** para desarrollo/testing
- 🔄 **Optimización de índices** según patrones de consulta
- 🔄 **Respaldos programados** y rotación de logs

### 🔒 Seguridad
- ✅ **Autenticación JWT** implementada con mejores prácticas
- ✅ **Hash de contraseñas** con BCrypt actualizado
- 🔄 **Rotación de secretos JWT** programada
- 🔄 **Escaneo de vulnerabilidades** en dependencias
- 🔄 **Monitoreo de accesos** y logs de seguridad

### 🧪 Testing y Calidad
- 🔄 **Tests unitarios** ejecutados en cada commit
- 🔄 **Cobertura de código** reportada automáticamente
- 🔄 **Linting y formato** aplicado en build
- 🔄 **Benchmarks de rendimiento** medidos continuamente
- 🔄 **Tests de integración E2E** en pipeline CI/CD

### 📊 Monitoreo y Métricas
- 🔄 **Health checks** de servicios y base de datos
- 🔄 **Métricas de performance** (CPU, memoria, respuesta)
- 🔄 **Logs de errores** capturados automáticamente
- 🔄 **Analytics de uso** de endpoints
- 🔄 **Sistema de alertas** por fallos

### 🚀 Deployment y DevOps
- 🔄 **Pipeline CI/CD** completo automatizado
- 🔄 **Contenedores Docker** actualizados automáticamente
- 🔄 **Configuraciones Kubernetes** sincronizadas
- 🔄 **Deploy multi-ambiente** (dev/staging/prod)
- 🔄 **Rollback automático** en caso de fallos

### 🌐 Frontend Web
- ✅ **Componentes Shadow DOM** implementados y documentados
- ✅ **Interfaz responsive** con CSS moderno
- 🔄 **Optimización de assets** (minificación CSS/JS)
- 🔄 **Testing cross-browser** automatizado
- 🔄 **PWA configuration** actualizada
- 🔄 **Cache invalidation** en deployments

### 📋 Gestión de Proyecto
- ✅ **TODO lists** actualizadas automáticamente durante desarrollo
- 🔄 **Sincronización GitHub Issues** con tareas
- 🔄 **Release notes** generadas automáticamente
- 🔄 **Notificaciones de cambios** importantes
- 🔄 **Asignación de code reviewers** automática

### 🔄 Integraciones Externas
- 🔄 **APIs de terceros** actualizadas automáticamente
- 🔄 **Webhooks** configurados y testeados
- 🔄 **Proveedores OAuth** sincronizados
- 🔄 **Templates de notificación** actualizados
- 🔄 **Rate limiting** ajustado según uso

## 🎯 Estado del Mantenimiento Automático

**Leyenda:**
- ✅ **Implementado y activo** - Funcionalidad completamente operativa
- 🔄 **Programado para implementación** - Será implementado según necesidades del proyecto
- ⚠️ **Requiere configuración adicional** - Necesita setup específico del entorno

### 📈 Beneficios del Sistema Automático
1. **Reducción de errores humanos** en tareas repetitivas
2. **Consistencia** en aplicación de estándares y convenciones
3. **Eficiencia** en el ciclo de desarrollo y deployment
4. **Transparencia** total en cambios y decisiones
5. **Calidad mejorada** mediante testing y monitoreo continuo
6. **Seguridad fortalecida** con actualizaciones proactivas
7. **Documentación siempre actualizada** sin intervención manual

### 🔧 Configuración del Sistema
El sistema de mantenimiento automático se activa mediante:
- **Hooks de Git** para ejecución en commits/push
- **GitHub Actions** para CI/CD pipeline
- **Cron jobs** para tareas programadas
- **Webhooks** para integraciones externas
- **Monitoring agents** para métricas en tiempo real

**Nota**: Este sistema evoluciona continuamente agregando nuevas capacidades de automatización según las necesidades del proyecto.