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

async function invokeSharpMethod(name, parameter, hasParameterRefercence = false) {
    return await DotNet.invokeMethodAsync("ProgrammerTools", name, {
        parameterRefercence: hasParameterRefercence ? DotNet.createJSObjectReference(parameter) : null,
        parameter: parameter
    });
}

Array.prototype.remove = function (item) {
    let targetIndex = this.findIndex(x => x === item);
    if (targetIndex < 0) return;
    this.splice(targetIndex, 1);
}