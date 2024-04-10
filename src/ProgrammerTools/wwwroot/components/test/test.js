export default {
    methods: {
        async test(name) {
            vant.showToast('ÌבÊ¾');
            let result = await invokeSharpMethod('Test', name);
            vant.showToast(result);
        }
    }
}