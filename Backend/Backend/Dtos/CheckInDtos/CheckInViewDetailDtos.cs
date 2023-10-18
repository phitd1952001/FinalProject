using Backend.Dtos.UserDtos;

namespace Backend.Dtos.CheckInDtos;

public class CheckInViewDetailDtos
{
    public int Id { get; set; }

    public DateTime DateTime { get; set; }

    public bool IsAccept { get; set; }

    public string Note { get; set; }

    public AccountResponse User { get; set; }
}