using Backend.Models;

namespace Backend.Dtos.UserDtos;

public class UploadExcelUserDtos
{
    public string? Title { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public bool AcceptTerms { get; set; }
    public Role? Role { get; set; }
    public DateTime? Verified { get; set; }

    public DateTime Created { get; set; }
        
    public string? Address { get; set; }
    public string? CCID { get; set; }
    public string? Phone { get; set; }
    public string? Position { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public bool? Sex { get; set; }
    public string? ManagementCode { get; set; }
}