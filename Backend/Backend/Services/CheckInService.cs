using System.Security.Cryptography;
using System.Text;
using System.Web;
using AutoMapper;
using Backend.DbContext;
using Backend.Dtos.CheckInDtos;
using Backend.Dtos.UserDtos;
using Backend.Models;
using Backend.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class CheckInService : ICheckInService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CheckInService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CheckInViewDetailDtos>> GetAllCheckIn(int slotId)
    {
        var listCheckIn = await _context.Checkins.Where(_ => _.SlotId == slotId).ToListAsync();
        var userIds = listCheckIn.Select(_ => _.UserId);
        var accounts = await _context.Accounts.Where(_ => userIds.Contains(_.Id)).ToListAsync();

        var result = new List<CheckInViewDetailDtos>();

        foreach (var checkin in listCheckIn)
        {
            var account = accounts.FirstOrDefault(_ => _.Id == checkin.UserId);
            if (account == null)
            {
                continue;
            }
            
            var ck = new CheckInViewDetailDtos()
            {
                Id = checkin.Id,
                DateTime = checkin.DateTime,
                IsAccept = checkin.IsAccept,
                Note = checkin.Note,
                User = _mapper.Map<AccountResponse>(account)
            };
            
            result.Add(ck);
        }
        
        return result;
    }

    public async Task<Object> CheckInConfirm(CheckInConfirmDtos input)
    {
        var studentIdEncode = Decrypt(HttpUtility.UrlDecode(input.QrCodeString));

        var studentInDb = await _context.Accounts.FirstOrDefaultAsync(_ => _.ManagementCode == studentIdEncode);

        if (studentInDb == null)
        {
            throw new AppException("User is not found");
        }

        var isStudentAlreadyCheckin =
            await _context.Checkins.AnyAsync(_ => _.UserId == studentInDb.Id && _.SlotId == input.SlotId);

        if (isStudentAlreadyCheckin)
        {
            throw new AppException("User already checkin");
        }

        await _context.Checkins.AddAsync(new Checkin()
        {
            DateTime = DateTime.Now,
            UserId = studentInDb.Id,
            SlotId = input.SlotId,
            IsAccept = input.IsAccept,
            Note = input.Note
        });

        await _context.SaveChangesAsync();

        return new
        {
            studentInDb.ManagementCode,
            studentInDb.DateOfBirth,
            studentInDb.FirstName,
            studentInDb.LastName,
            studentInDb.Avatar
        };
    }

    public async Task<Object> CheckIn(string qrCodeString)
    {
        var studentInDb =
            await _context.Accounts.FirstOrDefaultAsync(s =>
                s.ManagementCode == Decrypt(HttpUtility.UrlDecode(qrCodeString)));

        if (studentInDb == null)
        {
            throw new AppException("User is not found");
        }

        return new
        {
            studentInDb.ManagementCode,
            studentInDb.DateOfBirth,
            studentInDb.FirstName,
            studentInDb.LastName,
            studentInDb.Avatar
        };
    }
    
    public async Task Delete(int id)
    {
        var checkin = await _context.Checkins
            .FirstOrDefaultAsync(_ => _.Id == id);

        _context.Remove(checkin);
        _context.SaveChanges();
    }

    private string Decrypt(string cipherText)
    {
        try
        {
            string EncryptionKey = AppSettings.QrCodeKey;
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey,
                    new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }

                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }

            return cipherText;
        }
        catch (Exception)
        {
            return "StudentWrong";
        }
    }
}