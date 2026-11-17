using KT11_CryptoLibraries.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KT11_CryptoLibraries.Pages;

public class CryptoModel : PageModel
{
    private readonly CryptoService _crypto;

    public CryptoModel(CryptoService crypto)
    {
        _crypto = crypto;
    }

    // Bouncy Castle

    [BindProperty]
    public string BouncyPlainText { get; set; } = string.Empty;

    [BindProperty]
    public string BouncyCipherText { get; set; } = string.Empty;

    public string? BouncyEncryptedText { get; set; }

    public string? BouncyDecryptedText { get; set; }

    public string? BouncyMessage { get; set; }

    public string? BouncyError { get; set; }

    // Libsodium

    [BindProperty]
    public string SodiumPlainText { get; set; } = string.Empty;

    [BindProperty]
    public string SodiumCipherText { get; set; } = string.Empty;

    public string? SodiumEncryptedText { get; set; }

    public string? SodiumDecryptedText { get; set; }

    public string? SodiumMessage { get; set; }

    public string? SodiumError { get; set; }


    // Bouncy Castle: шифрование

    public void OnPostBouncyEncrypt()
    {
        if (string.IsNullOrWhiteSpace(BouncyPlainText))
        {
            BouncyError = "Введите текст для шифрования";
            return;
        }

        try
        {
            BouncyEncryptedText =
                _crypto.EncryptWithBouncyCastle(BouncyPlainText);

            // Автоматически подставляем результат в форму дешифрования
            BouncyCipherText = BouncyEncryptedText;

            BouncyMessage = "Текст успешно зашифрован!";
        }
        catch (Exception)
        {
            BouncyError = "Не удалось зашифровать текст";
        }
    }

    // Bouncy Castle: дешифрование

    public void OnPostBouncyDecrypt()
    {
        if (string.IsNullOrWhiteSpace(BouncyCipherText))
        {
            BouncyError = "Введите зашифрованный текст";
            return;
        }

        try
        {
            BouncyDecryptedText =
                _crypto.DecryptWithBouncyCastle(BouncyCipherText);

            BouncyMessage = "Текст успешно расшифрован!";
        }
        catch (Exception)
        {
            BouncyError =
                "Не удалось расшифровать данные. Проверьте текст и ключ приложения.";
        }
    }


    // Libsodium: шифрование

    public void OnPostSodiumEncrypt()
    {
        if (string.IsNullOrWhiteSpace(SodiumPlainText))
        {
            SodiumError = "Введите текст для шифрования";
            return;
        }

        try
        {
            SodiumEncryptedText =
                _crypto.EncryptWithSodium(SodiumPlainText);

            // Автоматически подставляем результат в форму дешифрования
            SodiumCipherText = SodiumEncryptedText;

            SodiumMessage = "Текст успешно зашифрован!";
        }
        catch (Exception)
        {
            SodiumError = "Не удалось зашифровать текст";
        }
    }

    // Libsodium: дешифрование

    public void OnPostSodiumDecrypt()
    {
        if (string.IsNullOrWhiteSpace(SodiumCipherText))
        {
            SodiumError = "Введите зашифрованный текст";
            return;
        }

        try
        {
            SodiumDecryptedText =
                _crypto.DecryptWithSodium(SodiumCipherText);

            SodiumMessage = "Текст успешно расшифрован!";
        }
        catch (Exception)
        {
            SodiumError =
                "Не удалось расшифровать данные. Проверьте текст и ключ приложения.";
        }
    }
}