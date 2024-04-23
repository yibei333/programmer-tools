using Microsoft.JSInterop;
using SharpDevLib;
using SharpDevLib.Extensions.Model;
using System.Text;

#if WINDOWS
using System.Diagnostics;
#endif

namespace ProgrammerTools.Services.Common;

public class AppService : BaseService
{
    public async Task<bool> WWWRootFilesExsit(JSRequest<string> request)
    {
        var path = "wwwroot".CombinePath(request.Parameter);
        return await FileSystem.AppPackageFileExistsAsync(path);
    }

    public async Task OpenBrowser(JSRequest<string> request)
    {
        await Browser.Default.OpenAsync(new Uri(request.Parameter!), BrowserLaunchMode.SystemPreferred);
    }

    public async Task<ApplicationInfo> GetAppInfo()
    {
        var version = string.Empty;
        using var stream = await FileSystem.Current.OpenAppPackageFileAsync("wwwroot/version.txt");
        if (stream is not null)
        {
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            version = Encoding.UTF8.GetString(memoryStream.ToArray());
        }
        return new() { Version = version, Platform = DeviceInfo.Current.Platform.Convert() };
    }

    public async Task SetClipboard(JSRequest<string> request)
    {
        await Clipboard.Default.SetTextAsync(request.Parameter);
    }

    public async Task Upgrade(JSRequest<string> request)
    {
        if (request.Parameter is null)
        {
            request.JSObjectReference?.InvokeVoidAsync("notifyInstallUpdate", Result.Failed("parameter error"));
            return;
        }

        try
        {

#if WINDOWS
            var process = Process.Start(new ProcessStartInfo(request.Parameter));
            await process!.WaitForExitAsync().ContinueWith((s) =>
            {
                request.JSObjectReference?.InvokeVoidAsync("notifyInstallUpdate", Result.Succeed());
            });
#elif ANDROID
            var context = Android.App.Application.Context;
            if (context is null || context.ApplicationContext is null) throw new Exception("unable to find android context");
            var file = new Java.IO.File(request.Parameter);

            using var install = new Android.Content.Intent(Android.Content.Intent.ActionView);
            var apkURI = AndroidX.Core.Content.FileProvider.GetUriForFile(context, context.ApplicationContext.PackageName + ".provider", file);
            install.SetDataAndType(apkURI, "application/vnd.android.package-archive");
            install.AddFlags(Android.Content.ActivityFlags.NewTask);
            install.AddFlags(Android.Content.ActivityFlags.GrantReadUriPermission);
            install.AddFlags(Android.Content.ActivityFlags.ClearTop);
            install.PutExtra(Android.Content.Intent.ExtraNotUnknownSource, true);
            Platform.CurrentActivity?.StartActivity(install);
            request.JSObjectReference?.InvokeVoidAsync("notifyInstallUpdate", Result.Succeed());
            await Task.CompletedTask;
#endif
        }
        catch (Exception ex)
        {
            App.Logger.Error(ex, $"启动安装失败:{ex.Message}");
            request.JSObjectReference?.InvokeVoidAsync("notifyInstallUpdate", Result.Failed(ex.Message));
        }
    }

    public async Task<List<PickFileInfo>> PickFiles()
    {
        var files = await FilePicker.Default.PickMultipleAsync();
        return files.Select(x => GetPickerFileInfo(x.FullPath)).ToList();
    }

    PickFileInfo GetPickerFileInfo(string path)
    {
        var fileInfo = new FileInfo(path);
        return new PickFileInfo
        {
            Name = fileInfo.Name[..^fileInfo.Extension.Length],
            FullName = fileInfo.Name,
            Extension = fileInfo.Extension,
            FullPath = path,
        };
    }
}

public class ApplicationInfo
{
    public string? Version { get; set; }
    public string? Platform { get; set; }
}

public static class PlatformsConvert
{
    public static string Convert(this DevicePlatform platform)
    {
        if (platform == DevicePlatform.Android) return Platforms.android.ToString();
        else if (platform == DevicePlatform.iOS) return Platforms.ios.ToString();
        else if (platform == DevicePlatform.macOS || platform == DevicePlatform.MacCatalyst) return Platforms.mac.ToString();
        else if (platform == DevicePlatform.Tizen) return Platforms.tizen.ToString();
        else if (platform == DevicePlatform.WinUI) return Platforms.windows.ToString();
        else return platform.ToString();
    }
}

public enum Platforms
{
    android = 1,
    ios,
    mac,
    tizen,
    windows,
}

public class PickFileInfo
{
    public required string Name { get; set; }
    public required string FullName { get; set; }
    public required string Extension { get; set; }
    public required string FullPath { get; set; }
}