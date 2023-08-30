using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos.Subject
{
    public class CreateSubjectRequest
    {
        [Required]
        public string SubjectCode { get; set; }

        [Required]
        public string Name { get; set; }
        
        public string Description { get; set; }
    }
}