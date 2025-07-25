using BaseApi.DTOs;
using BaseApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Obtiene todos los usuarios
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        /// <summary>
        /// Obtiene un usuario por ID
        /// </summary>
        /// <param name="id">ID del usuario</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            
            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            return Ok(user);
        }

        /// <summary>
        /// Crea un nuevo usuario
        /// </summary>
        /// <param name="createUserDto">Datos del usuario a crear</param>
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.CreateUserAsync(createUserDto);
            
            if (user == null)
            {
                return BadRequest(new { message = "El usuario o email ya existe" });
            }

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        /// <summary>
        /// Actualiza un usuario existente
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <param name="updateUserDto">Datos a actualizar</param>
        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.UpdateUserAsync(id, updateUserDto);
            
            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado o datos duplicados" });
            }

            return Ok(user);
        }

        /// <summary>
        /// Desactiva un usuario (dar de baja)
        /// </summary>
        /// <param name="id">ID del usuario</param>
        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            var success = await _userService.DeactivateUserAsync(id);
            
            if (!success)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            return Ok(new { message = "Usuario desactivado correctamente" });
        }

        /// <summary>
        /// Activa un usuario (dar de alta)
        /// </summary>
        /// <param name="id">ID del usuario</param>
        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> ActivateUser(int id)
        {
            var success = await _userService.ActivateUserAsync(id);
            
            if (!success)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            return Ok(new { message = "Usuario activado correctamente" });
        }

        /// <summary>
        /// Elimina un usuario permanentemente
        /// </summary>
        /// <param name="id">ID del usuario</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var success = await _userService.DeleteUserAsync(id);
            
            if (!success)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            return Ok(new { message = "Usuario eliminado correctamente" });
        }
    }
}