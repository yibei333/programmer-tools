export default {
    props: {
        mask: {
            type: Boolean,
            default: true
        },
        fill: {
            type: String,
            default: 'var(--purple)'
        },
    },
    mounted() {
        let parent = this.$refs.loading.parentElement;
        if (parent) {
            parent.style.position = 'relative';
        }
    }
}