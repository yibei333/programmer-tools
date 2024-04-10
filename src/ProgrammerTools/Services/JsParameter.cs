using Microsoft.JSInterop;

namespace ProgrammerTools.Services;

public class JsParameter<T>
{
    public IJSObjectReference? ParameterRefercence { get; set; }
    public T? Parameter { get; set; }
}