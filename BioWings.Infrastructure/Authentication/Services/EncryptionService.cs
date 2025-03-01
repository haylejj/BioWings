using BioWings.Application.Services;
using BioWings.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace BioWings.Infrastructure.Authentication.Services;

public class EncryptionService : IEncryptionService
{
    private readonly ILogger<EncryptionService> _logger;
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public EncryptionService(ILogger<EncryptionService> logger, IOptions<EncryptionSettings> options)
    {
        _logger = logger;

        var settings = options.Value;

        // Güvenli olmayan anahtar ve IV'yi direkt kullanmak yerine, 
        // bunları güvenli anahtar/IV'ye dönüştürüyoruz
        using var deriveBytes = new Rfc2898DeriveBytes(
            settings.Key,
            Encoding.UTF8.GetBytes("BioWingsSalt"), // Sabit salt değeri
            10000); // Iteration count

        _key = deriveBytes.GetBytes(32); // 256-bit key
        _iv = deriveBytes.GetBytes(16);  // 128-bit IV
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return plainText;

        try
        {
            using Aes aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using MemoryStream memoryStream = new();
            using CryptoStream cryptoStream = new(memoryStream, encryptor, CryptoStreamMode.Write);
            using (StreamWriter streamWriter = new(cryptoStream))
            {
                streamWriter.Write(plainText);
            }

            byte[] encryptedData = memoryStream.ToArray();
            // URL-safe Base64 string'e dönüştür
            return Convert.ToBase64String(encryptedData)
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token şifreleme hatası");
            throw new InvalidOperationException("Token şifreleme hatası", ex);
        }
    }

    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            return cipherText;

        try
        {
            // URL-safe Base64'den standart Base64'e dönüştür
            cipherText = cipherText
                .Replace('-', '+')
                .Replace('_', '/');

            // Padding ekle (gerekirse)
            switch (cipherText.Length % 4)
            {
                case 2: cipherText += "=="; break;
                case 3: cipherText += "="; break;
            }

            byte[] encryptedData = Convert.FromBase64String(cipherText);

            using Aes aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using MemoryStream memoryStream = new(encryptedData);
            using CryptoStream cryptoStream = new(memoryStream, decryptor, CryptoStreamMode.Read);
            using StreamReader streamReader = new(cryptoStream);

            return streamReader.ReadToEnd();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token şifre çözme hatası");
            throw new InvalidOperationException("Token şifre çözme hatası", ex);
        }
    }
}