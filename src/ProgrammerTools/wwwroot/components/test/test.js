export default {
    methods: {
        async test(name) {
            let notify = new notification();
            notify.info(`test component clicked:${name}`);
            let result = await invokeSharpMethod('Test', 'jarvan');
            notify.info(result);
        }
    }
}