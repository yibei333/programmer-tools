using DocumentFormat.OpenXml.Drawing.Diagrams;
using Microsoft.JSInterop;
using ProgrammerTools.Services.Common;
using SharpDevLib;
using SharpDevLib.Extensions.Encryption;
using SharpDevLib.Extensions.Model;
using System.Security.Cryptography;
using System.Text;

namespace ProgrammerTools.Services.Crypto;

public class RsaKeyService : BaseService
{
    IRsaKey _rsaKey;

    public RsaKeyService()
    {
        _rsaKey = App.ServiceProvider.GetRequiredService<IRsaKey>();
    }

    public async Task<RsaKeyPair> GenerateKeyPair(JSRequest<GenerateKeyPairRequest> request)
    {
        if (request.Parameter is null) throw new ArgumentNullException(nameof(request.Parameter));

        if (string.IsNullOrWhiteSpace(request.Parameter.Password))
        {
            var privateKey = _rsaKey.ExportPrivateKey(request.Parameter.GetKeyType(), request.Parameter.Length);
            var publicKey = _rsaKey.ExportPublicKey(privateKey);
            return await Task.FromResult(new RsaKeyPair { Private = privateKey, Public = publicKey });
        }
        else
        {
            var privateKey = _rsaKey.ExportPrivateKey(request.Parameter.GetKeyType(), request.Parameter.Password, request.Parameter.Length);
            var publicKey = _rsaKey.ExportPublicKey(privateKey, request.Parameter.Password);
            return await Task.FromResult(new RsaKeyPair { Private = privateKey, Public = publicKey });
        }
    }

    public async Task<string> ExportPublicKey(JSRequest<ExportPublicKeyRequest> request)
    {
        if (request.Parameter is null) throw new ArgumentNullException(nameof(request.Parameter));

        try
        {
            if (string.IsNullOrWhiteSpace(request.Parameter.Password))
            {
                var publicKey = _rsaKey.ExportPublicKey(request.Parameter.PrivateKey);
                return await Task.FromResult(publicKey);
            }
            else
            {
                var publicKey = _rsaKey.ExportPublicKey(request.Parameter.PrivateKey, request.Parameter.Password);
                return await Task.FromResult(publicKey);
            }
        }
        catch (NotSupportedException)
        {
            throw new Exception($"{(request.Parameter.Password.IsNull() ? "password required" : "password should be null")}");
        }
    }

    public async Task<bool> MatchKeyPair(JSRequest<MatchKeyPairRequest> request)
    {
        if (request.Parameter is null) throw new ArgumentNullException(nameof(request.Parameter));
        try
        {
            if (string.IsNullOrWhiteSpace(request.Parameter.Password))
            {
                var isMatch = _rsaKey.IsKeyPairMatch(request.Parameter.PrivateKey, request.Parameter.PublicKey);
                return await Task.FromResult(isMatch);
            }
            else
            {
                var isMatch = _rsaKey.IsKeyPairMatch(request.Parameter.PrivateKey, request.Parameter.PublicKey, request.Parameter.Password);
                return await Task.FromResult(isMatch);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"may be password error", ex);
        }
    }

    public async Task<string?> GetKeyType(JSRequest<string> request)
    {
        if (request.Parameter is null) throw new ArgumentNullException(nameof(request.Parameter));
        try
        {
            return await Task.FromResult(_rsaKey.KeyType(request.Parameter).ToString());
        }
        catch
        {
            return null;
        }
    }

    public async Task<string> Convert(JSRequest<ConvertRequest> request)
    {
        return await Task.FromResult(_rsaKey.ConvertType(request.Parameter.Source,request.Parameter.TargetType));
    }
}

public class GenerateKeyPairRequest
{
    public int Length { get; set; }
    public string? Password { get; set; }
    public required string Type { get; set; }
    public RsaKeyType GetKeyType()
    {
        if (Type == "Pkcs1") return RsaKeyType.Pkcs1;
        else if (Type == "Pkcs8") return RsaKeyType.Pkcs8;
        else throw new NotSupportedException();
    }
}

public class ExportPublicKeyRequest
{
    public required string PrivateKey { get; set; }
    public string? Password { get; set; }
}

public class MatchKeyPairRequest
{
    public required string PrivateKey { get; set; }
    public required string PublicKey { get; set; }
    public string? Password { get; set; }
}

public class ConvertRequest
{
    public required string Source { get; set; }
    public required RsaKeyType TargetType { get; set; }
}

public class RsaKeyPair
{
    public required string Private { get; set; }
    public required string Public { get; set; }
}