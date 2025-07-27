using BaseApi.Plugins.Abstractions;

namespace BaseApi.Plugins.Core
{
    /// <summary>
    /// Interfaz para el gestor de plugins
    /// </summary>
    public interface IPluginManager
    {
        /// <summary>
        /// Registra un plugin
        /// </summary>
        /// <typeparam name="T">Tipo del plugin</typeparam>
        /// <param name="plugin">Instancia del plugin</param>
        void RegisterPlugin<T>(T plugin) where T : class, IPlugin;
        
        /// <summary>
        /// Obtiene un plugin por tipo
        /// </summary>
        /// <typeparam name="T">Tipo del plugin</typeparam>
        /// <returns>Plugin encontrado o null</returns>
        T? GetPlugin<T>() where T : class, IPlugin;
        
        /// <summary>
        /// Obtiene todos los plugins de un tipo específico
        /// </summary>
        /// <typeparam name="T">Tipo del plugin</typeparam>
        /// <returns>Lista de plugins</returns>
        IEnumerable<T> GetPlugins<T>() where T : class, IPlugin;
        
        /// <summary>
        /// Obtiene todos los plugins registrados
        /// </summary>
        /// <returns>Lista de todos los plugins</returns>
        IEnumerable<IPlugin> GetAllPlugins();
        
        /// <summary>
        /// Habilita un plugin
        /// </summary>
        /// <param name="pluginName">Nombre del plugin</param>
        Task EnablePluginAsync(string pluginName);
        
        /// <summary>
        /// Deshabilita un plugin
        /// </summary>
        /// <param name="pluginName">Nombre del plugin</param>
        Task DisablePluginAsync(string pluginName);
        
        /// <summary>
        /// Inicializa todos los plugins habilitados
        /// </summary>
        Task InitializePluginsAsync();
        
        /// <summary>
        /// Finaliza todos los plugins
        /// </summary>
        Task DisposePluginsAsync();
        
        /// <summary>
        /// Verifica la salud de todos los plugins
        /// </summary>
        Task<Dictionary<string, PluginHealthStatus>> CheckPluginsHealthAsync();
    }
}