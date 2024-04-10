using Microsoft.JSInterop;

namespace ProgrammerTools.Services.Crypto
{
    public static class CryptoService
    {
        [JSInvokable]
        public static string Test(JsParameter<string> parameter)
        {
            return $"hello,{parameter.Parameter}";
        }
    }
}
