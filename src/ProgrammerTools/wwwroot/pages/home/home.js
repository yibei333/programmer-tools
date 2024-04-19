export default {
    data() {
        return {
            message: '',
            files: []
        }
    },
    mounted() {
    },
    methods: {
        test() {
            this.$Message.info('This is a info tip');
            this.message = this.$t('message.currentVersion');
        }
    }
}