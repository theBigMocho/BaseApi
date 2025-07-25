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

### Control de Versiones (Git)
- **Commits automáticos**: Cuando se solicite hacer commit y push, analizar los cambios realizados y crear un commit con una descripción clara y detallada que refleje las modificaciones implementadas
- **Mensajes descriptivos**: Los commits deben incluir:
  - Tipo de cambio (feat, fix, refactor, docs, style, test, etc.)
  - Descripción breve pero específica de los cambios
  - Detalles adicionales si es necesario
- **Push automático**: Después del commit, realizar push automáticamente al repositorio remoto
- **Formato de mensajes**: Seguir convenciones de commits semánticos cuando sea apropiado

### Documentación de Permisos de Claude Code
- **Registrar permisos otorgados**: Documentar todos los permisos y capacidades otorgadas a Claude Code durante el desarrollo del proyecto
- **Categorías de permisos a documentar**:
  - **Sistema de archivos**: Crear, leer, escribir, eliminar archivos y carpetas
  - **Procesos**: Ejecutar, finalizar, monitorear procesos del sistema
  - **Base de datos**: Conexiones, migraciones, operaciones CRUD
  - **Red**: Peticiones HTTP, conexiones de red, APIs externas
  - **Git**: Operaciones de control de versiones (add, commit, push, pull, etc.)
  - **Terminal/Bash**: Ejecución de comandos del sistema operativo
  - **Herramientas de desarrollo**: dotnet CLI, Entity Framework CLI, npm, etc.
- **Propósito**: Mantener transparencia sobre las acciones que Claude Code puede realizar en el sistema
- **Ubicación**: Mantener esta documentación actualizada en el README.md del proyecto

## Mantenimiento Automático del Proyecto

### 📚 Documentación Técnica (Actualización Automática)
- **README.md**: Mantener actualizado con nuevos endpoints, comandos bash utilizados, tecnologías incorporadas
- **CLAUDE.md**: Agregar nuevas reglas, convenciones y patrones implementados automáticamente
- **Comentarios de API**: Actualizar documentación XML en controladores para Swagger/OpenAPI
- **Changelog**: Registrar cambios de versión automáticamente en cada release
- **Documentación de arquitectura**: Actualizar diagramas y documentación de componentes cuando se modifique la estructura

### 🔧 Configuración y Dependencias (Gestión Automática)
- **Versiones de paquetes**: Documentar actualizaciones de NuGet packages y alertar sobre vulnerabilidades de seguridad
- **Connection strings**: Actualizar automáticamente configuraciones de base de datos según ambiente
- **Variables de entorno**: Mantener sincronizadas configuraciones en appsettings.json para dev/staging/prod
- **Certificados SSL**: Implementar renovación automática y alertas de vencimiento
- **Configuración CORS**: Actualizar URLs permitidas según entornos de despliegue

### 🗄️ Base de Datos (Mantenimiento Automático)
- **Migraciones EF Core**: Aplicar automáticamente en deployments y documentar cambios de esquema
- **Scripts de semillas**: Mantener datos iniciales actualizados para desarrollo y testing
- **Índices de optimización**: Crear y mantener índices según patrones de consulta
- **Respaldos automáticos**: Implementar políticas de backup programadas
- **Limpieza de logs**: Rotación automática de logs de auditoría y transacciones

### 🔒 Seguridad (Mantenimiento Automático)
- **JWT Secrets**: Implementar rotación automática de claves de firma
- **Algoritmos de hash**: Actualizar BCrypt y otros algoritmos según mejores prácticas
- **Certificados TLS**: Renovación automática de certificados SSL/TLS
- **Escaneo de vulnerabilidades**: Verificación automática de dependencias vulnerables
- **Logs de seguridad**: Monitoreo automático de intentos de acceso no autorizado

### 🧪 Testing y Calidad (Ejecución Automática)
- **Tests unitarios**: Ejecutar automáticamente en cada commit/push con reporte de resultados
- **Cobertura de código**: Generar reportes de coverage actualizados
- **Linting y formato**: Aplicar reglas de código automáticamente (EditorConfig, StyleCop)
- **Benchmarks de performance**: Medir métricas de rendimiento en cada build
- **Tests de integración**: Ejecutar tests E2E automáticamente en pipeline CI/CD

### 📊 Monitoreo y Métricas (Seguimiento Automático)
- **Health checks**: Verificación automática de estado de servicios y base de datos
- **Métricas de performance**: Monitoreo de tiempo de respuesta, memoria y CPU
- **Tracking de errores**: Captura automática de logs de errores y excepciones
- **Analytics de uso**: Recopilar métricas de uso de endpoints automáticamente
- **Sistema de alertas**: Notificaciones automáticas por fallos o degradación

### 🚀 Deployment y DevOps (Automatización Completa)
- **Pipelines CI/CD**: Build, test y deploy automático en cada commit
- **Contenedorización**: Actualización automática de imágenes Docker
- **Configuración de orquestación**: Mantener manifiestos Kubernetes actualizados
- **Sincronización de ambientes**: Mantener configuraciones consistentes entre entornos
- **Procedimientos de rollback**: Reversión automática en caso de fallos en producción

### 🌐 Frontend/Interface Web (Mantenimiento Automático)
- **Componentes Shadow DOM**: Versionado automático y verificación de compatibilidad
- **Optimización de assets**: Minificación automática de CSS/JS en build
- **Compatibilidad cross-browser**: Testing automático en múltiples navegadores
- **PWA/Manifest**: Actualización automática de configuración Progressive Web App
- **Cache invalidation**: Invalidación automática de CDN cache en deployments

### 📋 Gestión de Proyecto (Automatización)
- **TODO lists**: Actualización automática de estado de tareas y pendientes
- **Issue tracking**: Sincronización automática con GitHub Issues
- **Release notes**: Generación automática de notas de versión basadas en commits
- **Notificaciones de equipo**: Alertas automáticas de cambios importantes
- **Code reviews**: Asignación automática de reviewers según archivos modificados

### 🔄 Integraciones Externas (Mantenimiento Automático)
- **Clientes de API**: Actualización automática de SDKs de servicios de terceros
- **Webhooks**: Configuración y testing automático de endpoints de notificación
- **Proveedores OAuth**: Actualización automática de configuraciones de autenticación
- **Templates de email**: Mantener plantillas de notificaciones actualizadas
- **Rate limiting**: Ajuste automático de límites según patrones de uso observados

### 🎯 Principios de Mantenimiento Automático
- **Proactividad**: Anticipar necesidades de mantenimiento antes de que se conviertan en problemas
- **Transparencia**: Documentar automáticamente todos los cambios y decisiones
- **Consistencia**: Aplicar estándares y convenciones de manera uniforme
- **Monitoreo continuo**: Supervisar constantemente el estado del sistema y dependencias
- **Adaptabilidad**: Ajustar automáticamente configuraciones según el contexto y ambiente