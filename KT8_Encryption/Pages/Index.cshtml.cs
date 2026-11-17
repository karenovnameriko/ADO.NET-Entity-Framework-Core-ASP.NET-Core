
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using System.Text;

namespace KT8_Encryption.Pages;

public class IndexModel : PageModel
{
    [BindProperty]
    public string Algorithm { get; set; } = "AES";

    [BindProperty]
    public string InputText { get; set; } = "";

    [BindProperty]
    public string EncryptedText { get; set; } = "";

    [BindProperty]
    public string Key { get; set; } = "";

    [BindProperty]
    public string PublicKey { get; set; } = "";

    public string? DecryptedText { get; set; }

    public string? Error { get; set; }

    public void OnPostEncrypt()
    {
        try
        {
            Error = null;
            DecryptedText = null;
            EncryptedText = "";

            if (string.IsNullOrWhiteSpace(InputText))
                throw new Exception("Введите исходный текст.");

            if (Algorithm == "AES")
            {
                EncryptAes();
            }
            else if (Algorithm == "RSA")
            {
                EncryptRsa();
            }
            else
            {
                throw new Exception("Выберите AES или RSA.");
            }

            // Обновляем значения полей после генерации ключей.
            ModelState.Remove(nameof(Key));
            ModelState.Remove(nameof(PublicKey));
            ModelState.Remove(nameof(EncryptedText));
        }
        catch (Exception ex)
        {
            Error = "Ошибка шифрования: " + ex.Message;
        }
    }

    private void EncryptAes()
    {
        byte[] key;

        if (string.IsNullOrWhiteSpace(Key))
        {
            // Генерируем случайный ключ AES-256.
            key = RandomNumberGenerator.GetBytes(32);
            Key = Convert.ToBase64String(key);
        }
        else
        {
            key = Convert.FromBase64String(Key);
        }

        if (key.Length != 16 && key.Length != 24 && key.Length != 32)
            throw new Exception("Ключ AES должен содержать 16, 24 или 32 байта.");

        byte[] nonce = RandomNumberGenerator.GetBytes(12);
        byte[] plaintext = Encoding.UTF8.GetBytes(InputText);

        byte[] ciphertext = new byte[plaintext.Length];
        byte[] tag = new byte[16];

        using var aes = new AesGcm(key, 16);

        aes.Encrypt(nonce, plaintext, ciphertext, tag);

        // Формируем последовательность: nonce + tag + ciphertext.
        byte[] result = new byte[nonce.Length + tag.Length + ciphertext.Length];

        Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
        Buffer.BlockCopy(tag, 0, result, nonce.Length, tag.Length);
        Buffer.BlockCopy(ciphertext, 0, result, nonce.Length + tag.Length, ciphertext.Length);

        EncryptedText = Convert.ToBase64String(result);
    }

    private void EncryptRsa()
    {
        using RSA rsa = RSA.Create(2048);

        if (string.IsNullOrWhiteSpace(PublicKey))
        {
            // Если открытый ключ не задан, создаём новую пару ключей.
            Key = rsa.ExportPkcs8PrivateKeyPem();
            PublicKey = rsa.ExportSubjectPublicKeyInfoPem();
        }
        else
        {
            // Используем введённый открытый ключ.
            rsa.ImportFromPem(PublicKey);
        }

        byte[] plaintext = Encoding.UTF8.GetBytes(InputText);

        // RSA-OAEP-SHA256 с ключом 2048 бит поддерживает до 190 байт.
        int maxLength = rsa.KeySize / 8 - 2 * 32 - 2;

        if (plaintext.Length > maxLength)
            throw new Exception($"Для RSA с этим ключом текст не должен превышать {maxLength} байт.");

        byte[] encrypted = rsa.Encrypt(
            plaintext,
            RSAEncryptionPadding.OaepSHA256);

        EncryptedText = Convert.ToBase64String(encrypted);
    }

    public void OnPostDecrypt()
    {
        try
        {
            Error = null;
            DecryptedText = null;

            if (string.IsNullOrWhiteSpace(EncryptedText))
                throw new Exception("Введите зашифрованный текст.");

            if (string.IsNullOrWhiteSpace(Key))
                throw new Exception("Введите ключ для расшифровки.");

            if (Algorithm == "AES")
            {
                DecryptAes();
            }
            else if (Algorithm == "RSA")
            {
                DecryptRsa();
            }
            else
            {
                throw new Exception("Выберите AES или RSA.");
            }
        }
        catch (CryptographicException)
        {
            Error = "Не удалось расшифровать. Проверьте правильность ключа и зашифрованного текста.";
        }
        catch (FormatException)
        {
            Error = "Неверный формат ключа или зашифрованного текста.";
        }
        catch (Exception ex)
        {
            Error = "Ошибка расшифровки: " + ex.Message;
        }
    }

    private void DecryptAes()
    {
        byte[] key = Convert.FromBase64String(Key);
        byte[] data = Convert.FromBase64String(EncryptedText);

        if (key.Length != 16 && key.Length != 24 && key.Length != 32)
            throw new Exception("Некорректная длина ключа AES.");

        if (data.Length < 28)
            throw new Exception("Некорректный зашифрованный текст.");

        byte[] nonce = data[..12];
        byte[] tag = data[12..28];
        byte[] ciphertext = data[28..];

        byte[] plaintext = new byte[ciphertext.Length];

        using var aes = new AesGcm(key, 16);

        aes.Decrypt(nonce, ciphertext, tag, plaintext);

        DecryptedText = Encoding.UTF8.GetString(plaintext);
    }

    private void DecryptRsa()
    {
        using RSA rsa = RSA.Create();

        // Для расшифровки RSA необходим закрытый ключ.
        rsa.ImportFromPem(Key);

        byte[] encrypted = Convert.FromBase64String(EncryptedText);

        byte[] decrypted = rsa.Decrypt(
            encrypted,
            RSAEncryptionPadding.OaepSHA256);

        DecryptedText = Encoding.UTF8.GetString(decrypted);
    }
}
