export default {
    data() {
        return {
            message: '',
            files: []
        }
    },
    mounted() {
        this.$router.push({ name: 'aes' });
    },
    methods: {
        test() {
            this.$Message.info('This is a info tip');
            this.message = this.$t('message.currentVersion');
        }
    }
}