using System.Security.Cryptography;
using Obelix.Api.Services.Items.Services.Contracts;

namespace Obelix.Api.Services.Items.Services.Implementations;

public class EncryptionService : IEncryptionService
{
    /// <inheritdoc />
    public byte[] Encrypt(byte[] data, byte[] key)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new System.IO.MemoryStream();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);

        ms.Write(aes.IV, 0, aes.IV.Length); // Prepend IV to the encrypted data
        cs.Write(data, 0, data.Length);
        cs.FlushFinalBlock();

        return ms.ToArray();
    }

    /// <inheritdoc />
    public byte[] Decrypt(byte[] data, byte[] key)
    {
        using var ms = new System.IO.MemoryStream(data);
        using var aes = Aes.Create();
        aes.Key = key;

        var iv = new byte[aes.BlockSize / 8];
        ms.Read(iv, 0, iv.Length); // Read the IV from the beginning of the data
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var reader = new MemoryStream();

        cs.CopyTo(reader);
        return reader.ToArray();
    }
}