using Microsoft.JSInterop;
using SharpDevLib;
using SharpDevLib.Extensions.Model;
using System.Diagnostics;
using System.Text;

namespace ProgrammerTools.Services;

public static class AppService
{
    [JSInvokable]
    public static async Task<bool> WwwRootFilesExsitAsync(JsParameter<string> parameter)
    {
        var path = "wwwroot".CombinePath(parameter.Parameter);
        return await FileSystem.AppPackageFileExistsAsync(path);
    }

    [JSInvokable]
    public static async Task OpenBrowserAsync(JsParameter<string> parameter)
    {
        try
        {
            await Browser.Default.OpenAsync(new Uri(parameter.Parameter!), BrowserLaunchMode.SystemPreferred);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"open browser failed:{ex.Message}");
        }
    }

    [JSInvokable]
    public static async Task<ApplicationInfo> GetAppInfoAsync()
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

    [JSInvokable]
    public static async Task<Result> SetClipboardAsync(JsParameter<string> parameter)
    {
        try
        {
            await Clipboard.Default.SetTextAsync(parameter.Parameter);
            return Result.Succeed();
        }
        catch (Exception ex)
        {
            return Result.Failed(ex.Message);
        }
    }

    [JSInvokable]
    public static void Upgrade(JsParameter<string> parameter)
    {
        if (parameter.Parameter is null)
        {
            parameter.ParameterRefercence?.InvokeVoidAsync("notifyInstallUpdate", Result.Failed("parameter error"));
            return;
        }

        try
        {

#if WINDOWS
            var process = Process.Start(new ProcessStartInfo(parameter.Parameter));
            process!.WaitForExitAsync().ContinueWith((s) =>
            {
                parameter.ParameterRefercence?.InvokeVoidAsync("notifyInstallUpdate", Result.Succeed());
            });
#elif ANDROID
            var context = Android.App.Application.Context;
            if (context is null || context.ApplicationContext is null) throw new Exception("unable to find android context");
            var file = new Java.IO.File(parameter.Parameter);

            using var install = new Android.Content.Intent(Android.Content.Intent.ActionView);
            var apkURI = AndroidX.Core.Content.FileProvider.GetUriForFile(context, context.ApplicationContext.PackageName + ".provider", file);
            install.SetDataAndType(apkURI, "application/vnd.android.package-archive");
            install.AddFlags(Android.Content.ActivityFlags.NewTask);
            install.AddFlags(Android.Content.ActivityFlags.GrantReadUriPermission);
            install.AddFlags(Android.Content.ActivityFlags.ClearTop);
            install.PutExtra(Android.Content.Intent.ExtraNotUnknownSource, true);
            Platform.CurrentActivity?.StartActivity(install);
            parameter.ParameterRefercence?.InvokeVoidAsync("notifyInstallUpdate", Result.Succeed());
#endif
        }
        catch (Exception ex)
        {
            App.Logger.Error(ex, $"启动安装失败:{ex.Message}");
            parameter.ParameterRefercence?.InvokeVoidAsync("notifyInstallUpdate", Result.Failed(ex.Message));
        }
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