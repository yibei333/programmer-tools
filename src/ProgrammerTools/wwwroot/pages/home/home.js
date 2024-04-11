export default {
    mounted() {
    },
    methods: {
        test() {
            ElementPlus.ElMessage({
                dangerouslyUseHTMLString: true,
                message: '<h2>This is a message.</h2>',
                type: 'success'
            })
        }
    }
}