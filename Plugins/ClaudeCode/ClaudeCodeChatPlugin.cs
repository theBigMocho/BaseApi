using BaseApi.Plugins.Abstractions;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace BaseApi.Plugins.ClaudeCode
{
    /// <summary>
    /// Plugin de chat para integración con Claude Code CLI
    /// </summary>
    public class ClaudeCodeChatPlugin : IChatPlugin
    {
        private readonly ILogger<ClaudeCodeChatPlugin> _logger;
        private readonly IConfiguration _configuration;
        
        // Comandos permitidos por seguridad
        private readonly HashSet<string> _allowedCommands = new()
        {
            "help", "status", "version", "ls", "dir", "pwd", "cd",
            "dotnet", "git", "npm", "node", "python", "pip",
            "curl", "ping", "echo", "cat", "type", "more", "less"
        };

        // Comandos bloqueados por seguridad
        private readonly HashSet<string> _blockedCommands = new()
        {
            "rm", "del", "rmdir", "rd", "format", "fdisk", "mkfs",
            "kill", "taskkill", "shutdown", "reboot", "halt",
            "chmod", "chown", "passwd", "sudo", "su", "runas"
        };

        public string Name => "ClaudeCodeChat";
        public string Version => "1.0.0";
        public string Description => "Plugin para chat integrado con Claude Code CLI";
        public string Author => "BaseApi Team";
        public bool IsEnabled { get; set; } = false;

        public ClaudeCodeChatPlugin(ILogger<ClaudeCodeChatPlugin> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task InitializeAsync()
        {
            _logger.LogInformation("Inicializando plugin Claude Code Chat...");
            
            // Verificar que Claude Code esté disponible
            var isAvailable = await IsClaudeCodeAvailableAsync();
            if (!isAvailable)
            {
                _logger.LogWarning("Claude Code CLI no está disponible");
                // No fallar la inicialización, pero registrar el problema
            }
            
            _logger.LogInformation("Plugin Claude Code Chat inicializado correctamente");
        }

        public async Task DisposeAsync()
        {
            _logger.LogInformation("Finalizando plugin Claude Code Chat...");
            await Task.CompletedTask;
        }

        public async Task<PluginHealthStatus> CheckHealthAsync()
        {
            try
            {
                var isAvailable = await IsClaudeCodeAvailableAsync();
                return isAvailable ? PluginHealthStatus.Healthy : PluginHealthStatus.Degraded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verificando salud del plugin Claude Code");
                return PluginHealthStatus.Unhealthy;
            }
        }

        public async Task<ChatResponse> ProcessMessageAsync(string message, ChatContext context)
        {
            var startTime = DateTime.UtcNow;
            
            try
            {
                _logger.LogInformation("Procesando mensaje de {Username}: {Message}", context.Username, message);

                string claudeResponse;
                var responseType = ChatResponseType.Text;

                if (IsDirectCommand(message))
                {
                    // Es un comando directo
                    if (!IsCommandSafe(message))
                    {
                        return new ChatResponse
                        {
                            Message = $"❌ Comando no permitido por seguridad: {message}",
                            Type = ChatResponseType.Error,
                            IsSuccess = false,
                            ErrorCode = "FORBIDDEN_COMMAND",
                            ProcessingTime = DateTime.UtcNow - startTime
                        };
                    }

                    claudeResponse = await ExecuteSystemCommandAsync(message);
                    responseType = ChatResponseType.Code;
                }
                else if (message.StartsWith("/"))
                {
                    // Comando especial del plugin
                    claudeResponse = await HandleSpecialCommandAsync(message, context);
                    responseType = ChatResponseType.System;
                }
                else
                {
                    // Pregunta/conversación natural - enviar a Claude Code
                    claudeResponse = await SendToClaudeCodeAsync(message);
                }

                return new ChatResponse
                {
                    Message = claudeResponse,
                    Type = responseType,
                    IsSuccess = true,
                    ProcessingTime = DateTime.UtcNow - startTime,
                    Metadata = new Dictionary<string, object>
                    {
                        ["originalMessage"] = message,
                        ["processingType"] = IsDirectCommand(message) ? "command" : "natural"
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando mensaje de {Username}", context.Username);
                
                return new ChatResponse
                {
                    Message = $"❌ Error procesando mensaje: {ex.Message}",
                    Type = ChatResponseType.Error,
                    IsSuccess = false,
                    ProcessingTime = DateTime.UtcNow - startTime
                };
            }
        }

        public async Task<IEnumerable<string>> GetSuggestionsAsync(string input)
        {
            var suggestions = new List<string>();
            
            if (string.IsNullOrWhiteSpace(input))
            {
                suggestions.AddRange(new[]
                {
                    "/help",
                    "/status",
                    "¿Qué archivos hay en el proyecto?",
                    "dotnet build",
                    "git status",
                    "Explica el código de autenticación"
                });
            }
            else
            {
                var lowerInput = input.ToLowerInvariant();
                
                var allSuggestions = new[]
                {
                    "/help - Mostrar ayuda",
                    "/status - Estado de Claude Code", 
                    "dotnet build - Compilar proyecto",
                    "dotnet run - Ejecutar aplicación",
                    "git status - Estado del repositorio",
                    "¿Qué archivos hay? - Listar archivos",
                    "Explica el código de [archivo] - Analizar código"
                };

                suggestions.AddRange(allSuggestions.Where(s => 
                    s.ToLowerInvariant().Contains(lowerInput)));
            }

            await Task.CompletedTask;
            return suggestions.Take(10);
        }

        public bool CanHandle(string message)
        {
            // Este plugin puede manejar cualquier mensaje
            return true;
        }

        public ChatHelp GetHelp()
        {
            return new ChatHelp
            {
                Commands = new[]
                {
                    "/help - Muestra esta ayuda",
                    "/status - Verifica el estado de Claude Code",
                    "dotnet [comando] - Ejecuta comandos .NET",
                    "git [comando] - Ejecuta comandos Git",
                    "[pregunta natural] - Hace una pregunta a Claude Code"
                },
                Examples = new[]
                {
                    "¿Cómo funciona la autenticación JWT?",
                    "dotnet build",
                    "git status",
                    "Explica el patrón Repository"
                },
                Tips = new[]
                {
                    "Puedes escribir en lenguaje natural",
                    "Los comandos de sistema se ejecutan de forma segura",
                    "Usa /help para ver todos los comandos disponibles"
                }
            };
        }

        #region Private Methods

        private async Task<bool> IsClaudeCodeAvailableAsync()
        {
            try
            {
                var result = await ExecuteClaudeCodeAsync("--version", TimeSpan.FromSeconds(5));
                return result.IsSuccess && !string.IsNullOrEmpty(result.Output);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Claude Code no disponible");
                return false;
            }
        }

        private async Task<string> SendToClaudeCodeAsync(string message)
        {
            try
            {
                var escapedMessage = EscapeMessageForClaude(message);
                var result = await ExecuteClaudeCodeAsync($"\"{escapedMessage}\"", TimeSpan.FromMinutes(2));
                
                if (result.IsSuccess)
                {
                    return result.Output ?? "Claude Code procesó el mensaje pero no retornó respuesta.";
                }
                else
                {
                    return $"❌ Error en Claude Code: {result.Error}";
                }
            }
            catch (TimeoutException)
            {
                return "⏱️ Claude Code está tardando mucho en responder. Intenta con un mensaje más simple.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando mensaje a Claude Code");
                return $"❌ Error de comunicación con Claude Code: {ex.Message}";
            }
        }

        private async Task<(bool IsSuccess, string? Output, string? Error)> ExecuteClaudeCodeAsync(string arguments, TimeSpan timeout)
        {
            using var process = new Process();
            
            bool isWindows = Environment.OSVersion.Platform == PlatformID.Win32NT;
            
            process.StartInfo = new ProcessStartInfo
            {
                FileName = isWindows ? "cmd.exe" : "claude",
                Arguments = isWindows ? $"/c claude {arguments}" : arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = Directory.GetCurrentDirectory()
            };

            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();

            process.OutputDataReceived += (s, e) => 
            {
                if (!string.IsNullOrEmpty(e.Data))
                    outputBuilder.AppendLine(e.Data);
            };

            process.ErrorDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    errorBuilder.AppendLine(e.Data);
            };

            try
            {
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                using var cts = new CancellationTokenSource(timeout);
                
                try
                {
                    await process.WaitForExitAsync(cts.Token);
                }
                catch (OperationCanceledException)
                {
                    try
                    {
                        process.Kill(true);
                    }
                    catch { }
                    throw new TimeoutException($"Claude Code timeout after {timeout.TotalSeconds} seconds");
                }

                var output = outputBuilder.ToString().Trim();
                var error = errorBuilder.ToString().Trim();
                
                return (process.ExitCode == 0, output, error);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ejecutando Claude Code con argumentos: {Arguments}", arguments);
                throw;
            }
        }

        private async Task<string> ExecuteSystemCommandAsync(string command)
        {
            var parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return "❌ Comando vacío";

            var cmd = parts[0].ToLowerInvariant();
            var args = string.Join(" ", parts.Skip(1));

            try
            {
                using var process = new Process();
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = cmd,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = Directory.GetCurrentDirectory()
                };

                process.Start();
                var output = await process.StandardOutput.ReadToEndAsync();
                var error = await process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();

                if (process.ExitCode == 0)
                {
                    return string.IsNullOrEmpty(output) ? "✅ Comando ejecutado exitosamente" : output;
                }
                else
                {
                    return $"❌ Error ejecutando comando:\n{error}";
                }
            }
            catch (Exception ex)
            {
                return $"❌ Error ejecutando comando del sistema: {ex.Message}";
            }
        }

        private async Task<string> HandleSpecialCommandAsync(string command, ChatContext context)
        {
            return command.ToLowerInvariant() switch
            {
                "/help" => FormatHelp(),
                "/status" => await GetStatusAsync(),
                "/version" => await GetVersionAsync(),
                _ => $"❓ Comando especial no reconocido: {command}"
            };
        }

        private string FormatHelp()
        {
            var help = GetHelp();
            var sb = new StringBuilder();
            sb.AppendLine("🤖 **Claude Code Chat Plugin - Ayuda**");
            sb.AppendLine();
            sb.AppendLine("**Comandos Disponibles:**");
            foreach (var cmd in help.Commands)
            {
                sb.AppendLine($"• {cmd}");
            }
            sb.AppendLine();
            sb.AppendLine("**Ejemplos:**");
            foreach (var example in help.Examples)
            {
                sb.AppendLine($"• \"{example}\"");
            }
            sb.AppendLine();
            sb.AppendLine("**Tips:**");
            foreach (var tip in help.Tips)
            {
                sb.AppendLine($"💡 {tip}");
            }
            
            return sb.ToString();
        }

        private async Task<string> GetStatusAsync()
        {
            var isAvailable = await IsClaudeCodeAvailableAsync();
            var version = await GetVersionAsync();
            
            return $@"🔍 **Estado de Claude Code Plugin**

**Disponibilidad:** {(isAvailable ? "✅ Conectado" : "❌ No disponible")}
**Versión:** {version}
**Último check:** {DateTime.Now:HH:mm:ss}
**Directorio:** {Directory.GetCurrentDirectory()}

{(isAvailable ? "🚀 Listo para procesar comandos" : "⚠️ Verifica que Claude Code esté instalado")}";
        }

        private async Task<string> GetVersionAsync()
        {
            try
            {
                var result = await ExecuteClaudeCodeAsync("--version", TimeSpan.FromSeconds(5));
                if (result.IsSuccess && !string.IsNullOrEmpty(result.Output))
                {
                    var match = Regex.Match(result.Output, @"(\d+\.\d+\.\d+)", RegexOptions.IgnoreCase);
                    return match.Success ? match.Groups[1].Value : result.Output.Trim();
                }
                return "No detectada";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo versión de Claude Code");
                return "Error";
            }
        }

        private bool IsDirectCommand(string message)
        {
            var lowerMessage = message.ToLowerInvariant().Trim();
            
            return lowerMessage.StartsWith("dotnet ") ||
                   lowerMessage.StartsWith("git ") ||
                   lowerMessage.StartsWith("npm ") ||
                   lowerMessage.StartsWith("node ") ||
                   _allowedCommands.Any(cmd => lowerMessage.StartsWith(cmd + " "));
        }

        private bool IsCommandSafe(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                return false;

            var normalizedCommand = command.Trim().ToLowerInvariant();
            
            // Verificar comandos bloqueados
            if (_blockedCommands.Any(blocked => normalizedCommand.StartsWith(blocked)))
            {
                return false;
            }

            // Verificar patrones peligrosos
            var dangerousPatterns = new[]
            {
                @">\s*nul", @">\s*/dev/null",
                @"&&", @"\|\|", @";",
                @"\$\(", @"`",
                @"\.\.[/\\]",
                @"[<>]"
            };

            foreach (var pattern in dangerousPatterns)
            {
                if (Regex.IsMatch(normalizedCommand, pattern))
                {
                    return false;
                }
            }

            return true;
        }

        private string EscapeMessageForClaude(string message)
        {
            return message.Replace("\"", "\\\"").Replace("'", "\\'");
        }

        #endregion
    }
}