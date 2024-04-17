using CommunityToolkit.Maui.Alerts;
using ProgrammerTools.Extensions;

namespace ProgrammerTools;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        App.Logger.Info("started");

        Toast.Make($"path:{AppExtension.LogDirectory}").Show();
        blazorWebView.StartPath = "/";

    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        Toast.Make($"path:{AppExtension.LogDirectory}").Show();
        Toast.Make($"path:{blazorWebView.HostPage},{blazorWebView.StartPath}").Show();
    }
}

