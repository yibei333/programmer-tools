using Microsoft.JSInterop;
using ProgrammerTools.Services.Common;
using SharpDevLib;
using SharpDevLib.Extensions.Model;

namespace ProgrammerTools.Services.Encode;

public static class EncodeService
{
    [JSInvokable]
    public static Result<string> ConvertBase64ToHext(JsParameter<string> parameter)
    {
        try
        {
            if (parameter.Parameter is null) return Result.Failed<string>("parameter required");
            var bytes = Convert.FromBase64String(parameter.Parameter);
            var result = bytes.ToHexString();
            return Result.Succeed<string>(result);
        }
        catch (Exception ex)
        {
            return Result.Failed<string>(ex.Message);
        }
    }
}