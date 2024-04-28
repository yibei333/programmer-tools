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
            },
            panel2: {
                privateKey: null,
                publicKey: null,
                password: null
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
            let data = await callService('RsaKeyService.GenerateKeyPair', { type: this.panel1.type, length: this.panel1.length, password: this.panel1.password });
            this.panel1.pair = data;
        },
        async exportPublicKey() {
            let data = await callService('RsaKeyService.ExportPublicKey', { privateKey: this.panel2.privateKey, password: this.panel2.password });
            this.panel2.publicKey = data;
        },
        async matchKeyPair() {
            let data = await callService('RsaKeyService.MatchKeyPair', { privateKey: this.panel3.privateKey, publicKey: this.panel3.publicKey, password: this.panel3.password });
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
            this.panel4.target = data;
        }
    }
}