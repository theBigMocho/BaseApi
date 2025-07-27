namespace BaseApi.Plugins.Abstractions
{
    /// <summary>
    /// Interfaz base para todos los plugins de BaseApi
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Nombre único del plugin
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Versión del plugin
        /// </summary>
        string Version { get; }
        
        /// <summary>
        /// Descripción del plugin
        /// </summary>
        string Description { get; }
        
        /// <summary>
        /// Autor del plugin
        /// </summary>
        string Author { get; }
        
        /// <summary>
        /// Indica si el plugin está habilitado
        /// </summary>
        bool IsEnabled { get; set; }
        
        /// <summary>
        /// Inicializa el plugin
        /// </summary>
        Task InitializeAsync();
        
        /// <summary>
        /// Finaliza el plugin y libera recursos
        /// </summary>
        Task DisposeAsync();
        
        /// <summary>
        /// Verifica la salud del plugin
        /// </summary>
        Task<PluginHealthStatus> CheckHealthAsync();
    }
    
    /// <summary>
    /// Estado de salud del plugin
    /// </summary>
    public enum PluginHealthStatus
    {
        Healthy,
        Degraded,
        Unhealthy
    }
}