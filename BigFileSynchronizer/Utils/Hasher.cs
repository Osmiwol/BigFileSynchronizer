using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BigFileSynchronizer.Utils
{
    public static class Hasher
    {
        public static string ComputeSHA256(string filePath)
        {
            using var sha256 = SHA256.Create();
            using var stream = File.OpenRead(filePath);
            byte[] hashBytes = sha256.ComputeHash(stream);
            return BytesToHex(hashBytes);
        }

        private static string BytesToHex(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (var b in bytes)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}
