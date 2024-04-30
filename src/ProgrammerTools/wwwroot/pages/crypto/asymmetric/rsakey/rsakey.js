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
                pair: {
                    private: null,
                    public: null
                }
            },
            panel2: {
                hasPassword: false,
                privateKey: null,
                publicKey: null,
                password: null
            },
            panel3: {
                hasPassword: false,
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
            panel5: {
                hasPassword: false,
                source: null,
                password: null,
                currentType: null,
                target: null,
            }
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
        async copy(text) {
            await callService('AppService.SetClipboard', text);
            notifySuccess(this.$t('copySuccess'));
        },
        async generateKeyPair() {
            let data = await callService('RsaKeyService.GenerateKeyPair', { type: this.panel1.type, length: this.panel1.length });
            notifySuccess(this.$t('success'));
            this.panel1.pair = data;
        },
        async exportPublicKey() {
            if (this.panel2.hasPassword && !this.panel2.password) {
                notifyWarning(this.$t('passwordRequired'));
                return;
            }
            let data = await callService('RsaKeyService.ExportPublicKey', { privateKey: this.panel2.privateKey, password: this.panel2.password });
            notifySuccess(this.$t('success'));
            this.panel2.publicKey = data;
        },
        async matchKeyPair() {
            if (this.panel3.hasPassword && !this.panel3.password) {
                notifyWarning(this.$t('passwordRequired'));
                return;
            }
            let data = await callService('RsaKeyService.MatchKeyPair', { privateKey: this.panel3.privateKey, publicKey: this.panel3.publicKey, password: this.panel3.password });
            if (data) notifySuccess(this.$t('success'));
            else notifyWarning(this.$t('fail'));
            this.panel3.result = data;
        },
        async convertSourceChanged() {
            this.panel4.target = null;
            this.panel4.targetType = null;
            this.panel4.targetTypes = [];
            if (!this.panel4.source || this.panel4.source == '') {
                this.panel4.currentType = null;
                return;
            }

            let data = await callService('RsaKeyService.GetKeyType', this.panel4.source);
            this.panel4.currentType = data;
            if (data == null || data == 'UnKnown') return;

            if (data == "Pkcs1") this.panel4.targetTypes = ['Xml', 'Pkcs8'];
            else if (data == "Pkcs8") this.panel4.targetTypes = ['Xml', 'Pkcs1'];
            else if (data == "Xml") this.panel4.targetTypes = ['Pkcs1', 'Pkcs8'];
            else if (data == "PemPublic") this.panel4.targetTypes = ['Xml'];
            else notifyError(`not surppot type '${data}'`);
        },
        async convert() {
            let data = await callService('RsaKeyService.Convert', { source: this.panel4.source, targetType: this.panel4.targetType });
            notifySuccess(this.$t('success'));
            this.panel4.target = data;
        },
        async panel2PrivateKeyChanged() {
            let data = await callService('RsaKeyService.IsKeyHasPassword', this.panel2.privateKey);
            this.panel2.hasPassword = data;
        },
        async panel3PrivateKeyChanged() {
            this.panel3.result = null
            let data = await callService('RsaKeyService.IsKeyHasPassword', this.panel3.privateKey);
            this.panel3.hasPassword = data;
        },
        async panel5PrivateKeyChanged() {
            if (!this.panel5.source) {
                this.panel5.type = null;
                this.panel5.hasPassword = false;
                return;
            }
            let type = await callService('RsaKeyService.GetKeyType', this.panel5.source);
            this.panel5.currentType = type;
            let data = await callService('RsaKeyService.IsKeyHasPassword', this.panel5.source);
            this.panel5.hasPassword = data;
        },
        async addPassword() {
            if (!this.veriryPanel5()) return;
            let data = await callService('RsaKeyService.AddPassword', this.panel5);
            notifySuccess(this.$t('success'));
            this.panel5.target = data;
        },
        async removePassword() {
            if (!this.veriryPanel5()) return;
            let data = await callService('RsaKeyService.RemovePassword', this.panel5);
            notifySuccess(this.$t('success'));
            this.panel5.target = data;
        },
        veriryPanel5() {
            if (this.panel5.currentType != 'Pkcs1' && this.panel5.currentType != "Pkcs8") {
                notifyWarning(this.$t('xml not supported'));
                return false;
            }
            if (!this.panel5.password) {
                notifyWarning(this.$t('passwordRequired'));
                return false;
            }
            return true;
        }
    }
}