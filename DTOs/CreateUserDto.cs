using System.ComponentModel.DataAnnotations;

namespace BaseApi.DTOs
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [StringLength(100, ErrorMessage = "El nombre de usuario no puede exceder 100 caracteres")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [StringLength(255, ErrorMessage = "El email no puede exceder 255 caracteres")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres")]
        public string Password { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string? FirstName { get; set; }

        [StringLength(100, ErrorMessage = "El apellido no puede exceder 100 caracteres")]
        public string? LastName { get; set; }

        public bool IsActive { get; set; } = true;
    }
}