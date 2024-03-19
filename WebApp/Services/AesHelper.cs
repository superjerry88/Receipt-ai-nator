using System.Security.Cryptography;
using System.Text;

namespace WebApp.Services;

public class AesHelper
{
    public string EncryptString(string plainText)
    {
        var secret = RezApi.Settings.Secret;
        byte[] encrypted;
        using (var aesAlg = Aes.Create())
        {
            aesAlg.Key = CreateKey(secret);
            aesAlg.IV = new byte[16]; // Initialization vector. Set to zero for simplicity.

            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }
            encrypted = msEncrypt.ToArray();
        }

        return Convert.ToBase64String(encrypted);
    }

    public string DecryptString(string cipherText)
    {
        var secret = RezApi.Settings.Secret;
        var cipherBytes = Convert.FromBase64String(cipherText);

        using var aesAlg = Aes.Create();
        aesAlg.Key = CreateKey(secret); ;
        aesAlg.IV = new byte[16]; // Initialization vector. Set to zero for simplicity.

        var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        using var msDecrypt = new MemoryStream(cipherBytes);
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);
        var plaintext = srDecrypt.ReadToEnd();

        return plaintext;
    }

    public string GenerateSecret()
    {
        var secret = new byte[256 / 8];
        RandomNumberGenerator.Create().GetBytes(secret);
        return Convert.ToBase64String(secret);
    }

    private byte[] CreateKey(string secret)
    {
        using var sha256 = SHA256.Create();
        var key = sha256.ComputeHash(Encoding.UTF8.GetBytes(secret));
        return key;
    }
}