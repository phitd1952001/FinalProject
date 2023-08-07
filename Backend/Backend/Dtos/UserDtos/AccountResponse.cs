using Backend.Models;

namespace Backend.Dtos.UserDtos;

public class AccountResponse
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public Role Role { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
    public bool IsVerified { get; set; }
    
    public string? Address { get; set; }
    public string? CCID { get; set; }
    public string? Phone { get; set; }
    public string? Position { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public bool Sex { get; set; }
    public string? ManagementCode { get; set; }
    public string? Avatar { get; set; }
}