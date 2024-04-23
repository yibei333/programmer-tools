using Microsoft.JSInterop;
using ProgrammerTools.Services.Common;
using SharpDevLib;
using SharpDevLib.Extensions.Encryption;
using SharpDevLib.Extensions.Model;
using System.Security.Cryptography;
using System.Text;

namespace ProgrammerTools.Services.Crypto;

public static class AesCryptoService
{
    [JSInvokable]
    public static Result<string> AesEncrypt(JsParameter<AesEncryptRequest> parameter)
    {
        try
        {
            if (parameter.Parameter is null) return Result.Failed<string>("parameter required");
            if (parameter.Parameter.Key is null) return Result.Failed<string>("key required");
            if (parameter.Parameter.PlainText is null) return Result.Failed<string>("plain text required");
            CipherMode? mode = Enum.TryParse(typeof(CipherMode), parameter.Parameter.Mode, out var x) ? (CipherMode)x : null;
            if (mode is null) return Result.Failed<string>("cipher mode required");
            if (mode != CipherMode.ECB && parameter.Parameter.IV is null) return Result.Failed<string>("iv required");
            PaddingMode? padding = Enum.TryParse(typeof(PaddingMode), parameter.Parameter.Padding, out var y) ? (PaddingMode)y : null;
            if (padding is null) return Result.Failed<string>("cipher padding required");

            var encryption = App.ServiceProvider.GetRequiredService<IEncryption>();
            var option = new AesEncryptOption(parameter.Parameter.Key, parameter.Parameter.IV.NotEmpty() ? Encoding.UTF8.GetBytes(parameter.Parameter.IV!) : [])
            {
                CipherMode = mode.Value,
                Padding = padding.Value,
            };
            var cipherBytes = encryption.Symmetric.Aes.Encrypt(parameter.Parameter.PlainText, option);
            var cipherText = Convert.ToBase64String(cipherBytes);
            return Result.Succeed<string>(cipherText);
        }
        catch (Exception ex)
        {
            return Result.Failed<string>(ex.Message);
        }
    }

    [JSInvokable]
    public static Result<string> AesDecrypt(JsParameter<AesEncryptRequest> parameter)
    {
        try
        {
            if (parameter.Parameter is null) return Result.Failed<string>("parameter required");
            if (parameter.Parameter.Key is null) return Result.Failed<string>("key required");
            if (parameter.Parameter.CipherText is null) return Result.Failed<string>("cipher text required");
            CipherMode? mode = Enum.TryParse(typeof(CipherMode), parameter.Parameter.Mode, out var x) ? (CipherMode)x : null;
            if (mode is null) return Result.Failed<string>("cipher mode required");
            if (mode != CipherMode.ECB && parameter.Parameter.IV is null) return Result.Failed<string>("iv required");
            PaddingMode? padding = Enum.TryParse(typeof(PaddingMode), parameter.Parameter.Padding, out var y) ? (PaddingMode)y : null;
            if (padding is null) return Result.Failed<string>("cipher padding required");

            var encryption = App.ServiceProvider.GetRequiredService<IEncryption>();
            var option = new AesDecryptOption(parameter.Parameter.Key, parameter.Parameter.IV.NotEmpty() ? Encoding.UTF8.GetBytes(parameter.Parameter.IV!) : [])
            {
                CipherMode = mode.Value,
                Padding = padding.Value,
            };
            var plainBytes = encryption.Symmetric.Aes.Decrypt(parameter.Parameter.CipherText, option);
            var plainText = Encoding.UTF8.GetString(plainBytes);
            return Result.Succeed<string>(plainText);
        }
        catch (Exception ex)
        {
            return Result.Failed<string>(ex.Message);
        }
    }

    [JSInvokable]
    public static Result AesEncryptFile(JsParameter<AesEncryptRequest> parameter)
    {
        try
        {
            if (parameter.Parameter is null) return Result.Failed("parameter required");
            if (parameter.Parameter.Key is null) return Result.Failed("key required");
            if (parameter.Parameter.InputFiles is null || parameter.Parameter.InputFiles.Count <= 0) return Result.Failed("input files required");
            CipherMode? mode = Enum.TryParse(typeof(CipherMode), parameter.Parameter.Mode, out var x) ? (CipherMode)x : null;
            if (mode is null) return Result.Failed("cipher mode required");
            if (mode != CipherMode.ECB && parameter.Parameter.IV is null) return Result.Failed<string>("iv required");
            PaddingMode? padding = Enum.TryParse(typeof(PaddingMode), parameter.Parameter.Padding, out var y) ? (PaddingMode)y : null;
            if (padding is null) return Result.Failed("cipher padding required");

            var encryption = App.ServiceProvider.GetRequiredService<IEncryption>();
            var option = new AesEncryptOption(parameter.Parameter.Key, parameter.Parameter.IV.NotEmpty() ? Encoding.UTF8.GetBytes(parameter.Parameter.IV!) : [])
            {
                CipherMode = mode.Value,
                Padding = padding.Value,
            };
            foreach (var file in parameter.Parameter.InputFiles)
            {
                var fileInfo = new FileInfo(file.FullPath);
                if (!fileInfo.Exists)
                {
                    continue;
                }
                var targetDirectory = fileInfo.Directory!.FullName;
                var targetName = $"{file.Name}.encrypted{file.Extension}";
#if ANDROID
                targetDirectory = Android.OS.Environment.ExternalStorageDirectory?.Path.CombinePath($"Download") ?? throw new Exception("找不到外部存储目录");
#endif
                var targetPath = targetDirectory.CombinePath(targetName);
                encryption.Symmetric.Aes.EncryptFile(file.FullPath, targetPath, option);
            }
            return Result.Succeed();
        }
        catch (Exception ex)
        {
            return Result.Failed(ex.Message);
        }
    }

    [JSInvokable]
    public static Result AesDecryptFile(JsParameter<AesEncryptRequest> parameter)
    {
        try
        {
            if (parameter.Parameter is null) return Result.Failed("parameter required");
            if (parameter.Parameter.Key is null) return Result.Failed("key required");
            if (parameter.Parameter.InputFiles is null || parameter.Parameter.InputFiles.Count <= 0) return Result.Failed("input files required");
            CipherMode? mode = Enum.TryParse(typeof(CipherMode), parameter.Parameter.Mode, out var x) ? (CipherMode)x : null;
            if (mode is null) return Result.Failed<string>("cipher mode required");
            if (mode != CipherMode.ECB && parameter.Parameter.IV is null) return Result.Failed<string>("iv required");
            PaddingMode? padding = Enum.TryParse(typeof(PaddingMode), parameter.Parameter.Padding, out var y) ? (PaddingMode)y : null;
            if (padding is null) return Result.Failed<string>("cipher padding required");

            var encryption = App.ServiceProvider.GetRequiredService<IEncryption>();
            var option = new AesDecryptOption(parameter.Parameter.Key, parameter.Parameter.IV.NotEmpty() ? Encoding.UTF8.GetBytes(parameter.Parameter.IV!) : [])
            {
                CipherMode = mode.Value,
                Padding = padding.Value,
            };
            foreach (var file in parameter.Parameter.InputFiles)
            {
                var fileInfo = new FileInfo(file.FullPath);
                if (!fileInfo.Exists)
                {
                    continue;
                }
                var targetDirectory = fileInfo.Directory!.FullName;
                var targetName = $"{file.Name}.decrypted{file.Extension}";
#if ANDROID
                targetDirectory = Android.OS.Environment.ExternalStorageDirectory?.Path.CombinePath("Download") ?? throw new Exception("找不到外部存储目录");
#endif
                var targetPath = targetDirectory.CombinePath(targetName);
                encryption.Symmetric.Aes.DecryptFile(file.FullPath, targetPath, option);
            }
            return Result.Succeed();
        }
        catch (Exception ex)
        {
            return Result.Failed(ex.Message);
        }
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