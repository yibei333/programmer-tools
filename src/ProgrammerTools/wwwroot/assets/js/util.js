const platforms = [
    { key: 'android', value: 1 },
    { key: 'ios', value: 2 },
    { key: 'iphone', value: 2 },
    { key: 'windows', value: 3 },
    { key: 'mac os', value: 4 },
    { key: 'linux', value: 5 },
];

function getPlatform() {
    let ua = navigator.userAgent.toLowerCase();
    let platform = platforms.filter(x => ua.indexOf(x.key) > -1)[0];
    return platform?.value ?? 0;
}

function invokeSharpMethod(name, parameter, parameterRefercence) {
    return DotNet.invokeMethodAsync("ProgrammerTools", name, {
        parameterRefercence: parameterRefercence ? DotNet.createJSObjectReference(parameterRefercence) : null,
        parameter: parameter
    });
}

function callService(signature, request, objectRefercence) {
    return DotNet.invokeMethodAsync("ProgrammerTools", "Invoke", {
        signature: signature,
        jSObjectReference: objectRefercence ? DotNet.createJSObjectReference(objectRefercence) : null,
        json: request ? (request instanceof Object ? JSON.stringify(request) : request) : null
    });
}

Array.prototype.remove = function (item) {
    let targetIndex = this.findIndex(x => x === item);
    if (targetIndex < 0) return;
    this.splice(targetIndex, 1);
}