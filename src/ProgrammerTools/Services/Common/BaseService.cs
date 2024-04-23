using Microsoft.JSInterop;
using Newtonsoft.Json;
using SharpDevLib;
using SharpDevLib.Extensions.Http;
using SharpDevLib.Extensions.Model;
using System.Reflection;

namespace ProgrammerTools.Services.Common;

public class BaseService
{
    static object _locker = new();
    static List<Type>? _services;
    static List<Type> Services
    {
        get
        {
            lock (_locker)
            {
                if (_services is not null) return _services;

                var baseType = typeof(BaseService);
                _services = baseType.Assembly.GetTypes().Where(x => x.BaseType == baseType && x.IsPublic).ToList();
                return _services;
            }
        }
    }
    static Dictionary<string, object?> _serviceInstances = [];
    static object GetServiceInstance(string serviceName)
    {
        object? instance;
        if (_serviceInstances.TryGetValue(serviceName, out object? value)) instance = value;
        else
        {
            var serviceType = Services.FirstOrDefault(x => x.Name == serviceName);
            instance = serviceType is null ? null : Activator.CreateInstance(serviceType);
            _serviceInstances.Add(serviceName, instance);
        }
        return instance ?? throw new Exception($"can't find service of name '{serviceName}'");
    }
    static Dictionary<string, MethodInfo?> _methods = [];
    static MethodInfo GetMethodInfo(string serviceName, string methodName)
    {
        var signature = $"{serviceName}.{methodName}";
        MethodInfo? methodInfo;
        if (_methods.TryGetValue(signature, out MethodInfo? value)) methodInfo = value;
        else
        {
            var serviceType = Services.FirstOrDefault(x => x.Name == serviceName) ?? throw new Exception($"can't find service of name '{serviceName}'");
            methodInfo = serviceType.GetMethod(methodName);
            _methods.Add(signature, methodInfo);
        }
        return methodInfo ?? throw new Exception($"can't find method of name '{methodName}'");
    }

    [JSInvokable]
    public static async Task<Result<object?>> Invoke(InvokeRequest invokeRequest)
    {
        try
        {
            var signatureArray = invokeRequest.Signature.SplitToList(false, ['.']);
            if (signatureArray.IsEmpty() || signatureArray.Count != 2) throw new Exception($"signature format error");
            var instance = GetServiceInstance(signatureArray[0]);
            var method = GetMethodInfo(signatureArray[0], signatureArray[1]);
            object? request = null;
            if (string.IsNullOrWhiteSpace(invokeRequest.Json))
            {
                if (invokeRequest.JSObjectReference is not null) request = new JSRequest(invokeRequest.JSObjectReference);
            }
            else
            {
                var parameters = method.GetParameters();
                if (parameters.IsEmpty()) throw new Exception("parameter required");
                var parameterInfo = parameters[0];
                var arguments = parameterInfo.ParameterType.GetGenericArguments();
                if (arguments.IsEmpty()) throw new Exception("parameter sholud be generic");
                var argumentType = arguments[0];
                var requestData = (argumentType.IsClass && argumentType != typeof(string)) ? JsonConvert.DeserializeObject(invokeRequest.Json, argumentType) : Convert.ChangeType(invokeRequest.Json, argumentType);
                var requestType = typeof(JSRequest<>).MakeGenericType(argumentType);
                request = Activator.CreateInstance(requestType, [requestData, invokeRequest.JSObjectReference])!;
            }
            var result = request is null ? method.Invoke(instance, []) : method.Invoke(instance, [request]);
            if (result is not Task task) throw new Exception("result should be task");
            if (!result.GetType().IsGenericType)
            {
                await task;
                return Result.Succeed<object?>(null);
            }
            else
            {
                await task.ConfigureAwait(false);
                var resultProperty = task.GetType().GetProperty("Result");
                var resultData = resultProperty?.GetValue(task);
                return Result.Succeed(resultData);
            }
        }
        catch (Exception ex)
        {
            App.Logger.Error(ex);
            return Result.Failed<object?>(ex.Message);
        }
    }
}