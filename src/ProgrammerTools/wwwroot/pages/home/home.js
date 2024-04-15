export default {
    mounted() {
    },
    methods: {
        test() {
            this.$Message.info('This is a info tip');
            //ElementPlus.ElMessage({
            //    dangerouslyUseHTMLString: true,
            //    message: '<h2>This is a message.</h2>',
            //    type: 'success'
            //})
        }
    }
}