namespace Backend.Dtos.CheckInDtos;

public class CheckInConfirmDtos
{
    public bool IsAccept { get; set; }
    public string Note { get; set; }
    public string QrCodeString { get; set; }
    public int SlotId { get; set; }
}