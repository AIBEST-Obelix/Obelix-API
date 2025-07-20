namespace Obelix.Api.Services.Items.Services.Contracts;

public interface IEncryptionService
{
    /// <summary>
    /// Encrypts the data using the key.
    /// </summary>
    /// <param name="data">Bytes to be encrypted</param>
    /// <param name="key">Key to be used for encryption</param>
    /// <returns>Encrypted bytes</returns>
    byte[] Encrypt(byte[] data, byte[] key);

    /// <summary>
    /// Decrypts the data using the key.
    /// </summary>
    /// <param name="data">Bytes to be decrypted</param>
    /// <param name="key">Key to be used for decryption</param>
    /// <returns>Decrypted bytes</returns>
    byte[] Decrypt(byte[] data, byte[] key);
}