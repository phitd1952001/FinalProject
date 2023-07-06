using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos.UserDtos
{
    public class AuthenticateRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}