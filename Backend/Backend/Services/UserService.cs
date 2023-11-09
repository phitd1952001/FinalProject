using System.Data;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Backend.DbContext;
using Backend.Dtos.UserDtos;
using Backend.Extensions;
using Backend.Models;
using Backend.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Net.Codecrete.QrCodeGenerator;
using OfficeOpenXml;
using Role = Backend.Models.Role;

namespace Backend.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtUtils _jwtUtils;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IImageServices _imageServices;

        public UserService(
            ApplicationDbContext context,
            IJwtUtils jwtUtils,
            IMapper mapper,
            IEmailService emailService,
            IImageServices imageServices)
        {
            _context = context;
            _jwtUtils = jwtUtils;
            _mapper = mapper;
            _emailService = emailService;
            _imageServices = imageServices;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress)
        {
            var account = _context.Accounts.SingleOrDefault(x => x.Email == model.Email);

            // validate
            if (account == null || !account.IsVerified ||
                !BCrypt.Net.BCrypt.Verify(model.Password, account.PasswordHash))
                throw new AppException("Email or password is incorrect");

            // authentication successful so generate jwt and refresh tokens
            var jwtToken = _jwtUtils.GenerateJwtToken(account);
            var refreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
            account.RefreshTokens.Add(refreshToken);

            // remove old refresh tokens from account
            removeOldRefreshTokens(account);

            // save changes to db
            _context.Update(account);
            _context.SaveChanges();

            var response = _mapper.Map<AuthenticateResponse>(account);
            response.JwtToken = jwtToken;
            response.RefreshToken = refreshToken.Token;
            return response;
        }

        public AuthenticateResponse RefreshToken(string token, string ipAddress)
        {
            var account = getAccountByRefreshToken(token);
            var refreshToken = account.RefreshTokens.Single(x => x.Token == token);

            if (refreshToken.IsRevoked)
            {
                // revoke all descendant tokens in case this token has been compromised
                revokeDescendantRefreshTokens(refreshToken, account, ipAddress,
                    $"Attempted reuse of revoked ancestor token: {token}");
                _context.Update(account);
                _context.SaveChanges();
            }

            if (!refreshToken.IsActive)
                throw new AppException("Invalid token");

            // replace old refresh token with a new one (rotate token)
            var newRefreshToken = rotateRefreshToken(refreshToken, ipAddress);
            account.RefreshTokens.Add(newRefreshToken);


            // remove old refresh tokens from account
            removeOldRefreshTokens(account);

            // save changes to db
            _context.Update(account);
            _context.SaveChanges();

            // generate new jwt
            var jwtToken = _jwtUtils.GenerateJwtToken(account);

            // return data in authenticate response object
            var response = _mapper.Map<AuthenticateResponse>(account);
            response.JwtToken = jwtToken;
            response.RefreshToken = newRefreshToken.Token;
            return response;
        }

        public void RevokeToken(string token, string ipAddress)
        {
            var account = getAccountByRefreshToken(token);
            var refreshToken = account.RefreshTokens.Single(x => x.Token == token);

            if (!refreshToken.IsActive)
                throw new AppException("Invalid token");

            // revoke token and save
            revokeRefreshToken(refreshToken, ipAddress, "Revoked without replacement");
            _context.Update(account);
            _context.SaveChanges();
        }

        public void Register(RegisterRequest model, string origin)
        {
            // validate
            if (_context.Accounts.Any(x => x.Email == model.Email))
            {
                // send already registered error in email to prevent account enumeration
                sendAlreadyRegisteredEmail(model.Email, origin);
                return;
            }

            // map model to new account object
            var account = _mapper.Map<Account>(model);

            // first registered account is an admin
            account.Role = Role.User;
            account.Created = DateTime.UtcNow;
            account.VerificationToken = generateVerificationToken();

            // hash password
            account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // save account
            _context.Accounts.Add(account);
            _context.SaveChanges();

            // send email
            sendVerificationEmail(account, origin);
        }

        public void VerifyEmail(string token)
        {
            var account = _context.Accounts.SingleOrDefault(x => x.VerificationToken == token);

            if (account == null)
                throw new AppException("Verification failed");

            account.Verified = DateTime.UtcNow;
            account.VerificationToken = null;

            _context.Accounts.Update(account);
            _context.SaveChanges();
        }

        public void ForgotPassword(ForgotPasswordRequest model, string origin)
        {
            var account = _context.Accounts.SingleOrDefault(x => x.Email == model.Email);

            // always return ok response to prevent email enumeration
            if (account == null) return;

            // create reset token that expires after 1 day
            account.ResetToken = generateResetToken();
            account.ResetTokenExpires = DateTime.UtcNow.AddDays(1);

            _context.Accounts.Update(account);
            _context.SaveChanges();

            // send email
            sendPasswordResetEmail(account, origin);
        }

        public void ValidateResetToken(ValidateResetTokenRequest model)
        {
            getAccountByResetToken(model.Token);
        }

        public void ResetPassword(ResetPasswordRequest model)
        {
            var account = getAccountByResetToken(model.Token);

            // update password and remove reset token
            account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
            account.PasswordReset = DateTime.UtcNow;
            account.ResetToken = null;
            account.ResetTokenExpires = null;

            _context.Accounts.Update(account);
            _context.SaveChanges();
        }

        public IEnumerable<AccountResponse> GetAll()
        {
            var accounts = _context.Accounts;
            return _mapper.Map<IList<AccountResponse>>(accounts);
        }

        public AccountResponse GetById(int id)
        {
            var account = getAccount(id);
            return _mapper.Map<AccountResponse>(account);
        }

        public AccountResponse Create(CreateRequest model)
        {
            // validate
            if (_context.Accounts.Any(x => x.Email == model.Email))
                throw new AppException($"Email '{model.Email}' is already registered");

            // map model to new account object
            var account = _mapper.Map<Account>(model);
            account.Sex = true;
            account.AcceptTerms = true;
            account.Created = DateTime.UtcNow;
            account.Verified = DateTime.UtcNow;

            // hash password
            account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // save account
            _context.Accounts.Add(account);
            _context.SaveChanges();

            return _mapper.Map<AccountResponse>(account);
        }

        public AccountResponse Update(int id, UpdateRequest model)
        {
            var account = getAccount(id);

            // validate
            if (account.Email != model.Email && _context.Accounts.Any(x => x.Email == model.Email))
                throw new AppException($"Email '{model.Email}' is already registered");

            // hash password if it was entered
            if (!string.IsNullOrEmpty(model.Password))
                account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // make a copy of management code
            var managementCodeCopy = model.ManagementCode;
            if (!string.IsNullOrEmpty(managementCodeCopy)
                && account.ManagementCode != managementCodeCopy
                && !_context.Accounts.Any(_ => _.ManagementCode == model.ManagementCode && _.Id != id))
            {
                var qrCodeImg = GenerateQrCode(managementCodeCopy);
                if (!String.IsNullOrEmpty(account.QrCode))
                {
                    var isDeleteSuccessfully = _imageServices.DeleteFile(account.QrCodePublishId);
                    if (isDeleteSuccessfully)
                    {
                        account.QrCode = String.Empty;
                        account.QrCodePublishId = String.Empty;
                    }
                    else
                    {
                        return _mapper.Map<AccountResponse>(account);
                    }
                }

                // Create a MemoryStream from the byte array
                using (MemoryStream stream = new MemoryStream(qrCodeImg))
                {
                    var uploadResult = _imageServices.UploadFile(stream, Guid.NewGuid().ToString());
                    account.QrCode = uploadResult?.Url.ToString();
                    account.QrCodePublishId = uploadResult?.PublicId;
                }
            }

            // copy model to account and save
            _mapper.Map(model, account);
            account.Sex = model._sex;
            account.Updated = DateTime.UtcNow;
            _context.Accounts.Update(account);
            _context.SaveChanges();

            if(account.QrCode != null)
                SendMailNotiQrCodeCreated(account.Email, account.QrCode);

            return _mapper.Map<AccountResponse>(account);
        }

        public AccountResponse UpdateSelf(int id, UpdateSelfRequest model)
        {
            var account = getAccount(id);

            // hash password if it was entered
            if (!string.IsNullOrEmpty(model.Password))
                account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // copy model to account and save
            _mapper.Map(model, account);
            account.Updated = DateTime.UtcNow;
            _context.Accounts.Update(account);
            _context.SaveChanges();

            return _mapper.Map<AccountResponse>(account);
        }

        public bool AdminCheck()
        {
            return _context.Accounts.Any(_ => _.Email == "Admin@gmail.com");
        }

        public void Delete(int id)
        {
            var account = getAccount(id);
            _context.Accounts.Remove(account);
            _context.SaveChanges();
        }

        // helper methods

        private Account getAccount(int id)
        {
            var account = _context.Accounts.Find(id);
            if (account == null) throw new KeyNotFoundException("Account not found");
            return account;
        }

        private Account getAccountByRefreshToken(string token)
        {
            var account = _context.Accounts.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));
            if (account == null) throw new AppException("Invalid token");
            return account;
        }

        private Account getAccountByResetToken(string token)
        {
            var account = _context.Accounts.SingleOrDefault(x =>
                x.ResetToken == token && x.ResetTokenExpires > DateTime.UtcNow);
            if (account == null) throw new AppException("Invalid token");
            return account;
        }

        private string generateResetToken()
        {
            // token is a cryptographically strong random sequence of values
            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

            // ensure token is unique by checking against db
            var tokenIsUnique = !_context.Accounts.Any(x => x.ResetToken == token);
            if (!tokenIsUnique)
                return generateResetToken();

            return token;
        }

        private string generateVerificationToken()
        {
            // token is a cryptographically strong random sequence of values
            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

            // ensure token is unique by checking against db
            var tokenIsUnique = !_context.Accounts.Any(x => x.VerificationToken == token);
            if (!tokenIsUnique)
                return generateVerificationToken();

            return token;
        }

        private RefreshToken rotateRefreshToken(RefreshToken refreshToken, string ipAddress)
        {
            var newRefreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
            revokeRefreshToken(refreshToken, ipAddress, "Replaced by new token", newRefreshToken.Token);
            return newRefreshToken;
        }

        private void removeOldRefreshTokens(Account account)
        {
            account.RefreshTokens.RemoveAll(x =>
                !x.IsActive &&
                x.Created.AddDays(AppSettings.RefreshTokenTTL) <= DateTime.UtcNow);
        }

        private void revokeDescendantRefreshTokens(RefreshToken refreshToken, Account account, string ipAddress,
            string reason)
        {
            // recursively traverse the refresh token chain and ensure all descendants are revoked
            if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
            {
                var childToken = account.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken.ReplacedByToken);
                if (childToken.IsActive)
                    revokeRefreshToken(childToken, ipAddress, reason);
                else
                    revokeDescendantRefreshTokens(childToken, account, ipAddress, reason);
            }
        }

        private void revokeRefreshToken(RefreshToken token, string ipAddress, string reason = null,
            string replacedByToken = null)
        {
            token.Revoked = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
            token.ReasonRevoked = reason;
            token.ReplacedByToken = replacedByToken;
        }

        private void sendVerificationEmail(Account account, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))
            {
                // origin exists if request sent from browser single page app (e.g. Angular or React)
                // so send link to verify via single page app
                var verifyUrl = $"{origin}/account/verify-email?token={account.VerificationToken}";
                message = $@"<p>Please click the below link to verify your email address:</p>
                            <p><a href=""{verifyUrl}"">{verifyUrl}</a></p>";
            }
            else
            {
                // origin missing if request sent directly to api (e.g. from Postman)
                // so send instructions to verify directly with api
                message =
                    $@"<p>Please use the below token to verify your email address with the <code>/accounts/verify-email</code> api route:</p>
                            <p><code>{account.VerificationToken}</code></p>";
            }

            _emailService.Send(
                to: account.Email,
                subject: "Sign-up Verification API - Verify Email",
                html: $@"<h4>Verify Email</h4>
                        <p>Thanks for registering!</p>
                        {message}"
            );
        }

        private void sendAlreadyRegisteredEmail(string email, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))
                message =
                    $@"<p>If you don't know your password please visit the <a href=""{origin}/account/forgot-password"">forgot password</a> page.</p>";
            else
                message =
                    "<p>If you don't know your password you can reset it via the <code>/accounts/forgot-password</code> api route.</p>";

            _emailService.Send(
                to: email,
                subject: "Sign-up Verification API - Email Already Registered",
                html: $@"<h4>Email Already Registered</h4>
                        <p>Your email <strong>{email}</strong> is already registered.</p>
                        {message}"
            );
        }

        private void sendPasswordResetEmail(Account account, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))
            {
                var resetUrl = $"{origin}/account/reset-password?token={account.ResetToken}";
                message =
                    $@"<p>Please click the below link to reset your password, the link will be valid for 1 day:</p>
                            <p><a href=""{resetUrl}"">{resetUrl}</a></p>";
            }
            else
            {
                message =
                    $@"<p>Please use the below token to reset your password with the <code>/accounts/reset-password</code> api route:</p>
                            <p><code>{account.ResetToken}</code></p>";
            }

            _emailService.Send(
                to: account.Email,
                subject: "Sign-up Verification API - Reset Password",
                html: $@"<h4>Reset Password Email</h4>
                        {message}"
            );
        }

        public async Task<string> UpLoadAvatar(int id, Stream avatar)
        {
            var user = await _context.Accounts.FindAsync(id);
            if (!String.IsNullOrEmpty(user.Avatar))
            {
                var isDeleteSuccessfully = _imageServices.DeleteFile(user.AvatarPublicId);
                if (isDeleteSuccessfully)
                {
                    user.Avatar = String.Empty;
                    user.AvatarPublicId = String.Empty;
                }
                else
                {
                    return string.Empty;
                }
            }

            var uploadResult = _imageServices.UploadFile(avatar, Guid.NewGuid().ToString());

            user.Avatar = uploadResult?.Url.ToString();
            user.AvatarPublicId = uploadResult?.PublicId;
            
            return user.Avatar;
        }

        private byte[] GenerateQrCode(string managementCode)
        {
            var studentId = Encrypt(managementCode);
            var borderWidth = Math.Clamp(3, 0, 999999);
            var qrCode = QrCode.EncodeText(studentId, QrCode.Ecc.Medium);
            byte[] qrcode = qrCode.ToPng(20, (int)borderWidth);
            return qrcode;
        }

        private void SendMailNotiQrCodeCreated(string mail, string qrCode)
        {
            string message = System.IO.File.ReadAllTextAsync("HtmlEmails/QrCodeNoti.html").GetAwaiter().GetResult();
            message = message.Replace("[[name]]", mail);
            message = message.Replace("[[qrcode]]", qrCode);

            _emailService.Send(mail, "Your QrCode Is Created", message);
        }

        private string Encrypt(string clearText)
        {
            var clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(AppSettings.QrCodeKey,
                    new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }

                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }

            return clearText;
        }

        //Excel
        public List<string> GetFields()
        {
            return new List<string>()
            {
                "Title", "FirstName", "LastName", "Email","Role", "Password", "ConfirmPassword", "Address", "CCID", "Phone",
                "Position", "DateOfBirth", "Sex", "ManagementCode"
            };
        }

        public async Task<List<Dictionary<string, string>>> UploadExcel(IFormFile file)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(file.OpenReadStream()))
            {
                var worksheet = package.Workbook.Worksheets[0];

                // Extract data into a list of dictionaries
                var data = new List<Dictionary<string, string>>();
                for (var rowNumber = 1; rowNumber <= worksheet.Dimension.End.Row; rowNumber++)
                {
                    var row = worksheet.Cells[rowNumber, 1, rowNumber, worksheet.Dimension.End.Column];
                    var rowData = new Dictionary<string, string>();
                    foreach (var cell in row)
                    {
                        rowData[cell.Start.Column.ToString()] = cell.Text;
                    }

                    data.Add(rowData);
                }

                return data;
            }
        }

        public async Task<IList<AccountResponse>> ImportExcel(IFormFile file, string mapping)
        {
            // Load the Excel file using EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(file.OpenReadStream()))
            {
                var worksheet = package.Workbook.Worksheets[0];

                // Extract data into a DataTable
                DataTable dt = new DataTable();
                foreach (var firstRowCell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                {
                    dt.Columns.Add(firstRowCell.Text);
                }

                for (var rowNumber = 2; rowNumber <= worksheet.Dimension.End.Row; rowNumber++)
                {
                    var row = worksheet.Cells[rowNumber, 1, rowNumber, worksheet.Dimension.End.Column];
                    var newRow = dt.NewRow();
                    var count = 0;
                    foreach (var cell in row)
                    {
                        newRow[count] = cell.Text;
                        count++;
                    }

                    dt.Rows.Add(newRow);
                }

                // Deserialize the mapping string into a C# object
                var userMapping =
                    Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(mapping);

                // Map Excel columns to appropriate fields
                var mappedData = new List<UploadExcelUserDtos>();
                foreach (DataRow row in dt.Rows)
                {
                    var item = new UploadExcelUserDtos();
                    item.AcceptTerms = true;
                    item.Created = DateTime.UtcNow;
                    item.Verified = DateTime.UtcNow;

                    foreach (var mappingEntry in userMapping)
                    {
                        var excelColumn = mappingEntry.Value;
                        var dataField = mappingEntry.Key;

                        // Map Excel data to the corresponding field
                        if (dt.Columns.Contains(excelColumn))
                        {
                            var value = row[excelColumn].ToString();

                            var propertyInfo = typeof(UploadExcelUserDtos).GetProperty(dataField);
                            if (propertyInfo?.PropertyType == typeof(int))
                            {
                                int intValue;
                                if (int.TryParse(value, out intValue))
                                {
                                    // Successfully converted to int
                                    propertyInfo.SetValue(item, intValue);
                                }
                                else
                                {
                                    propertyInfo.SetValue(item, -1);
                                }
                            }
                            else if (propertyInfo?.PropertyType == typeof(bool) || propertyInfo?.PropertyType == typeof(bool?))
                            {
                                bool boolValue;
                                if (bool.TryParse(value, out boolValue))
                                {
                                    // Successfully converted to int
                                    propertyInfo.SetValue(item, boolValue);
                                }
                                else
                                {
                                    propertyInfo.SetValue(item, true);
                                }
                            }
                            else if (propertyInfo?.PropertyType == typeof(DateTime) || propertyInfo?.PropertyType == typeof(DateTime?))
                            {
                                DateTime dateValue;
                                if (DateTime.TryParse(value, out dateValue))
                                {
                                    // Successfully converted to int
                                    propertyInfo.SetValue(item, dateValue);
                                }
                                else
                                {
                                    propertyInfo.SetValue(item, DateTime.UtcNow);
                                }
                            }
                            else if (propertyInfo?.PropertyType == typeof(Role) || propertyInfo?.PropertyType == typeof(Role?))
                            {
                                Role role;
                                if (Role.TryParse(value, out role))
                                {
                                    // Successfully converted to int
                                    propertyInfo.SetValue(item, role);
                                }
                                else
                                {
                                    propertyInfo.SetValue(item, Role.User);
                                }
                            }
                            else
                            {
                                propertyInfo?.SetValue(item, value);
                            }
                        }
                    }

                    mappedData.Add(item);
                }

                var accounts = await _context.Accounts.ToListAsync();
                var validData = new List<Account>();
                // Process and validate the data as needed and save
                foreach (var data in mappedData)
                {
                    if (!CheckValidData(data, validData, accounts))
                        continue;

                    var account = _mapper.Map<Account>(data);
                    account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(data.Password);

                    var managementCodeCopy = data.ManagementCode;
                    if (!string.IsNullOrEmpty(managementCodeCopy))
                    {
                        var qrCodeImg = GenerateQrCode(managementCodeCopy);
                        if (!String.IsNullOrEmpty(account.QrCode) && !String.IsNullOrEmpty(account.QrCodePublishId))
                        {
                            var isDeleteSuccessfully = _imageServices.DeleteFile(account.QrCodePublishId);
                            if (isDeleteSuccessfully)
                            {
                                account.QrCode = String.Empty;
                                account.QrCodePublishId = String.Empty;
                            }
                        }

                        // Create a MemoryStream from the byte array
                        using (MemoryStream stream = new MemoryStream(qrCodeImg))
                        {
                            var uploadResult = _imageServices.UploadFile(stream, Guid.NewGuid().ToString());
                            account.QrCode = uploadResult?.Url.ToString();
                            account.QrCodePublishId = uploadResult?.PublicId;

                            if (account.QrCode != null)
                                SendMailNotiQrCodeCreated(account.Email, account.QrCode);
                        }
                    }

                    validData.Add(account);
                }

                await _context.Accounts.AddRangeAsync(validData);
                await _context.SaveChangesAsync();

                return _mapper.Map<List<AccountResponse>>(validData);
            }
        }

        private bool CheckValidData(UploadExcelUserDtos account, List<Account> validData, List<Account> currentAccounts)
        {
            var basicValidation = !string.IsNullOrEmpty(account.Email)
                                  && !string.IsNullOrEmpty(account.Password)
                                  && !string.IsNullOrEmpty(account.ConfirmPassword)
                                  && account.Password == account.ConfirmPassword
                                  && account.Role != null
                                  && account.Sex != null
                                  && !string.IsNullOrEmpty(account.ManagementCode);

            var isEmailInUsed = validData.Any(_ => _.Email.Trim().ToLower() == account.Email.Trim().ToLower())
                                || currentAccounts.Any(_ => _.Email.Trim().ToLower() == account.Email.Trim().ToLower());
            var isManagementCodeInUsed = validData.Any(_ =>
                                             !string.IsNullOrEmpty(_.ManagementCode) &&
                                             !string.IsNullOrEmpty(account.ManagementCode) &&
                                             _.ManagementCode.Trim().ToLower() ==
                                             account.ManagementCode.Trim().ToLower())
                                         || currentAccounts.Any(_ =>
                                             !string.IsNullOrEmpty(_.ManagementCode) &&
                                             !string.IsNullOrEmpty(account.ManagementCode) &&
                                             _.ManagementCode.Trim().ToLower() ==
                                             account.ManagementCode.Trim().ToLower());

            return basicValidation && !isEmailInUsed && !isManagementCodeInUsed;
        }
    }
}