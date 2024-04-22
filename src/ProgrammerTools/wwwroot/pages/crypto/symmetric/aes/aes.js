export default {
    data() {
        return {
            request: {
                type: "txt",
                mode: 'CBC',
                padding: 'PKCS7',
                key: '',
                iv: '0000000000000000',
                result: {
                    type: 'base64',
                    case: 'lower'
                },
                plainText: null,
                cipherText: null,
            },
            types: ['txt', 'file'],
            modes: ['CBC', 'ECB', 'OFB', 'CFB', 'CTS'],
            paddings: ['None', 'PKCS7', 'Zeros', 'ANSIX923', 'ISO10126'],
            resultTypes: ['base64', 'hex'],
            resultCases: ['lower', 'upper'],
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
        }
    }
}