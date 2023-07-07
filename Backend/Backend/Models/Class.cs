using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

public class Class
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public Account Account { get; set; }
    
    [Required]
    public int SubjectId { get; set; }
    [ForeignKey("SubjectId")]
    public Subject Subject { get; set; }
}