namespace Backend.Dtos.ChatDtos;

public class UploadImageRequest
{
    public IFormFile image { get; set; }
    public string id { get; set; }
}