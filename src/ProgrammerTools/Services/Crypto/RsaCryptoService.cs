using DocumentFormat.OpenXml.Drawing.Diagrams;
using Microsoft.JSInterop;
using ProgrammerTools.Services.Common;
using SharpDevLib;
using SharpDevLib.Extensions.Encryption;
using SharpDevLib.Extensions.Model;
using System.Security.Cryptography;
using System.Text;

namespace ProgrammerTools.Services.Crypto;

public class RsaCryptoService : BaseService
{
    IEncryption _encryption;

    public RsaCryptoService()
    {
        _encryption = App.ServiceProvider.GetRequiredService<IEncryption>();
    }

}