using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos.SettingDtos
{
    public class SettingUpsertDtos
    {
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        [Range(0, 101)]
        public int ConcurrencyLevelDefault { get; set; }
        [Required]
        [Range(0, 101)]
        public int InternalDistance { get; set; }
        [Required]
        [Range(0, 101)]
        public int ExternalDistance { get; set; }
        [Required]
        [Range(0, 101)]
        public int NoOfTimeSlot { get; set; }
    }
}
