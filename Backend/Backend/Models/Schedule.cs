﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

public class Schedule
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string SubjectCode { get; set; }
    
    public int Color { get; set; }
    
    public DateTime Date { get; set; }
    
    public int Slot { get; set; }
}