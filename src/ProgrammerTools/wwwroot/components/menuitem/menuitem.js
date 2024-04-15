export default {
    props: ["item", "isCollapse"],
    components: {
        menuitem: Vue.defineAsyncComponent(async () => await importComponent('menuitem')),
    },
    methods: {
        upper(value) {
            return value.toUpperCase();
        }
    }
}