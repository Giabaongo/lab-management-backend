using LabManagement.BLL.Interfaces;

namespace LabManagement.BLL.Implementations
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            // Generate a salt and hash the password using BCrypt
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            // Verify the password against the hash
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}