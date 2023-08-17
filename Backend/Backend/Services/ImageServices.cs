using Backend.Services.IServices;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace Backend.Services;

public class ImageServices : IImageServices
{
    private readonly Cloudinary _cloudinary;

    public ImageServices(Cloudinary cloudinary)
    {
        _cloudinary = cloudinary;
    }
    
    public ImageUploadResult? UploadFile(Stream fileStream, string fileName)
    {
        var uploadParams = new ImageUploadParams(){
            File = new FileDescription(fileName, fileStream),
        };
        
        var uploadResult = _cloudinary.Upload(uploadParams);
        
        return uploadResult;
    }

    public bool DeleteFile(string publicId)
    {
        var deletionParams = new DeletionParams(publicId);
        var deletionResult = _cloudinary.Destroy(deletionParams);
        
        if (deletionResult.Result == "ok")
        {
            return true;
        }

        return false;
    }
}