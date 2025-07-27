using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BaseApi.Services;

namespace BaseApi.Controllers
{
    /// <summary>
    /// Controlador para chat basado en sistema de plugins
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PluginChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ILogger<PluginChatController> _logger;

        public PluginChatController(IChatService chatService, ILogger<PluginChatController> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        /// <summary>
        /// Procesa un mensaje de chat usando el sistema de plugins
        /// </summary>
        [HttpPost("message")]
        public async Task<ActionResult> SendMessage([FromBody] ChatMessageRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest("El mensaje no puede estar vacío");
            }

            var username = User.Identity?.Name ?? "Usuario";
            
            try
            {
                var response = await _chatService.ProcessMessageAsync(request.Message, username);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando mensaje de chat");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene el estado del sistema de chat
        /// </summary>
        [HttpGet("status")]
        public async Task<ActionResult> GetStatus()
        {
            try
            {
                var status = await _chatService.GetStatusAsync();
                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo estado del chat");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene sugerencias de comandos
        /// </summary>
        [HttpGet("suggestions")]
        public async Task<ActionResult> GetSuggestions([FromQuery] string? input = null)
        {
            try
            {
                var suggestions = await _chatService.GetSuggestionsAsync(input ?? string.Empty);
                return Ok(suggestions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo sugerencias");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene ayuda consolidada de todos los plugins
        /// </summary>
        [HttpGet("help")]
        public async Task<ActionResult> GetHelp()
        {
            try
            {
                var help = await _chatService.GetHelpAsync();
                return Ok(help);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo ayuda");
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }

    /// <summary>
    /// Modelo para enviar mensajes de chat
    /// </summary>
    public class ChatMessageRequest
    {
        public string Message { get; set; } = string.Empty;
    }
}