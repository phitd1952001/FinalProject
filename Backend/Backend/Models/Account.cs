using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Backend.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        public bool AcceptTerms { get; set; }
        public Role Role { get; set; }
        public string? VerificationToken { get; set; }
        public DateTime? Verified { get; set; }
        public bool IsVerified => Verified.HasValue || PasswordReset.HasValue;
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }
        public DateTime? PasswordReset { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; }

        public bool OwnsToken(string token) 
        {
            return this.RefreshTokens?.Find(x => x.Token == token) != null;
        }
        
        public string? Address { get; set; }
        public string? CCID { get; set; }
        public string? Phone { get; set; }
        public string? Position { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool Sex { get; set; }
        public string? ManagementCode { get; set; }
        public string? Avatar { get; set; }
        public string? AvatarPublicId { get; set; }
        public string? QrCode { get; set; }
        public string? QrCodePublishId { get; set; }
    }
}
