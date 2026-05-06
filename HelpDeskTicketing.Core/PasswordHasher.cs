using System.Security.Cryptography;
using System.Text;

namespace HelpDeskTicketing;

public static class PasswordHasher
{
    public static string GenerateHash(string password, DateTime dateTime)
    {
        var salt =   dateTime.ToLongTimeString();
        var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password + salt));
        return Encoding.UTF8.GetString(hashedBytes);
    }
}