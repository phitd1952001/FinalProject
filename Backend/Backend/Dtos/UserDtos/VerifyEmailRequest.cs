
using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos.UserDtos;

public class VerifyEmailRequest
{
    [Required]
    public string Token { get; set; }
}