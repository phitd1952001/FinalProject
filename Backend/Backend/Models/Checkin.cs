using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

public class Checkin
{
    [Key]
    public int Id { get; set; }

    public DateTime DateTime { get; set; }

    public bool IsAccept { get; set; }

    public string Note { get; set; }
    
    [Required]
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public Account Account { get; set; }
    
    [Required]
    public int SubjectId { get; set; }
    [ForeignKey("SubjectId")]
    public Subject Subject { get; set; }
    
    [Required]
    public int RoomId { get; set; }
    [ForeignKey("RoomId")]
    public Room Room { get; set; }
}