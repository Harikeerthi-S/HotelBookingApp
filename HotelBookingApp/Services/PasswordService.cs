using FirstAPI.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace FirstAPI.Services
{
    public class PasswordService : IPasswordService
    {
            public byte[] HashPassword(string password)
            {
                if (string.IsNullOrWhiteSpace(password))
                    throw new ArgumentNullException(nameof(password));

                using var sha = System.Security.Cryptography.SHA256.Create();
                return sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

            public bool VerifyPassword(string password, byte[] storedHash)
            {
                var hash = HashPassword(password);
                return hash.SequenceEqual(storedHash);
            }
        
    }
}