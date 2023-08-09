using Backend.Services.IServices;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace Backend.Services;

public class ImageServices : IImageServices
{
    public ImageUploadResult? UploadFile(Stream fileStream, string fileName)
    {
        Account account = new Account(
            AppSettings.Cloud,
            AppSettings.ApiKey,
            AppSettings.ApiSecretKey);

        Cloudinary cloudinary = new Cloudinary(account);

        var uploadParams = new ImageUploadParams(){
            File = new FileDescription(fileName, fileStream),
        };
        var uploadResult = cloudinary.Upload(uploadParams);
        return uploadResult;
    }
}