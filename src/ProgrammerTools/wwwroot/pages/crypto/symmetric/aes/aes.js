export default {
    data() {
        return {
            request: {
                type: "file",
                mode: 'CBC',
                padding: 'PKCS7',
                key: null,
                iv: '0000000000000000',
                plainText: null,
                cipherText: null,
                inputFiles: []
            },
            types: ['txt', 'file'],
            modes: ['CBC', 'ECB', 'OFB', 'CFB', 'CTS'],
            paddings: ['None', 'PKCS7', 'Zeros', 'ANSIX923', 'ISO10126'],
            hexResult: null,
            hexResultShow: false,
            fileEncrypting: false,
        }
    },
    mounted() {

    },
    methods: {
        valid() {
            if (!this.request.key) {
                this.$Message.warning(this.$t('message.keyRequired'));
                return false;
            }
            if (this.request.mode != 'ECB' && (!this.request.iv || this.request.iv.length != 16)) {
                this.$Message.warning(this.$t('message.ivFormatError'));
                return false;
            }
            return true;
        },
        async encrypt() {
            if (!this.valid()) return;
            let result = await callService('AesCryptoService.AesEncrypt', this.request);
            if (result.success) this.request.cipherText = result.data;
            else this.$Message.error(result.description);
        },
        async decrypt() {
            if (!this.valid()) return;
            let result = await callService('AesCryptoService.AesDecrypt', this.request);
            if (result.success) this.request.plainText = result.data;
            else this.$Message.error(result.description);
        },
        async copy(text) {
            let result = await callService('AppService.SetClipboard', text);
            if (result.success) this.$Message.success(this.$t('message.copySuccess'));
            else this.$Message.error(result.description);
        },
        async convertToHex(text) {
            let result = await callService('EncodeService.ConvertBase64ToHext', text);
            if (result.success) {
                this.hexResult = result.data;
                this.hexResultShow = true;
            }
            else this.$Message.error(result.description);
        },
        async filePicker() {
            let result = await callService('AppService.PickFiles');
            if (result.success) {
                this.request.inputFiles = result.data;
                this.initInputFilesStatus();
            }
            else this.$Message.error(result.description);
        },
        async encryptFile() {
            if (!this.valid()) return;
            this.fileEncrypting = true;
            this.initInputFilesStatus();
            let result = await callService('AesCryptoService.AesEncryptFile', this.request, this);
            this.fileEncrypting = false;
            if (result.success) {
                this.$Message.success(this.$t('message.operateComplete'));
            } else this.$Message.error(result.description);
        },
        async decryptFile() {
            if (!this.valid()) return;
            this.fileEncrypting = true;
            this.initInputFilesStatus();
            let result = await callService('AesCryptoService.AesDecryptFile', this.request, this);
            this.fileEncrypting = false;
            if (result.success) {
                this.$Message.success(this.$t('message.operateComplete'));
            } else this.$Message.error(result.description);
        },
        initInputFilesStatus() {
            this.request.inputFiles.forEach(x => {
                x.status = 0;
                x.data = null;
            });
        },
        setInputFileStatus(name, status, data) {
            let file = this.request.inputFiles.filter(x => x.fullName == name)[0];
            if (!file) return;
            file.status = status;
            file.data = data;
        }
    }
}