export default {
    methods: {
        async test(name) {
            ElementPlus.ElMessage('提示');
            let result = await invokeSharpMethod('Test', name);
            ElementPlus.ElMessage(result);
        }
    }
}