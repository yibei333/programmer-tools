using DocumentFormat.OpenXml.Drawing.Diagrams;
using Microsoft.JSInterop;
using ProgrammerTools.Services.Common;
using SharpDevLib;
using SharpDevLib.Extensions.Encryption;
using SharpDevLib.Extensions.Model;
using System.Security.Cryptography;
using System.Text;

namespace ProgrammerTools.Services.Crypto;

public class RsaCryptoService : CryptoService
{
    public async Task<string> Encrypt(JSRequest<RsaCryptoRequest> request)
    {
        if (request.Parameter.Text.IsNull()) throw new ArgumentNullException(nameof(request.Parameter.Text));
        this.VerifyRequest(request);

        return await Task.Run(() =>
        {
            var bytes = _encryption.Asymmetric.Rsa.Encrypt(request.Parameter.Text!, new RsaEncryptOption(request.Parameter.Key) { Padding = request.Parameter.GetPadding() });
            return Convert.ToBase64String(bytes);
        });
    }

    public async Task<string> Decrypt(JSRequest<RsaCryptoRequest> request)
    {
        if (request.Parameter.Text.IsNull()) throw new ArgumentNullException(nameof(request.Parameter.Text));
        this.VerifyRequest(request);

        return await Task.Run(() =>
        {
            var bytes = _encryption.Asymmetric.Rsa.Decrypt(request.Parameter.Text!, new RsaDecryptOption(request.Parameter.Key) { PrivateKeyPassword = request.Parameter.Password, Padding = request.Parameter.GetPadding() });
            return Encoding.UTF8.GetString(bytes);
        });
    }

    void VerifyRequest(JSRequest<RsaCryptoRequest> request)
    {
        if (request.Parameter is null) throw new ArgumentNullException(nameof(request.Parameter));
        if (request.Parameter.Key is null) throw new ArgumentNullException(nameof(request.Parameter.Key));
    }
}

public class RsaCryptoRequest : FileCryptoRequest
{
    public string? Password { get; set; }
    public required string Padding { get; set; }
    public RSAEncryptionPadding GetPadding()
    {
        if (Padding == "SHA1") return RSAEncryptionPadding.OaepSHA1;
        else if (Padding == "SHA256") return RSAEncryptionPadding.OaepSHA256;
        else if (Padding == "SHA384") return RSAEncryptionPadding.OaepSHA384;
        else if (Padding == "SHA512") return RSAEncryptionPadding.OaepSHA512;
        else if (Padding == "Pkcs1") return RSAEncryptionPadding.Pkcs1;
        else throw new NotSupportedException($"padding '{Padding}' not surppoted");
    }
}