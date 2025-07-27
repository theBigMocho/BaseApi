using BaseApi.Plugins.Abstractions;
using System.Collections.Concurrent;

namespace BaseApi.Plugins.Core
{
    /// <summary>
    /// Implementación del gestor de plugins
    /// </summary>
    public class PluginManager : IPluginManager, IDisposable
    {
        private readonly ConcurrentDictionary<string, IPlugin> _plugins = new();
        private readonly ConcurrentDictionary<Type, List<IPlugin>> _pluginsByType = new();
        private readonly ILogger<PluginManager> _logger;
        private bool _disposed = false;

        public PluginManager(ILogger<PluginManager> logger)
        {
            _logger = logger;
        }

        public void RegisterPlugin<T>(T plugin) where T : class, IPlugin
        {
            if (plugin == null)
                throw new ArgumentNullException(nameof(plugin));

            var pluginName = plugin.Name;
            
            if (_plugins.ContainsKey(pluginName))
            {
                _logger.LogWarning("Plugin {PluginName} ya está registrado, reemplazando...", pluginName);
            }

            _plugins[pluginName] = plugin;
            
            // Agregar a índice por tipo
            var pluginType = typeof(T);
            _pluginsByType.AddOrUpdate(pluginType, 
                new List<IPlugin> { plugin },
                (key, list) => 
                {
                    list.RemoveAll(p => p.Name == pluginName); // Remover duplicados
                    list.Add(plugin);
                    return list;
                });

            // También indexar por interfaces que implementa
            foreach (var interfaceType in pluginType.GetInterfaces().Where(i => typeof(IPlugin).IsAssignableFrom(i)))
            {
                _pluginsByType.AddOrUpdate(interfaceType, 
                    new List<IPlugin> { plugin },
                    (key, list) => 
                    {
                        list.RemoveAll(p => p.Name == pluginName);
                        list.Add(plugin);
                        return list;
                    });
            }

            _logger.LogInformation("Plugin registrado: {PluginName} v{Version} by {Author}", 
                plugin.Name, plugin.Version, plugin.Author);
        }

        public T? GetPlugin<T>() where T : class, IPlugin
        {
            var pluginType = typeof(T);
            
            if (_pluginsByType.TryGetValue(pluginType, out var plugins))
            {
                return plugins.FirstOrDefault(p => p.IsEnabled) as T;
            }
            
            return null;
        }

        public IEnumerable<T> GetPlugins<T>() where T : class, IPlugin
        {
            var pluginType = typeof(T);
            
            if (_pluginsByType.TryGetValue(pluginType, out var plugins))
            {
                return plugins.Where(p => p.IsEnabled).Cast<T>();
            }
            
            return Enumerable.Empty<T>();
        }

        public IEnumerable<IPlugin> GetAllPlugins()
        {
            return _plugins.Values.Where(p => p.IsEnabled);
        }

        public async Task EnablePluginAsync(string pluginName)
        {
            if (_plugins.TryGetValue(pluginName, out var plugin))
            {
                if (!plugin.IsEnabled)
                {
                    try
                    {
                        await plugin.InitializeAsync();
                        plugin.IsEnabled = true;
                        _logger.LogInformation("Plugin habilitado: {PluginName}", pluginName);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error habilitando plugin {PluginName}", pluginName);
                        throw;
                    }
                }
            }
            else
            {
                throw new InvalidOperationException($"Plugin {pluginName} no encontrado");
            }
        }

        public async Task DisablePluginAsync(string pluginName)
        {
            if (_plugins.TryGetValue(pluginName, out var plugin))
            {
                if (plugin.IsEnabled)
                {
                    try
                    {
                        await plugin.DisposeAsync();
                        plugin.IsEnabled = false;
                        _logger.LogInformation("Plugin deshabilitado: {PluginName}", pluginName);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error deshabilitando plugin {PluginName}", pluginName);
                        throw;
                    }
                }
            }
            else
            {
                throw new InvalidOperationException($"Plugin {pluginName} no encontrado");
            }
        }

        public async Task InitializePluginsAsync()
        {
            _logger.LogInformation("Inicializando {Count} plugins...", _plugins.Count);
            
            var initializationTasks = _plugins.Values
                .Where(p => p.IsEnabled)
                .Select(async plugin =>
                {
                    try
                    {
                        await plugin.InitializeAsync();
                        _logger.LogDebug("Plugin inicializado: {PluginName}", plugin.Name);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error inicializando plugin {PluginName}", plugin.Name);
                        plugin.IsEnabled = false; // Deshabilitar plugin con errores
                    }
                });

            await Task.WhenAll(initializationTasks);
            
            var enabledCount = _plugins.Values.Count(p => p.IsEnabled);
            _logger.LogInformation("Plugins inicializados: {EnabledCount}/{TotalCount}", enabledCount, _plugins.Count);
        }

        public async Task DisposePluginsAsync()
        {
            _logger.LogInformation("Finalizando plugins...");
            
            var disposalTasks = _plugins.Values
                .Where(p => p.IsEnabled)
                .Select(async plugin =>
                {
                    try
                    {
                        await plugin.DisposeAsync();
                        _logger.LogDebug("Plugin finalizado: {PluginName}", plugin.Name);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error finalizando plugin {PluginName}", plugin.Name);
                    }
                });

            await Task.WhenAll(disposalTasks);
            _logger.LogInformation("Todos los plugins han sido finalizados");
        }

        public async Task<Dictionary<string, PluginHealthStatus>> CheckPluginsHealthAsync()
        {
            var healthChecks = new Dictionary<string, PluginHealthStatus>();
            
            var healthTasks = _plugins.Values
                .Where(p => p.IsEnabled)
                .Select(async plugin =>
                {
                    try
                    {
                        var health = await plugin.CheckHealthAsync();
                        return new { plugin.Name, Health = health };
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error verificando salud del plugin {PluginName}", plugin.Name);
                        return new { plugin.Name, Health = PluginHealthStatus.Unhealthy };
                    }
                });

            var results = await Task.WhenAll(healthTasks);
            
            foreach (var result in results)
            {
                healthChecks[result.Name] = result.Health;
            }

            return healthChecks;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                try
                {
                    DisposePluginsAsync().GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error disposing plugin manager");
                }
                
                _disposed = true;
            }
        }
    }
}