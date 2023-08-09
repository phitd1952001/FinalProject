using CloudinaryDotNet.Actions;

namespace Backend.Services.IServices;

public interface IImageServices
{
    ImageUploadResult? UploadFile(Stream fileStream, string fileName);
}