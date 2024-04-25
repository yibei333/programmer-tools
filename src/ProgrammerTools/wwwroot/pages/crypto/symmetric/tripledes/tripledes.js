export default {
    components: {
        labelinput: Vue.defineAsyncComponent(async () => await importComponent('labelinput')),
        tooltip: Vue.defineAsyncComponent(async () => await importComponent('tooltip')),
    },
    data() {
        return {
            request: {
                type: "txt",
                mode: 'CBC',
                padding: 'PKCS7',
                key: null,
                iv: '00000000',
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
                notifyWarning(this.$t('keyRequired'));
                return false;
            }
            if (this.request.mode != 'ECB' && (!this.request.iv || this.request.iv.length != 8)) {
                notifyWarning(this.$t('ivFormatError'));
                return false;
            }
            return true;
        },
        async encrypt() {
            if (!this.valid()) return;
            let data = await callService('TripleDesCryptoService.Encrypt', this.request);
            this.request.cipherText = data;
        },
        async decrypt() {
            if (!this.valid()) return;
            let data = await callService('TripleDesCryptoService.Decrypt', this.request);
            this.request.plainText = data;
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
        async filePicker() {
            let data = await callService('AppService.PickFiles');
            this.request.inputFiles = data;
            this.initInputFilesStatus();
        },
        async encryptFile() {
            if (!this.valid()) return;
            this.fileEncrypting = true;
            this.initInputFilesStatus();
            await callService('TripleDesCryptoService.EncryptFile', this.request, this);
            this.fileEncrypting = false;
            notifySuccess(this.$t('operateComplete'));
        },
        async decryptFile() {
            if (!this.valid()) return;
            this.fileEncrypting = true;
            this.initInputFilesStatus();
            await callService('TripleDesCryptoService.DecryptFile', this.request, this);
            this.fileEncrypting = false;
            notifySuccess(this.$t('operateComplete'));
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