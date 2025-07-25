using System.ComponentModel.DataAnnotations;

namespace BaseApi.DTOs
{
    public class UpdateUserDto
    {
        [StringLength(100, ErrorMessage = "El nombre de usuario no puede exceder 100 caracteres")]
        public string? Username { get; set; }

        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [StringLength(255, ErrorMessage = "El email no puede exceder 255 caracteres")]
        public string? Email { get; set; }

        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string? FirstName { get; set; }

        [StringLength(100, ErrorMessage = "El apellido no puede exceder 100 caracteres")]
        public string? LastName { get; set; }

        public bool? IsActive { get; set; }
    }
}