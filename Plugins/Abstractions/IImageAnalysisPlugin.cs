using BaseApi.Plugins.Abstractions;

namespace BaseApi.Plugins.Abstractions
{
    /// <summary>
    /// Interfaz para plugins que pueden analizar imágenes
    /// </summary>
    public interface IImageAnalysisPlugin : IPlugin
    {
        /// <summary>
        /// Analiza una imagen subida por el usuario
        /// </summary>
        /// <param name="imageData">Datos de la imagen en bytes</param>
        /// <param name="fileName">Nombre del archivo original</param>
        /// <param name="mimeType">Tipo MIME de la imagen</param>
        /// <param name="analysisType">Tipo de análisis solicitado</param>
        /// <param name="context">Contexto del análisis</param>
        /// <returns>Resultado del análisis</returns>
        Task<ImageAnalysisResponse> AnalyzeImageAsync(
            byte[] imageData, 
            string fileName, 
            string mimeType, 
            ImageAnalysisType analysisType,
            ImageAnalysisContext context);

        /// <summary>
        /// Verifica si el plugin puede analizar el tipo de imagen especificado
        /// </summary>
        /// <param name="mimeType">Tipo MIME de la imagen</param>
        /// <param name="analysisType">Tipo de análisis requerido</param>
        /// <returns>True si puede manejar este tipo de análisis</returns>
        bool CanAnalyze(string mimeType, ImageAnalysisType analysisType);

        /// <summary>
        /// Obtiene los tipos de imagen soportados
        /// </summary>
        /// <returns>Lista de tipos MIME soportados</returns>
        IEnumerable<string> GetSupportedImageTypes();

        /// <summary>
        /// Obtiene los tipos de análisis soportados
        /// </summary>
        /// <returns>Lista de tipos de análisis disponibles</returns>
        IEnumerable<ImageAnalysisType> GetSupportedAnalysisTypes();
    }

    /// <summary>
    /// Tipos de análisis de imagen disponibles
    /// </summary>
    public enum ImageAnalysisType
    {
        /// <summary>
        /// Análisis general de imagen
        /// </summary>
        General = 0,
        
        /// <summary>
        /// Análisis específico de facturas
        /// </summary>
        Invoice = 1,
        
        /// <summary>
        /// Análisis de recibos
        /// </summary>
        Receipt = 2,
        
        /// <summary>
        /// Análisis de documentos
        /// </summary>
        Document = 3,
        
        /// <summary>
        /// Extracción de texto (OCR)
        /// </summary>
        TextExtraction = 4,
        
        /// <summary>
        /// Análisis de código (screenshots de código)
        /// </summary>
        CodeAnalysis = 5
    }

    /// <summary>
    /// Contexto para el análisis de imagen
    /// </summary>
    public class ImageAnalysisContext
    {
        public string Username { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public Dictionary<string, object> AdditionalData { get; set; } = new();
        public string? UserPrompt { get; set; }
    }

    /// <summary>
    /// Respuesta del análisis de imagen
    /// </summary>
    public class ImageAnalysisResponse
    {
        /// <summary>
        /// Resultado del análisis en texto
        /// </summary>
        public string Analysis { get; set; } = string.Empty;

        /// <summary>
        /// Datos estructurados extraídos (para facturas, datos específicos)
        /// </summary>
        public Dictionary<string, object> ExtractedData { get; set; } = new();

        /// <summary>
        /// Nivel de confianza del análisis (0-1)
        /// </summary>
        public double Confidence { get; set; }

        /// <summary>
        /// Tiempo que tardó el procesamiento
        /// </summary>
        public TimeSpan ProcessingTime { get; set; }

        /// <summary>
        /// Indica si el análisis fue exitoso
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Código de error si falló
        /// </summary>
        public string? ErrorCode { get; set; }

        /// <summary>
        /// Mensaje de error si falló
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Metadatos adicionales
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new();

        /// <summary>
        /// Tipo de análisis realizado
        /// </summary>
        public ImageAnalysisType AnalysisType { get; set; }

        /// <summary>
        /// Información del archivo analizado
        /// </summary>
        public ImageFileInfo FileInfo { get; set; } = new();
    }

    /// <summary>
    /// Información del archivo de imagen
    /// </summary>
    public class ImageFileInfo
    {
        public string FileName { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
        public long SizeInBytes { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public string? Format { get; set; }
    }
}