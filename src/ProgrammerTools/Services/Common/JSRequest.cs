using Microsoft.JSInterop;

namespace ProgrammerTools.Services.Common;

public class InvokeRequest
{
    public required string Signature { get; set; }
    public IJSObjectReference? JSObjectReference { get; set; }
    public string? Json { get; set; }
}

public class JSRequest(IJSObjectReference? jSObjectReference)
{
    public IJSObjectReference? JSObjectReference { get; } = jSObjectReference;
}

public class JSRequest<T>(T parameter, IJSObjectReference? jSObjectReference) : JSRequest(jSObjectReference)
{
    public T Parameter { get; } = parameter;
}