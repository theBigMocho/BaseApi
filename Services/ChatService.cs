using BaseApi.Plugins.Abstractions;
using BaseApi.Plugins.Core;

namespace BaseApi.Services
{
    /// <summary>
    /// Servicio de chat que coordina múltiples plugins
    /// </summary>
    public class ChatService : IChatService
    {
        private readonly IPluginManager _pluginManager;
        private readonly ILogger<ChatService> _logger;

        public ChatService(IPluginManager pluginManager, ILogger<ChatService> logger)
        {
            _pluginManager = pluginManager;
            _logger = logger;
        }

        public async Task<ChatResponse> ProcessMessageAsync(string message, string username)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return new ChatResponse
                {
                    Message = "❌ El mensaje no puede estar vacío",
                    Type = ChatResponseType.Error,
                    IsSuccess = false
                };
            }

            var context = new ChatContext
            {
                Username = username,
                SessionId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow
            };

            _logger.LogInformation("Procesando mensaje de chat para {Username}: {Message}", username, message);

            // Obtener plugins de chat disponibles
            var chatPlugins = _pluginManager.GetPlugins<IChatPlugin>().ToList();
            
            if (!chatPlugins.Any())
            {
                return new ChatResponse
                {
                    Message = "❌ No hay plugins de chat disponibles",
                    Type = ChatResponseType.Error,
                    IsSuccess = false
                };
            }

            // Buscar un plugin que pueda manejar el mensaje
            var plugin = chatPlugins.FirstOrDefault(p => p.CanHandle(message));
            
            if (plugin == null)
            {
                return new ChatResponse
                {
                    Message = "❓ No se encontró un plugin que pueda manejar este tipo de mensaje",
                    Type = ChatResponseType.Error,
                    IsSuccess = false
                };
            }

            try
            {
                _logger.LogDebug("Usando plugin {PluginName} para procesar mensaje", plugin.Name);
                var response = await plugin.ProcessMessageAsync(message, context);
                
                _logger.LogInformation("Mensaje procesado exitosamente por {PluginName} en {ProcessingTime}ms", 
                    plugin.Name, response.ProcessingTime.TotalMilliseconds);
                
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando mensaje con plugin {PluginName}", plugin.Name);
                
                return new ChatResponse
                {
                    Message = $"❌ Error interno procesando mensaje: {ex.Message}",
                    Type = ChatResponseType.Error,
                    IsSuccess = false
                };
            }
        }

        public async Task<IEnumerable<string>> GetSuggestionsAsync(string input)
        {
            var allSuggestions = new List<string>();
            var chatPlugins = _pluginManager.GetPlugins<IChatPlugin>();

            foreach (var plugin in chatPlugins)
            {
                try
                {
                    var suggestions = await plugin.GetSuggestionsAsync(input);
                    allSuggestions.AddRange(suggestions);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error obteniendo sugerencias del plugin {PluginName}", plugin.Name);
                }
            }

            // Remover duplicados y limitar resultados
            return allSuggestions.Distinct().Take(10);
        }

        public async Task<ChatHelp> GetHelpAsync()
        {
            var consolidatedHelp = new ChatHelp();
            var allCommands = new List<string>();
            var allExamples = new List<string>();
            var allTips = new List<string>();
            var allShortcuts = new Dictionary<string, string>();

            var chatPlugins = _pluginManager.GetPlugins<IChatPlugin>();

            foreach (var plugin in chatPlugins)
            {
                try
                {
                    var help = plugin.GetHelp();
                    
                    // Agregar prefijo del plugin para identificar origen
                    allCommands.AddRange(help.Commands.Select(cmd => $"[{plugin.Name}] {cmd}"));
                    allExamples.AddRange(help.Examples);
                    allTips.AddRange(help.Tips);
                    
                    foreach (var shortcut in help.Shortcuts)
                    {
                        allShortcuts[$"[{plugin.Name}] {shortcut.Key}"] = shortcut.Value;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error obteniendo ayuda del plugin {PluginName}", plugin.Name);
                }
            }

            consolidatedHelp.Commands = allCommands.ToArray();
            consolidatedHelp.Examples = allExamples.ToArray();
            consolidatedHelp.Tips = allTips.ToArray();
            consolidatedHelp.Shortcuts = allShortcuts;

            await Task.CompletedTask;
            return consolidatedHelp;
        }

        public async Task<ChatStatus> GetStatusAsync()
        {
            var chatPlugins = _pluginManager.GetPlugins<IChatPlugin>().ToList();
            var allPlugins = _pluginManager.GetAllPlugins().Count();
            
            var pluginHealth = await _pluginManager.CheckPluginsHealthAsync();
            
            // Filtrar solo plugins de chat
            var chatPluginHealth = pluginHealth
                .Where(kvp => chatPlugins.Any(p => p.Name == kvp.Key))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var availablePlugins = chatPluginHealth.Count(kvp => kvp.Value == PluginHealthStatus.Healthy);
            var preferredPlugin = chatPlugins.FirstOrDefault()?.Name;

            return new ChatStatus
            {
                IsAvailable = availablePlugins > 0,
                AvailablePlugins = availablePlugins,
                TotalPlugins = chatPlugins.Count,
                PluginHealth = chatPluginHealth,
                PreferredPlugin = preferredPlugin
            };
        }
    }
}