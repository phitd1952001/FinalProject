using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos.UserDtos;

public class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}