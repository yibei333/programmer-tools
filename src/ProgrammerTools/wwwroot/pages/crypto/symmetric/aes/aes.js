export default {
    data() {
        return {
            request: {
                type: "txt",
                mode: 'CBC',
                padding: 'PKCS7',
                key: '',
                iv: '0000000000000000',
                plainText: null,
                cipherText: null,
            },
            types: ['txt', 'file'],
            modes: ['CBC', 'ECB', 'OFB', 'CFB', 'CTS'],
            paddings: ['None', 'PKCS7', 'Zeros', 'ANSIX923', 'ISO10126'],
            hexResult: null,
            hexResultShow: false,
        }
    },
    mounted() {

    },
    methods: {
        async encrypt() {
            let result = await invokeSharpMethod('Encrypt', this.request);
            if (result.success) {
                this.request.cipherText = result.data;
            } else this.$Message.error(result.description);
        },
        async decrypt() {
            let result = await invokeSharpMethod('Decrypt', this.request);
            if (result.success) {
                this.request.plainText = result.data;
            } else this.$Message.error(result.description);
        },
        async copy(text) {
            let result = await invokeSharpMethod('SetClipboardAsync', text);
            if (result.success) this.$Message.success(this.$t('message.copySuccess'));
            else this.$Message.error(result.description);
        },
        async convertToHex(text) {
            let result = await invokeSharpMethod('ConvertBase64ToHext', text);
            if (result.success) {
                this.hexResult = result.data;
                this.hexResultShow = true;
            }
            else this.$Message.error(result.description);
        }
    }
}