using Microsoft.JSInterop;
using ProgrammerTools.Services.Common;
using SharpDevLib;
using SharpDevLib.Extensions.Model;

namespace ProgrammerTools.Services.Encode;

public class EncodeService:BaseService
{
    public async Task<string> ConvertBase64ToHext(JSRequest<string> request)
    {
        if (request.Parameter is null) throw new Exception("parameter required");
        var bytes = Convert.FromBase64String(request.Parameter);
        var result = bytes.ToHexString();
        return await Task.FromResult(result);
    }
}