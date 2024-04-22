using Microsoft.JSInterop;
using SharpDevLib.Extensions.Encryption;

namespace ProgrammerTools.Services.Crypto
{
    public static class CryptoService
    {
        [JSInvokable]
        public static string Test(JsParameter<string> parameter)
        {
            //var encryption = App.ServiceProvider.GetRequiredService<IEncryption>();
            //encryption.Symmetric.Aes.Encrypt("", new AesEncryptOption { });
            return $"hello,{parameter.Parameter}";
        }
    }
}
