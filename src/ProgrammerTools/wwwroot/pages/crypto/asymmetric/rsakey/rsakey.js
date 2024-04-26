export default {
    components: {
        labelinput: Vue.defineAsyncComponent(async () => await importComponent('labelinput')),
        tooltip: Vue.defineAsyncComponent(async () => await importComponent('tooltip')),
    },
    data() {
        return {
            panel1: {
                types: ['Pkcs1', 'Pkcs8'],
                type: 'Pkcs1',
                lengths: [512, 1024, 2048, 3072, 4096],
                length: 2048,
                password: null,
                pair: {
                    private: null,
                    public: null
                }
            }
        }
    },
    mounted() {
    },
    methods: {
        async generateKeyPair() {
            let data = await callService('RsaKeyService.GenerateKeyPair', { type: this.panel1.type, length: this.panel1.length, password: this.panel1.password });
            this.panel1.pair = data;
        },
        async copy(text) {
            await callService('AppService.SetClipboard', text);
            notifySuccess(this.$t('copySuccess'));
        },
    }
}