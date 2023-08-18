using System.ComponentModel.DataAnnotations;
using Backend.Helpers;
using Backend.Models;
using Newtonsoft.Json;

namespace Backend.Dtos.UserDtos
{
    public class UpdateRequest
    {
        private string? _password;
        private string? _confirmPassword;
        public Role? Role;
        private string? _email;
    
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        
        public string? Address { get; set; }
        public string? CCID { get; set; }
        public string? Phone { get; set; }
        public string? Position { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool _sex { get; set; }
        public string? ManagementCode { get; set; }
        public string? Avatar { get; set; }

        [EmailAddress]
        public string? Email
        {
            get => _email;
            set => _email = ReplaceEmptyWithNull(value);
        }
        
        public string? Sex
        {
            get => Convert.ToString(_sex);
            set => _sex = Convert.ToBoolean(value);
        }
        
        [JsonConverter(typeof(OptionalFieldConverter<string>))]
        [MinLength(6)]
        public string? Password
        {
            get => _password;
            set => _password = ReplaceEmptyWithNull(value);
        }
        
        [JsonConverter(typeof(OptionalFieldConverter<string>))]
        [Compare("Password")]
        public string? ConfirmPassword 
        {
            get => _confirmPassword;
            set => _confirmPassword = ReplaceEmptyWithNull(value);
        }

        // helpers

        private string? ReplaceEmptyWithNull(string? value)
        {
            // replace empty string with null to make field optional
            return String.IsNullOrEmpty(value) ? null : value;
        }
    }
}