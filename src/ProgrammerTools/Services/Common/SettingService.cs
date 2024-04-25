using Microsoft.JSInterop;
using SharpDevLib;
using SharpDevLib.Extensions.Model;
using System.Text;

namespace ProgrammerTools.Services.Common;

public class SettingService : BaseService
{
    public async Task Set(JSRequest<KeyValueDto> request) => await SecureStorage.Default.SetAsync(request.Parameter.Key, request.Parameter.Value);

    public async Task<string?> Get(JSRequest<string> request) => await SecureStorage.Default.GetAsync(request.Parameter);
}
