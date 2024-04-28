using DocumentFormat.OpenXml.Drawing.Diagrams;
using Microsoft.JSInterop;
using ProgrammerTools.Services.Common;
using SharpDevLib;
using SharpDevLib.Extensions.Encryption;
using SharpDevLib.Extensions.Model;
using System.Security.Cryptography;
using System.Text;

namespace ProgrammerTools.Services.Crypto;

public abstract class CryptoService : BaseService
{
    protected IEncryption _encryption;

    public CryptoService()
    {
        _encryption = App.ServiceProvider.GetRequiredService<IEncryption>();
    }

    protected async Task RunFileCryptoTask<T>(bool isEncrypt, JSRequest<T> request, Action<string, string> action, object? extra = null) where T : FileCryptoRequest
    {
        if (request.Parameter.InputFiles is null || request.Parameter.InputFiles.Count <= 0) throw new ArgumentNullException(nameof(request.Parameter.InputFiles));

        await Parallel.ForEachAsync(request.Parameter.InputFiles, async (file, _) =>
        {
            try
            {
                await SetInputFileStatus(request, file.FullName, 1, null, extra);
                var fileInfo = new FileInfo(file.FullPath);
                if (!fileInfo.Exists) throw new FileNotFoundException();
                var targetDirectory = fileInfo.Directory!.FullName;
                var targetName = $"{file.Name}.{(isEncrypt ? "encrypted" : "decrypted")}{file.Extension}";
#if ANDROID
                targetDirectory = Android.OS.Environment.ExternalStorageDirectory?.Path.CombinePath($"Download") ?? throw new Exception("找不到外部存储目录");
#endif
                var targetPath = targetDirectory.CombinePath(targetName);
                await Task.Run(() => action(file.FullPath, targetPath));
                await SetInputFileStatus(request, file.FullName, 2, targetPath, extra);
            }
            catch (Exception ex)
            {
                await SetInputFileStatus(request, file.FullName, 3, ex.Message, extra);
            }
        });
    }

    async Task SetInputFileStatus(JSRequest request, string name, int status, object? data = null, object? extra = null)
    {
        if (request.JSObjectReference is null) return;
        if (extra is null) await request.JSObjectReference.InvokeVoidAsync("setInputFileStatus", name, status, data);
        else await request.JSObjectReference.InvokeVoidAsync("setInputFileStatus", name, status, data, extra);
    }
}

public class FileCryptoRequest
{
    public required string Key { get; set; }
    public required string Text { get; set; }
    public List<PickFileInfo> InputFiles { get; set; } = [];
}