using Microsoft.JSInterop;
using System.Diagnostics;

namespace ProgrammerTools.Services
{
    public static class AppService
    {
        [JSInvokable]
        public static async Task OpenBrowser(JsParameter<string> parameter)
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
    }
}
