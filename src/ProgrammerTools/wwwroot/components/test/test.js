export default {
    methods: {
        async test(name) {
            vant.showToast('��ʾ');
            let result = await invokeSharpMethod('Test', name);
            vant.showToast(result);
        }
    }
}