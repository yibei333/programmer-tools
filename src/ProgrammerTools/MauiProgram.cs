using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

namespace ProgrammerTools;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .ConfigureLifecycleEvents(events =>
            {
#if WINDOWS
                events.AddWindows(windowsLifecycleBuilder =>
                {
                    windowsLifecycleBuilder.OnWindowCreated(window =>
                    {
                        window.ExtendsContentIntoTitleBar = true;
                        var bar = new Microsoft.UI.Xaml.Controls.StackPanel { Height=50,Background=new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red) };
                        var content = new Microsoft.UI.Xaml.Controls.TextBlock { Text = "aa" };
                        bar.Children.Add(content);
                        window.SetTitleBar(bar);
                        window.Title = "aaaa";
                    });
                });
#endif
            });

        builder.Services.AddMauiBlazorWebView();
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
