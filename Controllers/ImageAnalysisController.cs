using Microsoft.AspNetCore.Mvc;
using BaseApi.Plugins.Abstractions;
using BaseApi.Plugins.Core;

namespace BaseApi.Controllers
{
    /// <summary>
    /// Controlador para análisis de imágenes (facturas, documentos, etc.)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ImageAnalysisController : ControllerBase
    {
        private readonly IPluginManager _pluginManager;
        private readonly ILogger<ImageAnalysisController> _logger;

        // Tamaño máximo de archivo (10MB)
        private const long MaxFileSize = 10 * 1024 * 1024;

        // Tipos de archivo permitidos
        private readonly HashSet<string> _allowedMimeTypes = new()
        {
            "image/jpeg",
            "image/jpg",
            "image/png", 
            "image/gif",
            "image/bmp",
            "image/tiff",
            "image/webp"
        };

        public ImageAnalysisController(IPluginManager pluginManager, ILogger<ImageAnalysisController> logger)
        {
            _pluginManager = pluginManager;
            _logger = logger;
        }

        /// <summary>
        /// Analiza una imagen subida (factura, recibo, documento)
        /// </summary>
        /// <param name="file">Archivo de imagen</param>
        /// <param name="analysisType">Tipo de análisis a realizar</param>
        /// <param name="userPrompt">Instrucciones adicionales del usuario (opcional)</param>
        /// <returns>Resultado del análisis</returns>
        [HttpPost("analyze")]
        public async Task<ActionResult<ImageAnalysisResponse>> AnalyzeImage(
            IFormFile file,
            [FromForm] ImageAnalysisType analysisType = ImageAnalysisType.General,
            [FromForm] string? userPrompt = null)
        {
            try
            {
                // 1. Validar archivo
                var validationResult = ValidateFile(file);
                if (validationResult != null)
                {
                    return validationResult;
                }

                _logger.LogInformation("Procesando imagen: {FileName} ({Size} bytes, tipo: {AnalysisType})", 
                    file.FileName, file.Length, analysisType);

                // 2. Leer datos del archivo
                byte[] imageData;
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    imageData = memoryStream.ToArray();
                }

                // 3. Buscar plugin que pueda manejar este tipo de análisis
                var imagePlugins = _pluginManager.GetPlugins<IImageAnalysisPlugin>().ToList();
                var plugin = imagePlugins.FirstOrDefault(p => p.CanAnalyze(file.ContentType, analysisType));

                if (plugin == null)
                {
                    return BadRequest(new
                    {
                        error = "NO_PLUGIN_AVAILABLE",
                        message = $"No hay plugins disponibles para analizar {file.ContentType} con tipo {analysisType}",
                        availablePlugins = imagePlugins.Select(p => new { p.Name, p.Version }).ToList()
                    });
                }

                // 4. Crear contexto de análisis
                var context = new ImageAnalysisContext
                {
                    Username = User.Identity?.Name ?? "Usuario",
                    SessionId = Guid.NewGuid().ToString(),
                    Timestamp = DateTime.UtcNow,
                    UserPrompt = userPrompt
                };

                // 5. Ejecutar análisis
                var result = await plugin.AnalyzeImageAsync(
                    imageData,
                    file.FileName,
                    file.ContentType,
                    analysisType,
                    context);

                _logger.LogInformation("Análisis completado para {FileName} usando plugin {PluginName} en {Time}ms", 
                    file.FileName, plugin.Name, result.ProcessingTime.TotalMilliseconds);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando imagen {FileName}", file?.FileName);
                return StatusCode(500, new
                {
                    error = "PROCESSING_ERROR",
                    message = "Error interno procesando la imagen",
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Analiza múltiples imágenes en lote
        /// </summary>
        /// <param name="files">Lista de archivos de imagen</param>
        /// <param name="analysisType">Tipo de análisis a realizar</param>
        /// <param name="userPrompt">Instrucciones adicionales del usuario (opcional)</param>
        /// <returns>Lista de resultados de análisis</returns>
        [HttpPost("analyze-batch")]
        public async Task<ActionResult<List<ImageAnalysisResponse>>> AnalyzeBatchImages(
            List<IFormFile> files,
            [FromForm] ImageAnalysisType analysisType = ImageAnalysisType.General,
            [FromForm] string? userPrompt = null)
        {
            try
            {
                if (files == null || !files.Any())
                {
                    return BadRequest("No se proporcionaron archivos para analizar");
                }

                if (files.Count > 10) // Límite de 10 archivos por lote
                {
                    return BadRequest("Máximo 10 archivos por lote");
                }

                var results = new List<ImageAnalysisResponse>();

                foreach (var file in files)
                {
                    // Validar cada archivo
                    var validationResult = ValidateFile(file);
                    if (validationResult != null)
                    {
                        // Agregar error para este archivo específico
                        results.Add(new ImageAnalysisResponse
                        {
                            IsSuccess = false,
                            ErrorCode = "VALIDATION_ERROR",
                            ErrorMessage = $"Error en {file.FileName}: archivo inválido",
                            FileInfo = new ImageFileInfo
                            {
                                FileName = file.FileName,
                                MimeType = file.ContentType,
                                SizeInBytes = file.Length
                            }
                        });
                        continue;
                    }

                    // Procesar archivo
                    byte[] imageData;
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        imageData = memoryStream.ToArray();
                    }

                    var imagePlugins = _pluginManager.GetPlugins<IImageAnalysisPlugin>().ToList();
                    var plugin = imagePlugins.FirstOrDefault(p => p.CanAnalyze(file.ContentType, analysisType));

                    if (plugin == null)
                    {
                        results.Add(new ImageAnalysisResponse
                        {
                            IsSuccess = false,
                            ErrorCode = "NO_PLUGIN_AVAILABLE",
                            ErrorMessage = $"No hay plugins para {file.ContentType}",
                            FileInfo = new ImageFileInfo
                            {
                                FileName = file.FileName,
                                MimeType = file.ContentType,
                                SizeInBytes = file.Length
                            }
                        });
                        continue;
                    }

                    var context = new ImageAnalysisContext
                    {
                        Username = User.Identity?.Name ?? "Usuario",
                        SessionId = Guid.NewGuid().ToString(),
                        Timestamp = DateTime.UtcNow,
                        UserPrompt = userPrompt
                    };

                    var result = await plugin.AnalyzeImageAsync(
                        imageData,
                        file.FileName,
                        file.ContentType,
                        analysisType,
                        context);

                    results.Add(result);
                }

                _logger.LogInformation("Procesamiento en lote completado: {TotalFiles} archivos, {SuccessCount} exitosos", 
                    files.Count, results.Count(r => r.IsSuccess));

                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando lote de imágenes");
                return StatusCode(500, new
                {
                    error = "BATCH_PROCESSING_ERROR",
                    message = "Error interno procesando el lote de imágenes",
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene información sobre los tipos de análisis y formatos soportados
        /// </summary>
        /// <returns>Información de capacidades del sistema</returns>
        [HttpGet("capabilities")]
        public ActionResult GetCapabilities()
        {
            try
            {
                var imagePlugins = _pluginManager.GetPlugins<IImageAnalysisPlugin>().ToList();

                var capabilities = new
                {
                    maxFileSize = MaxFileSize,
                    maxFileSizeMB = MaxFileSize / (1024 * 1024),
                    supportedFormats = _allowedMimeTypes.ToList(),
                    availablePlugins = imagePlugins.Select(p => new
                    {
                        name = p.Name,
                        version = p.Version,
                        description = p.Description,
                        author = p.Author,
                        isEnabled = p.IsEnabled,
                        supportedImageTypes = p.GetSupportedImageTypes().ToList(),
                        supportedAnalysisTypes = p.GetSupportedAnalysisTypes().Select(t => new
                        {
                            value = (int)t,
                            name = t.ToString()
                        }).ToList()
                    }).ToList(),
                    analysisTypes = Enum.GetValues<ImageAnalysisType>().Select(t => new
                    {
                        value = (int)t,
                        name = t.ToString(),
                        description = GetAnalysisTypeDescription(t)
                    }).ToList()
                };

                return Ok(capabilities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo capacidades del sistema");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene el estado de salud de los plugins de análisis de imagen
        /// </summary>
        /// <returns>Estado de los plugins</returns>
        [HttpGet("health")]
        public async Task<ActionResult> GetHealthStatus()
        {
            try
            {
                var imagePlugins = _pluginManager.GetPlugins<IImageAnalysisPlugin>().ToList();
                var healthStatus = new List<object>();

                foreach (var plugin in imagePlugins)
                {
                    var health = await plugin.CheckHealthAsync();
                    healthStatus.Add(new
                    {
                        name = plugin.Name,
                        version = plugin.Version,
                        isEnabled = plugin.IsEnabled,
                        health = health.ToString(),
                        isHealthy = health == PluginHealthStatus.Healthy
                    });
                }

                var overall = new
                {
                    totalPlugins = imagePlugins.Count,
                    healthyPlugins = healthStatus.Count(p => ((dynamic)p).isHealthy),
                    isSystemHealthy = healthStatus.All(p => ((dynamic)p).isHealthy),
                    plugins = healthStatus,
                    timestamp = DateTime.UtcNow
                };

                return Ok(overall);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verificando estado de salud");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        #region Private Methods
        private ActionResult? ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No se proporcionó archivo o el archivo está vacío");
            }

            if (file.Length > MaxFileSize)
            {
                return BadRequest($"El archivo excede el tamaño máximo permitido de {MaxFileSize / (1024 * 1024)}MB");
            }

            if (string.IsNullOrEmpty(file.ContentType) || !_allowedMimeTypes.Contains(file.ContentType.ToLower()))
            {
                return BadRequest($"Tipo de archivo no soportado: {file.ContentType}. Tipos permitidos: {string.Join(", ", _allowedMimeTypes)}");
            }

            return null;
        }

        private string GetAnalysisTypeDescription(ImageAnalysisType analysisType)
        {
            return analysisType switch
            {
                ImageAnalysisType.General => "Análisis general de imagen",
                ImageAnalysisType.Invoice => "Análisis especializado de facturas con extracción de datos estructurados",
                ImageAnalysisType.Receipt => "Análisis de recibos y boletas",
                ImageAnalysisType.Document => "Análisis de documentos en general",
                ImageAnalysisType.TextExtraction => "Extracción de texto (OCR)",
                ImageAnalysisType.CodeAnalysis => "Análisis de capturas de código fuente",
                _ => "Tipo de análisis desconocido"
            };
        }
        #endregion
    }
}