using System.ComponentModel.DataAnnotations;
using Backend.Helpers;
using Backend.Models;
using Newtonsoft.Json;

namespace Backend.Dtos.UserDtos
{
    public class UpdateSelfRequest
    {
        private string? _password;
        private string? _confirmPassword;

        public string? Address { get; set; }
        public string? Phone { get; set; }

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