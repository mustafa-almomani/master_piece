using System.Text;

namespace startupCompany.DTO
{
    public class passwordHasherMethod
    {

        public static void createPasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            using (var h = new System.Security.Cryptography.HMACSHA512())
            {
                salt = h.Key;
                hash = h.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

        }

        public static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            //var hmac = new System.Security.Cryptography.HMACSHA256(storedSalt)
            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(storedHash);
            }
        }
    }
}
