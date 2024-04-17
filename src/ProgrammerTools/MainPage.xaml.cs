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
    }
}

