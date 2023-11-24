using Backend.Dtos.UserDtos;
using Backend.Models;

namespace Backend.Services.IServices
{
    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress);
        AuthenticateResponse RefreshToken(string token, string ipAddress);
        void RevokeToken(string token, string ipAddress);
        void Register(RegisterRequest model, string origin);
        void VerifyEmail(string token);
        void ForgotPassword(ForgotPasswordRequest model, string origin);
        void ValidateResetToken(ValidateResetTokenRequest model);
        void ResetPassword(ResetPasswordRequest model);
        IEnumerable<AccountResponse> GetAll();
        AccountResponse GetById(int id);
        AccountResponse Create(CreateRequest model);
        AccountResponse Update(int id, UpdateRequest model, Role role);
        AccountResponse UpdateSelf(int id, UpdateSelfRequest model);
        bool AdminCheck();
        void Delete(int id);
        Task<string> UpLoadAvatar(int id, Stream avatar);
        List<string> GetFields();
        Task<List<Dictionary<string, string>>> UploadExcel(IFormFile file);
        Task<IList<AccountResponse>> ImportExcel(IFormFile file, string mapping);
        IEnumerable<AccountResponse> LoadStaff();
        IEnumerable<AccountResponse> SearchStaff(string keyword);
    }
}