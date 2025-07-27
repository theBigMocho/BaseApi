using BaseApi.Plugins.Abstractions;

namespace BaseApi.Services
{
    /// <summary>
    /// Interfaz para el servicio de chat que coordina plugins
    /// </summary>
    public interface IChatService
    {
        /// <summary>
        /// Procesa un mensaje de chat usando plugins disponibles
        /// </summary>
        /// <param name="message">Mensaje del usuario</param>
        /// <param name="username">Usuario que envía el mensaje</param>
        /// <returns>Respuesta del chat</returns>
        Task<ChatResponse> ProcessMessageAsync(string message, string username);
        
        /// <summary>
        /// Obtiene sugerencias de comandos
        /// </summary>
        /// <param name="input">Input parcial del usuario</param>
        /// <returns>Lista de sugerencias</returns>
        Task<IEnumerable<string>> GetSuggestionsAsync(string input);
        
        /// <summary>
        /// Obtiene ayuda de todos los plugins disponibles
        /// </summary>
        /// <returns>Información de ayuda consolidada</returns>
        Task<ChatHelp> GetHelpAsync();
        
        /// <summary>
        /// Verifica el estado de los plugins de chat
        /// </summary>
        /// <returns>Estado de los plugins</returns>
        Task<ChatStatus> GetStatusAsync();
    }
    
    /// <summary>
    /// Estado del chat y sus plugins
    /// </summary>
    public class ChatStatus
    {
        public bool IsAvailable { get; set; }
        public int AvailablePlugins { get; set; }
        public int TotalPlugins { get; set; }
        public Dictionary<string, PluginHealthStatus> PluginHealth { get; set; } = new();
        public string? PreferredPlugin { get; set; }
    }
}