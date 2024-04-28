export default {
    components: {
        labelinput: Vue.defineAsyncComponent(async () => await importComponent('labelinput')),
        tooltip: Vue.defineAsyncComponent(async () => await importComponent('tooltip')),
    },
    data() {
        return {
            paddings: ['SHA1', 'SHA256', 'SHA384', 'SHA512', 'Pkcs1'],
            panel1: {
                padding: 'Pkcs1',
                key: null,
                text: null,
                inputFiles: [],
                cipherText: null,
            },
            panel2: {
                fileOrText: 'txt',
                padding: 'Pkcs1',
                key: null,
                text: null,
                inputFiles: [],
                plainText: null,
                handling: false,
            },
            panel3: {
                privateKey: null,
                publicKey: null,
                password: null,
                result: null
            },
            panel4: {
                source: null,
                currentType: null,
                target: null,
                targetTypes: [],
                targetType: null,
            },
            hexResult: null,
            hexResultShow: false,
        }
    },
    watch: {
        'panel4.source'() {
            this.convertSourceChanged();
        }
    },
    mounted() {
    },
    methods: {
        async encrypt() {
            let data = await callService('RsaCryptoService.Encrypt', this.panel1);
            this.panel1.cipherText = data;
        },
        async decrypt() {
            let data = await callService('RsaCryptoService.Decrypt', this.panel2);
            this.panel2.plainText = data;
        },
        async copy(text) {
            await callService('AppService.SetClipboard', text);
            notifySuccess(this.$t('copySuccess'));
        },
        async convertToHex(text) {
            let data = await callService('EncodeService.ConvertBase64ToHext', text);
            this.hexResult = data;
            this.hexResultShow = true;
        },
    }
}