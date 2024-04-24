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
    public async Task<string> AesEncrypt(JSRequest<AesEncryptRequest> request)
    {
        if (request.Parameter is null) throw new Exception("parameter required");
        if (request.Parameter.Key is null) throw new Exception("key required");
        if (request.Parameter.PlainText is null) throw new Exception("plain text required");
        CipherMode? mode = Enum.TryParse(typeof(CipherMode), request.Parameter.Mode, out var x) ? (CipherMode)x : null;
        if (mode is null) throw new Exception("cipher mode required");
        if (mode != CipherMode.ECB && request.Parameter.IV is null) throw new Exception("iv required");
        PaddingMode? padding = Enum.TryParse(typeof(PaddingMode), request.Parameter.Padding, out var y) ? (PaddingMode)y : null;
        if (padding is null) throw new Exception("cipher padding required");

        var encryption = App.ServiceProvider.GetRequiredService<IEncryption>();
        var option = new AesEncryptOption(request.Parameter.Key, request.Parameter.IV.NotEmpty() ? Encoding.UTF8.GetBytes(request.Parameter.IV!) : [])
        {
            CipherMode = mode.Value,
            Padding = padding.Value,
        };
        var cipherBytes = encryption.Symmetric.Aes.Encrypt(request.Parameter.PlainText, option);
        var cipherText = Convert.ToBase64String(cipherBytes);
        return await Task.FromResult(cipherText);
    }

    public async Task<string> AesDecrypt(JSRequest<AesEncryptRequest> request)
    {
        if (request.Parameter is null) throw new Exception("parameter required");
        if (request.Parameter.Key is null) throw new Exception("key required");
        if (request.Parameter.CipherText is null) throw new Exception("cipher text required");
        CipherMode? mode = Enum.TryParse(typeof(CipherMode), request.Parameter.Mode, out var x) ? (CipherMode)x : null;
        if (mode is null) throw new Exception("cipher mode required");
        if (mode != CipherMode.ECB && request.Parameter.IV is null) throw new Exception("iv required");
        PaddingMode? padding = Enum.TryParse(typeof(PaddingMode), request.Parameter.Padding, out var y) ? (PaddingMode)y : null;
        if (padding is null) throw new Exception("cipher padding required");

        var encryption = App.ServiceProvider.GetRequiredService<IEncryption>();
        var option = new AesDecryptOption(request.Parameter.Key, request.Parameter.IV.NotEmpty() ? Encoding.UTF8.GetBytes(request.Parameter.IV!) : [])
        {
            CipherMode = mode.Value,
            Padding = padding.Value,
        };
        var plainBytes = encryption.Symmetric.Aes.Decrypt(request.Parameter.CipherText, option);
        var plainText = Encoding.UTF8.GetString(plainBytes);
        return await Task.FromResult(plainText);
    }

    public async Task AesEncryptFile(JSRequest<AesEncryptRequest> request)
    {
        if (request.Parameter is null) throw new Exception("parameter required");
        if (request.Parameter.Key is null) throw new Exception("key required");
        if (request.Parameter.InputFiles is null || request.Parameter.InputFiles.Count <= 0) throw new Exception("input files required");
        CipherMode? mode = Enum.TryParse(typeof(CipherMode), request.Parameter.Mode, out var x) ? (CipherMode)x : null;
        if (mode is null) throw new Exception("cipher mode required");
        if (mode != CipherMode.ECB && request.Parameter.IV is null) throw new Exception("iv required");
        PaddingMode? padding = Enum.TryParse(typeof(PaddingMode), request.Parameter.Padding, out var y) ? (PaddingMode)y : null;
        if (padding is null) throw new Exception("cipher padding required");

        var encryption = App.ServiceProvider.GetRequiredService<IEncryption>();
        var option = new AesEncryptOption(request.Parameter.Key, request.Parameter.IV.NotEmpty() ? Encoding.UTF8.GetBytes(request.Parameter.IV!) : [])
        {
            CipherMode = mode.Value,
            Padding = padding.Value,
        };
        await Parallel.ForEachAsync(request.Parameter.InputFiles, async (file, _) =>
        {
            try
            {
                await SetInputFileStatus(request, file.FullName, 1);
                var fileInfo = new FileInfo(file.FullPath);
                if (!fileInfo.Exists) throw new FileNotFoundException();
                var targetDirectory = fileInfo.Directory!.FullName;
                var targetName = $"{file.Name}.encrypted{file.Extension}";
#if ANDROID
                targetDirectory = Android.OS.Environment.ExternalStorageDirectory?.Path.CombinePath($"Download") ?? throw new Exception("找不到外部存储目录");
#endif
                var targetPath = targetDirectory.CombinePath(targetName);
                await Task.Run(() => encryption.Symmetric.Aes.EncryptFile(file.FullPath, targetPath, option));
                await SetInputFileStatus(request, file.FullName, 2, targetPath);
            }
            catch (Exception ex)
            {
                await SetInputFileStatus(request, file.FullName, 3, ex.Message);
            }
        });
        await Task.CompletedTask;
    }

    public async Task AesDecryptFile(JSRequest<AesEncryptRequest> request)
    {
        if (request.Parameter is null) throw new Exception("parameter required");
        if (request.Parameter.Key is null) throw new Exception("key required");
        if (request.Parameter.InputFiles is null || request.Parameter.InputFiles.Count <= 0) throw new Exception("input files required");
        CipherMode? mode = Enum.TryParse(typeof(CipherMode), request.Parameter.Mode, out var x) ? (CipherMode)x : null;
        if (mode is null) throw new Exception("cipher mode required");
        if (mode != CipherMode.ECB && request.Parameter.IV is null) throw new Exception("iv required");
        PaddingMode? padding = Enum.TryParse(typeof(PaddingMode), request.Parameter.Padding, out var y) ? (PaddingMode)y : null;
        if (padding is null) throw new Exception("cipher padding required");

        var encryption = App.ServiceProvider.GetRequiredService<IEncryption>();
        var option = new AesDecryptOption(request.Parameter.Key, request.Parameter.IV.NotEmpty() ? Encoding.UTF8.GetBytes(request.Parameter.IV!) : [])
        {
            CipherMode = mode.Value,
            Padding = padding.Value,
        };
        await Parallel.ForEachAsync(request.Parameter.InputFiles, async (file, _) =>
        {
            try
            {
                await SetInputFileStatus(request, file.FullName, 1);
                var fileInfo = new FileInfo(file.FullPath);
                if (!fileInfo.Exists) throw new FileNotFoundException();
                var targetDirectory = fileInfo.Directory!.FullName;
                var targetName = $"{file.Name}.decrypted{file.Extension}";
#if ANDROID
                targetDirectory = Android.OS.Environment.ExternalStorageDirectory?.Path.CombinePath("Download") ?? throw new Exception("找不到外部存储目录");
#endif
                var targetPath = targetDirectory.CombinePath(targetName);
                await Task.Run(() => encryption.Symmetric.Aes.DecryptFile(file.FullPath, targetPath, option));
                await SetInputFileStatus(request, file.FullName, 2, targetPath);
            }
            catch (Exception ex)
            {
                await SetInputFileStatus(request, file.FullName, 3, ex.Message);
            }
        });
        await Task.CompletedTask;
    }

    async Task SetInputFileStatus(JSRequest request, string name, int status, object? data = null)
    {
        if (request.JSObjectReference is null) return;
        await request.JSObjectReference.InvokeVoidAsync("setInputFileStatus", name, status, data);
    }
}

public class AesEncryptRequest
{
    public string? PlainText { get; set; }
    public string? CipherText { get; set; }
    public string? Key { get; set; }
    public string? IV { get; set; }
    public string? Mode { get; set; }
    public string? Padding { get; set; }
    public List<PickFileInfo> InputFiles { get; set; } = [];
}