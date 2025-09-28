using System.Security.Cryptography;
using System.Text;

public static class SHA256_Generator
{
    public static string ComputeHash(string plain_text)
    {
        using var sha256 = SHA256.Create();
        byte[] hash_bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(plain_text));
        return Convert.ToHexString(hash_bytes);
    }
}