export default {
    data() {
        return {
        }
    },
    mounted() {
        setTimeout(() => {
            this.$router.push({ name: 'aes' });
        }, 20);
    },
    methods: {
        
    }
}