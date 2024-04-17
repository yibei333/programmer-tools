using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using ProgrammerTools.Extensions;

namespace ProgrammerTools;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        Environment.SetEnvironmentVariable("WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS", "--allow-running-insecure-content");
#if WINDOWS
        Environment.SetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
#endif

        var builder = MauiApp.CreateBuilder();
        builder
            .UseLogging()
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigServices();

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}
