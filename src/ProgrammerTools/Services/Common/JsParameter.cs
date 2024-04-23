using Microsoft.JSInterop;

namespace ProgrammerTools.Services.Common;

public class JsParameter<T>
{
    public IJSObjectReference? ParameterRefercence { get; set; }
    public T? Parameter { get; set; }
}