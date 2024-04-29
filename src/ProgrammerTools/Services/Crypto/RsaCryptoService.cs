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
        this.VerifyRequest(request);

        return await Task.Run(() =>
        {
            var bytes = _encryption.Asymmetric.Rsa.Encrypt(request.Parameter.Text, new RsaEncryptOption(request.Parameter.Key) { Padding = request.Parameter.GetCryptoPadding() });
            return Convert.ToBase64String(bytes);
        });
    }

    public async Task<string> Decrypt(JSRequest<RsaCryptoRequest> request)
    {
        this.VerifyRequest(request);

        return await Task.Run(() =>
        {
            var bytes = _encryption.Asymmetric.Rsa.Decrypt(request.Parameter.Text, new RsaDecryptOption(request.Parameter.Key) { PrivateKeyPassword = request.Parameter.Password, Padding = request.Parameter.GetCryptoPadding() });
            return Encoding.UTF8.GetString(bytes);
        });
    }

    public async Task<string> Sign(JSRequest<RsaCryptoRequest> request)
    {
        this.VerifyRequest(request);

        return await Task.Run(() =>
        {
            var bytes = _encryption.Asymmetric.Rsa.Sign(request.Parameter.Text, new RsaSignOption(request.Parameter.Key, request.Parameter.GetSignHashAlgorithm()) { SignaturePadding = request.Parameter.GetSignPadding(), PrivateKeyPassword = request.Parameter.Password });
            return Convert.ToBase64String(bytes);
        });
    }

    public async Task<bool> VerifySign(JSRequest<RsaCryptoRequest> request)
    {
        this.VerifyRequest(request);
        if (request.Parameter.Signature.IsNull()) throw new ArgumentNullException(nameof(request.Parameter.Signature));

        return await Task.Run(() =>
        {
            return _encryption.Asymmetric.Rsa.VerifySign(request.Parameter.Text, new RsaVerifySignOption(Convert.FromBase64String(request.Parameter.Signature), request.Parameter.Key, request.Parameter.GetSignHashAlgorithm()) { SignaturePadding = request.Parameter.GetSignPadding() });
        });
    }

    void VerifyRequest(JSRequest<RsaCryptoRequest> request)
    {
        if (request.Parameter is null) throw new ArgumentNullException(nameof(request.Parameter));
        if (request.Parameter.Text.IsNull()) throw new ArgumentNullException(nameof(request.Parameter.Text));
        if (request.Parameter.Key is null) throw new ArgumentNullException(nameof(request.Parameter.Key));
    }
}

public class RsaCryptoRequest : FileCryptoRequest
{
    public string? Signature { get; set; }
    public string? Password { get; set; }
    public required string Padding { get; set; }
    public RSAEncryptionPadding GetCryptoPadding()
    {
        if (Padding == "OaepSHA1") return RSAEncryptionPadding.OaepSHA1;
        else if (Padding == "OaepSHA256") return RSAEncryptionPadding.OaepSHA256;
        else if (Padding == "OaepSHA384") return RSAEncryptionPadding.OaepSHA384;
        else if (Padding == "OaepSHA512") return RSAEncryptionPadding.OaepSHA512;
        else if (Padding == "Pkcs1") return RSAEncryptionPadding.Pkcs1;
        else throw new NotSupportedException($"padding '{Padding}' not surppoted");
    }
    public RSASignaturePadding GetSignPadding()
    {
        if (Padding == "Pkcs1") return RSASignaturePadding.Pkcs1;
        else if (Padding == "Pss") return RSASignaturePadding.Pss;
        else throw new NotSupportedException($"padding '{Padding}' not surppoted");
    }
    public string? SignHashAlgorithm { get; set; }
    public HashAlgorithmName GetSignHashAlgorithm()
    {
        if (SignHashAlgorithm == "MD5") return HashAlgorithmName.MD5;
        else if (SignHashAlgorithm == "SHA1") return HashAlgorithmName.SHA1;
        else if (SignHashAlgorithm == "SHA256") return HashAlgorithmName.SHA256;
        else if (SignHashAlgorithm == "SHA384") return HashAlgorithmName.SHA384;
        else if (SignHashAlgorithm == "SHA512") return HashAlgorithmName.SHA512;
        else throw new NotSupportedException($"algorithm '{SignHashAlgorithm}' not surppoted");
    }
}