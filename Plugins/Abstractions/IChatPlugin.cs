namespace BaseApi.Plugins.Abstractions
{
    /// <summary>
    /// Interfaz específica para plugins de chat
    /// </summary>
    public interface IChatPlugin : IPlugin
    {
        /// <summary>
        /// Procesa un mensaje de chat
        /// </summary>
        /// <param name="message">Mensaje del usuario</param>
        /// <param name="context">Contexto del chat</param>
        /// <returns>Respuesta del chat</returns>
        Task<ChatResponse> ProcessMessageAsync(string message, ChatContext context);
        
        /// <summary>
        /// Obtiene sugerencias de comandos
        /// </summary>
        /// <param name="input">Input parcial del usuario</param>
        /// <returns>Lista de sugerencias</returns>
        Task<IEnumerable<string>> GetSuggestionsAsync(string input);
        
        /// <summary>
        /// Verifica si puede manejar un tipo de mensaje específico
        /// </summary>
        /// <param name="message">Mensaje a verificar</param>
        /// <returns>True si puede manejar el mensaje</returns>
        bool CanHandle(string message);
        
        /// <summary>
        /// Obtiene la ayuda del plugin
        /// </summary>
        /// <returns>Información de ayuda</returns>
        ChatHelp GetHelp();
    }
    
    /// <summary>
    /// Contexto de una conversación de chat
    /// </summary>
    public class ChatContext
    {
        public string Username { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public Dictionary<string, object> Properties { get; set; } = new();
    }
    
    /// <summary>
    /// Respuesta de un chat plugin
    /// </summary>
    public class ChatResponse
    {
        public string Message { get; set; } = string.Empty;
        public ChatResponseType Type { get; set; } = ChatResponseType.Text;
        public TimeSpan ProcessingTime { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string? ErrorCode { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }
    
    /// <summary>
    /// Tipo de respuesta de chat
    /// </summary>
    public enum ChatResponseType
    {
        Text,
        Code,
        Error,
        System,
        File,
        Image
    }
    
    /// <summary>
    /// Información de ayuda del plugin
    /// </summary>
    public class ChatHelp
    {
        public string[] Commands { get; set; } = Array.Empty<string>();
        public string[] Examples { get; set; } = Array.Empty<string>();
        public string[] Tips { get; set; } = Array.Empty<string>();
        public Dictionary<string, string> Shortcuts { get; set; } = new();
    }
}