using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class Subject
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string SubjectCode { get; set; }
    [Required]
    public string Name { get; set; }

    public int Credit { get; set; }

    public int Duration { get; set; }

    public string Description { get; set; }
}