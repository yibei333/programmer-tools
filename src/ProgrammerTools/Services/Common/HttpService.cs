using CommunityToolkit.Maui.Storage;
using Microsoft.JSInterop;
using SharpDevLib;
using SharpDevLib.Extensions.Http;

namespace ProgrammerTools.Services.Common;

public static class HttpService
{
    [JSInvokable]
    public static async Task<HttpResult<string>> UploadFolderAsync(JsParameter<RequestOptions> request)
    {
        var pickResult = await FolderPicker.Default.PickAsync();
        if (!pickResult.IsSuccessful) return new HttpResult<string> { IsSuccess = true, Code = System.Net.HttpStatusCode.OK, Message = "cancle" };

        var directory = new DirectoryInfo(pickResult.Folder.Path) ?? throw new NullReferenceException();
        var rootPath = directory.Parent?.FullName ?? string.Empty;
        var files = GetFolderFiles(rootPath, directory);
        if (files.IsEmpty()) return new HttpResult<string> { IsSuccess = false, Code = System.Net.HttpStatusCode.InternalServerError, Message = "不支持上传空文件夹" };

        request.Parameter!.Method = "post";
        var option = new FormOption(request.Parameter.Url, files).SetOptions(request);
        return await App.ServiceProvider.GetRequiredService<IHttpService>().PostFormAsync<string>(option);
    }

    private static List<FormFile> GetFolderFiles(string rootPath, DirectoryInfo directory)
    {
        var childDirectories = directory.GetDirectories();
        var childDirecotryFiles = childDirectories.SelectMany(x => GetFolderFiles(rootPath, x));
        var childFiles = directory.GetFiles().Select(x => new FormFile("Files", rootPath.IsEmpty() ? x.FullName : x.FullName.Replace(rootPath, ""), File.OpenRead(x.FullName)));
        return childDirecotryFiles.Union(childFiles).ToList();
    }

    [JSInvokable]
    public static async Task<HttpResult<string>> UploadFilesAsync(JsParameter<RequestOptions> request)
    {
        var pickResult = await FilePicker.Default.PickMultipleAsync();
        if (!pickResult.Any()) return new HttpResult<string> { IsSuccess = true, Code = System.Net.HttpStatusCode.OK, Message = "cancle" };

        var files = pickResult.Select(x => new FormFile("Files", x.FileName, File.OpenRead(x.FullPath))).ToList();
        if (files.IsEmpty()) return new HttpResult<string> { IsSuccess = false, Code = System.Net.HttpStatusCode.InternalServerError, Message = "没有需要上传的文件" };

        request.Parameter!.Method = "post";
        var option = new FormOption(request.Parameter.Url, files).SetOptions(request);
        return await App.ServiceProvider.GetRequiredService<IHttpService>().PostFormAsync<string>(option);
    }

    [JSInvokable]
    public static async Task<HttpResult<string>> DownloadAsync(JsParameter<RequestOptions> options)
    {
        try
        {
            var requestOptions = new ParameterOption(options.Parameter!.Url).SetOptions(options);
            var stream = await App.ServiceProvider.GetRequiredService<IHttpService>().GetStreamAsync(requestOptions);
            if (DeviceInfo.Current.Platform == DevicePlatform.Android) return await AndroidDownloadAsync(options.Parameter, stream);

            var result = await FileSaver.Default.SaveAsync(options.Parameter.Name, stream, CancellationToken.None);
            result.EnsureSuccess();
            stream.Close();
            return new HttpResult<string> { IsSuccess = result.IsSuccessful, Data = result.FilePath, Code = result.IsSuccessful ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.InternalServerError, Message = result.IsSuccessful ? $"已保存在位置:{result.FilePath}" : result.Exception?.Message ?? string.Empty };
        }
        catch (Exception ex)
        {
            return new HttpResult<string> { IsSuccess = false, Message = ex.Message, Code = System.Net.HttpStatusCode.InternalServerError };
        }
    }

    static async Task<HttpResult<string>> AndroidDownloadAsync(RequestOptions options, Stream stream)
    {
        try
        {
            var path = string.Empty;
#if ANDROID
            path = Android.OS.Environment.ExternalStorageDirectory?.Path.CombinePath($"Download/{options.Name}") ?? throw new Exception("找不到外部存储目录");
#endif
            var fileInfo = new FileInfo(path);
            if (fileInfo.Directory is null || !fileInfo.Directory.Exists) Directory.CreateDirectory(fileInfo.Directory!.FullName);
            if (File.Exists(path)) File.Delete(path);
            using var fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            await stream.CopyToAsync(fileStream);
            await fileStream.FlushAsync();
            return new HttpResult<string> { IsSuccess = true, Data = path, Code = System.Net.HttpStatusCode.OK, Message = $"已保存在位置:{path}" };
        }
        catch (Exception ex)
        {
            return new HttpResult<string> { IsSuccess = false, Code = System.Net.HttpStatusCode.InternalServerError, Message = ex.Message };
        }
    }

    [JSInvokable]
    public static async Task<HttpResult<string>> HttpRequestAsync(JsParameter<RequestOptions> options)
    {
        if (options.Parameter!.Method == "get") return await GetAsync(options);
        else if (options.Parameter.Method == "post") return await PostAsync(options);
        else if (options.Parameter.Method == "put") return await PutAsync(options);
        else if (options.Parameter.Method == "delete") return await DeleteAsync(options);
        else return new HttpResult<string> { IsSuccess = false, Message = $"request '{options.Parameter.Method}' not supported" };
    }

    [JSInvokable]
    public static async Task<HttpResult<byte[]>> GetBlobAsync(JsParameter<RequestOptions> options)
    {
        var requestOptions = new ParameterOption(options.Parameter!.Url).SetOptions(options);
        return await App.ServiceProvider.GetRequiredService<IHttpService>().GetAsync<byte[]>(requestOptions);
    }

    static async Task<HttpResult<string>> GetAsync(JsParameter<RequestOptions> options)
    {
        var requestOptions = new ParameterOption(options.Parameter!.Url).SetOptions(options);
        return await App.ServiceProvider.GetRequiredService<IHttpService>().GetAsync<string>(requestOptions);
    }

    static async Task<HttpResult<string>> PostAsync(JsParameter<RequestOptions> options)
    {
        var requestOptions = new JsonOption(options.Parameter!.Url, options.Parameter.Data).SetOptions(options);
        return await App.ServiceProvider.GetRequiredService<IHttpService>().PostAsync<string>(requestOptions);
    }

    static async Task<HttpResult<string>> PutAsync(JsParameter<RequestOptions> options)
    {
        var requestOptions = new JsonOption(options.Parameter!.Url, options.Parameter.Data).SetOptions(options);
        return await App.ServiceProvider.GetRequiredService<IHttpService>().PutAsync<string>(requestOptions);
    }

    static async Task<HttpResult<string>> DeleteAsync(JsParameter<RequestOptions> options)
    {
        var requestOptions = new ParameterOption(options.Parameter!.Url).SetOptions(options);
        return await App.ServiceProvider.GetRequiredService<IHttpService>().DeleteAsync<string>(requestOptions);
    }

    static TOption SetOptions<TOption>(this TOption option, JsParameter<RequestOptions> options) where TOption : HttpOption
    {
        option.Headers = options.Parameter!.Headers.ToDictionary(x => x.Key, x => x.Value);
        if (options.ParameterRefercence is not null)
        {
            if (options.Parameter.Method == "post")
            {
                option.OnSendProgress = async (p) =>
                {
                    await options.ParameterRefercence.InvokeVoidAsync("setProgress", p);
                };
            }
            else
            {
                option.OnReceiveProgress = async (p) =>
                {
                    await options.ParameterRefercence.InvokeVoidAsync("setProgress", p);
                };
            }
        }
        if (options.Parameter.Timeout.HasValue && options.Parameter.Timeout > 0) option.TimeOut = TimeSpan.FromSeconds(options.Parameter.Timeout.Value);
        return option;
    }
}

public class RequestOptions
{
    public string Url { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
    public List<KeyValueDto> Headers { get; set; } = [];
    public string Name { get; set; } = string.Empty;
    public int? Timeout { get; set; }
}

public class KeyValueDto
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}