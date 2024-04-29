export default {
    components: {
        labelinput: Vue.defineAsyncComponent(async () => await importComponent('labelinput')),
        tooltip: Vue.defineAsyncComponent(async () => await importComponent('tooltip')),
    },
    data() {
        return {
            paddings: ['Pkcs1', 'OaepSHA1', 'OaepSHA256', 'OaepSHA384', 'OaepSHA512'],
            signPaddings: ['Pkcs1', 'Pss'],
            algorithms: ['MD5', 'SHA1', 'SHA256', 'SHA384', 'SHA512'],
            panel1: {
                padding: 'Pkcs1',
                key: null,
                text: null,
                inputFiles: [],
                cipherText: null,
            },
            panel2: {
                padding: 'Pkcs1',
                key: null,
                hasPassword: false,
                password: null,
                text: null,
                plainText: null,
            },
            panel3: {
                key: null,
                hasPassword: false,
                password: null,
                text: null,
                padding: 'Pkcs1',
                signHashAlgorithm: 'SHA256',
                signature: null
            },
            panel4: {
                key: null,
                text: null,
                padding: 'Pkcs1',
                signHashAlgorithm: 'SHA256',
                signature: null,
                result: null
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
        async panel2PrivateKeyChanged() {
            if (!this.panel2.key) {
                this.panel2.hasPassword = false;
                return;
            }
            let data = await callService('RsaKeyService.IsKeyHasPassword', this.panel2.key);
            this.panel2.hasPassword = data;
        },
        async panel3PrivateKeyChanged() {
            this.panel3.signature = null;
            if (!this.panel3.key) {
                this.panel3.hasPassword = false;
                return;
            }
            let data = await callService('RsaKeyService.IsKeyHasPassword', this.panel3.key);
            this.panel3.hasPassword = data;
        },
        async sign() {
            let data = await callService('RsaCryptoService.Sign', this.panel3);
            this.panel3.signature = data;
        },
        async verifySign() {
            let data = await callService('RsaCryptoService.VerifySign', this.panel4);
            this.panel4.result = data;
        }
    }
}