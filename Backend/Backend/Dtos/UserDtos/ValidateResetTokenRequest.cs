using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos.UserDtos;

public class ValidateResetTokenRequest
{
    [Required]
    public string Token { get; set; }
}