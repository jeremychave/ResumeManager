using System.Security.Cryptography;
using System.Text;

namespace ResumeManagerWebApi.Common
{
    public static class HmacHelper
    {
        public static string GenerateSignature(string userEmail, string secret)
        {
            var key = Encoding.UTF8.GetBytes(secret);
            var message = Encoding.UTF8.GetBytes(userEmail);

            using (var hmac = new HMACSHA256(key))
            {
                var hash = hmac.ComputeHash(message);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
