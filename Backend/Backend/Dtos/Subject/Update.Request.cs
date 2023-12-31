using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos.Subject
{
    public class UpdateSubjectRequest
    {
        [Required]
        public string SubjectCode { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        [Required]
        public int Duration { get; set; }
        [Required]
        public int Credit { get; set; }

        public string Description { get; set; }
    }
}