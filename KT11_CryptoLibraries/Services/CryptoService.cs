using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Sodium;

namespace KT11_CryptoLibraries.Services;

public class CryptoService
{
    private const int AesNonceLength = 12;
    private const int SodiumNonceLength = 24;
    private const int TagLength = 128;

   
    private readonly byte[] _bouncyKey =
        RandomNumberGenerator.GetBytes(32);

    private readonly byte[] _sodiumKey =
        SecretBox.GenerateKey();


    public string EncryptWithBouncyCastle(string plainText)
    {
        byte[] nonce = RandomNumberGenerator.GetBytes(AesNonceLength);
        byte[] input = Encoding.UTF8.GetBytes(plainText);

        var cipher = new GcmBlockCipher(new AesEngine());

        cipher.Init(
            true,
            new AeadParameters(
                new KeyParameter(_bouncyKey),
                TagLength,
                nonce));

        byte[] output = new byte[cipher.GetOutputSize(input.Length)];

        int length = cipher.ProcessBytes(
            input, 0, input.Length, output, 0);

        length += cipher.DoFinal(output, length);

        byte[] encrypted = output[..length];
        
        byte[] result = nonce.Concat(encrypted).ToArray();

        return Convert.ToBase64String(result);
    }

    public string DecryptWithBouncyCastle(string encryptedText)
    {
        byte[] data = Convert.FromBase64String(encryptedText);

        if (data.Length < AesNonceLength + 16)
            throw new ArgumentException("Некорректные зашифрованные данные");

        byte[] nonce = data[..AesNonceLength];
        byte[] encrypted = data[AesNonceLength..];

        var cipher = new GcmBlockCipher(new AesEngine());

        cipher.Init(
            false,
            new AeadParameters(
                new KeyParameter(_bouncyKey),
                TagLength,
                nonce));

        byte[] output = new byte[cipher.GetOutputSize(encrypted.Length)];

        int length = cipher.ProcessBytes(
            encrypted, 0, encrypted.Length, output, 0);

        length += cipher.DoFinal(output, length);

        Array.Resize(ref output, length);

        return Encoding.UTF8.GetString(output);
    }


    public string EncryptWithSodium(string plainText)
    {
        byte[] nonce = SecretBox.GenerateNonce();

        byte[] encrypted = SecretBox.Create(
            plainText,
            nonce,
            _sodiumKey);

        byte[] result = nonce.Concat(encrypted).ToArray();

        return Convert.ToBase64String(result);
    }

    public string DecryptWithSodium(string encryptedText)
    {
        byte[] data = Convert.FromBase64String(encryptedText);

        if (data.Length < SodiumNonceLength + 16)
            throw new ArgumentException("Некоректные зашифрованные данные");

        byte[] nonce = data[..SodiumNonceLength];
        byte[] encrypted = data[SodiumNonceLength..];

        byte[] decrypted = SecretBox.Open(
            encrypted,
            nonce,
            _sodiumKey);

        return Encoding.UTF8.GetString(decrypted);
    }
}