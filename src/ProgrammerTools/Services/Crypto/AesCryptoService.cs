using DocumentFormat.OpenXml.Drawing.Diagrams;
using Microsoft.JSInterop;
using ProgrammerTools.Services.Common;
using SharpDevLib;
using SharpDevLib.Extensions.Encryption;
using SharpDevLib.Extensions.Model;
using System.Security.Cryptography;
using System.Text;

namespace ProgrammerTools.Services.Crypto;

public class AesCryptoService : CryptoService
{
    public async Task<string> Encrypt(JSRequest<AesCryptoRequest> request)
    {
        if (request.Parameter.Text is null) throw new ArgumentNullException(nameof(request.Parameter.Text));
        this.VerifyRequest(request, out var mode, out var padding);

        var option = CreateEncryptionOption<AesEncryptOption>(request, mode, padding);
        var cipherBytes = _encryption.Symmetric.Aes.Encrypt(request.Parameter.Text, option);
        var cipherText = Convert.ToBase64String(cipherBytes);
        return await Task.FromResult(cipherText);
    }

    public async Task<string> Decrypt(JSRequest<AesCryptoRequest> request)
    {
        if (request.Parameter.Text is null) throw new ArgumentNullException(nameof(request.Parameter.Text));
        this.VerifyRequest(request, out var mode, out var padding);

        var option = CreateEncryptionOption<AesDecryptOption>(request, mode, padding);
        var plainBytes = _encryption.Symmetric.Aes.Decrypt(request.Parameter.Text, option);
        var plainText = Encoding.UTF8.GetString(plainBytes);
        return await Task.FromResult(plainText);
    }

    public async Task EncryptFile(JSRequest<AesCryptoRequest> request)
    {
        this.VerifyRequest(request, out var mode, out var padding);

        var option = CreateEncryptionOption<AesEncryptOption>(request, mode, padding);
        await RunFileCryptoTask(true, request, (source, target) => _encryption.Symmetric.Aes.EncryptFile(source, target, option));
    }

    public async Task DecryptFile(JSRequest<AesCryptoRequest> request)
    {
        this.VerifyRequest(request, out var mode, out var padding);

        var option = CreateEncryptionOption<AesDecryptOption>(request, mode, padding);
        await RunFileCryptoTask(false, request, (source, target) => _encryption.Symmetric.Aes.DecryptFile(source, target, option));
    }

    void VerifyRequest(JSRequest<AesCryptoRequest> request, out CipherMode mode, out PaddingMode padding)
    {
        if (request.Parameter is null) throw new ArgumentNullException(nameof(request.Parameter));
        if (request.Parameter.Key is null) throw new ArgumentNullException(nameof(request.Parameter.Key));
        mode = Enum.TryParse(typeof(CipherMode), request.Parameter.Mode, out var x) ? (CipherMode)x : throw new ArgumentNullException(nameof(request.Parameter.Mode));
        if (mode != CipherMode.ECB && request.Parameter.IV is null) throw new ArgumentNullException(nameof(request.Parameter.IV));
        padding = Enum.TryParse(typeof(PaddingMode), request.Parameter.Padding, out var y) ? (PaddingMode)y : throw new ArgumentNullException(nameof(request.Parameter.Padding));
    }

    T CreateEncryptionOption<T>(JSRequest<AesCryptoRequest> request, CipherMode mode, PaddingMode padding) where T : AesEncryptOption
    {
        var option = new AesDecryptOption(request.Parameter.Key, request.Parameter.IV.NotEmpty() ? Encoding.UTF8.GetBytes(request.Parameter.IV!) : [])
        {
            CipherMode = mode,
            Padding = padding,
        };
        return (option as T)!;
    }
}

public class AesCryptoRequest : FileCryptoRequest
{
    public required string Mode { get; set; }
    public required string Padding { get; set; }
    public string? IV { get; set; }
}