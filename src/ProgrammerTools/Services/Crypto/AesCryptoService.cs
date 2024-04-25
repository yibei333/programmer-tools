using DocumentFormat.OpenXml.Drawing.Diagrams;
using Microsoft.JSInterop;
using ProgrammerTools.Services.Common;
using SharpDevLib;
using SharpDevLib.Extensions.Encryption;
using SharpDevLib.Extensions.Model;
using System.Security.Cryptography;
using System.Text;

namespace ProgrammerTools.Services.Crypto;

public class AesCryptoService : BaseService
{
    IEncryption _encryption;

    public AesCryptoService()
    {
        _encryption = App.ServiceProvider.GetRequiredService<IEncryption>();
    }

    public async Task<string> AesEncrypt(JSRequest<AesEncryptRequest> request)
    {
        if (request.Parameter.PlainText is null) throw new ArgumentNullException(nameof(request.Parameter.PlainText));
        this.VerifyAesRequest(request, out var mode, out var padding);

        var option = CreateEncryptionOption<AesEncryptOption>(request, mode, padding);
        var cipherBytes = _encryption.Symmetric.Aes.Encrypt(request.Parameter.PlainText, option);
        var cipherText = Convert.ToBase64String(cipherBytes);
        return await Task.FromResult(cipherText);
    }

    public async Task<string> AesDecrypt(JSRequest<AesEncryptRequest> request)
    {
        if (request.Parameter.CipherText is null) throw new ArgumentNullException(nameof(request.Parameter.CipherText));
        this.VerifyAesRequest(request, out var mode, out var padding);

        var option = CreateEncryptionOption<AesDecryptOption>(request, mode, padding);
        var plainBytes = _encryption.Symmetric.Aes.Decrypt(request.Parameter.CipherText, option);
        var plainText = Encoding.UTF8.GetString(plainBytes);
        return await Task.FromResult(plainText);
    }

    public async Task AesEncryptFile(JSRequest<AesEncryptRequest> request)
    {
        if (request.Parameter.InputFiles is null || request.Parameter.InputFiles.Count <= 0) throw new ArgumentNullException(nameof(request.Parameter.InputFiles));
        this.VerifyAesRequest(request, out var mode, out var padding);

        var option = CreateEncryptionOption<AesEncryptOption>(request, mode, padding);
        await RunFileCryptoTask(true, request, (source, target) => _encryption.Symmetric.Aes.EncryptFile(source, target, option));

        await Task.CompletedTask;
    }

    public async Task AesDecryptFile(JSRequest<AesEncryptRequest> request)
    {
        if (request.Parameter.InputFiles is null || request.Parameter.InputFiles.Count <= 0) throw new ArgumentNullException(nameof(request.Parameter.InputFiles));
        this.VerifyAesRequest(request, out var mode, out var padding);

        var option = CreateEncryptionOption<AesDecryptOption>(request, mode, padding);
        await RunFileCryptoTask(false, request, (source, target) => _encryption.Symmetric.Aes.DecryptFile(source, target, option));

        await Task.CompletedTask;
    }

    void VerifyAesRequest(JSRequest<AesEncryptRequest> request, out CipherMode mode, out PaddingMode padding)
    {
        if (request.Parameter is null) throw new ArgumentNullException(nameof(request.Parameter));
        if (request.Parameter.Key is null) throw new ArgumentNullException(nameof(request.Parameter.Key));
        mode = Enum.TryParse(typeof(CipherMode), request.Parameter.Mode, out var x) ? (CipherMode)x : throw new ArgumentNullException(nameof(request.Parameter.Mode));
        if (mode != CipherMode.ECB && request.Parameter.IV is null) throw new ArgumentNullException(nameof(request.Parameter.IV));
        padding = Enum.TryParse(typeof(PaddingMode), request.Parameter.Padding, out var y) ? (PaddingMode)y : throw new ArgumentNullException(nameof(request.Parameter.Padding));
    }

    T CreateEncryptionOption<T>(JSRequest<AesEncryptRequest> request, CipherMode mode, PaddingMode padding) where T : AesEncryptOption
    {
        var option = new AesDecryptOption(request.Parameter.Key, request.Parameter.IV.NotEmpty() ? Encoding.UTF8.GetBytes(request.Parameter.IV!) : [])
        {
            CipherMode = mode,
            Padding = padding,
        };
        return (option as T)!;
    }

    async Task RunFileCryptoTask(bool isEncrypt, JSRequest<AesEncryptRequest> request, Action<string, string> action)
    {
        await Parallel.ForEachAsync(request.Parameter.InputFiles, async (file, _) =>
        {
            try
            {
                await SetInputFileStatus(request, file.FullName, 1);
                var fileInfo = new FileInfo(file.FullPath);
                if (!fileInfo.Exists) throw new FileNotFoundException();
                var targetDirectory = fileInfo.Directory!.FullName;
                var targetName = $"{file.Name}.{(isEncrypt ? "encrypted" : "decrypted")}{file.Extension}";
#if ANDROID
                targetDirectory = Android.OS.Environment.ExternalStorageDirectory?.Path.CombinePath($"Download") ?? throw new Exception("找不到外部存储目录");
#endif
                var targetPath = targetDirectory.CombinePath(targetName);
                await Task.Run(() => action(file.FullPath, targetPath));
                await SetInputFileStatus(request, file.FullName, 2, targetPath);
            }
            catch (Exception ex)
            {
                await SetInputFileStatus(request, file.FullName, 3, ex.Message);
            }
        });
    }

    async Task SetInputFileStatus(JSRequest request, string name, int status, object? data = null)
    {
        if (request.JSObjectReference is null) return;
        await request.JSObjectReference.InvokeVoidAsync("setInputFileStatus", name, status, data);
    }
}

public class AesEncryptRequest
{
    public required string Key { get; set; }
    public required string Mode { get; set; }
    public required string Padding { get; set; }
    public string? IV { get; set; }
    public string? PlainText { get; set; }
    public string? CipherText { get; set; }
    public List<PickFileInfo> InputFiles { get; set; } = [];
}