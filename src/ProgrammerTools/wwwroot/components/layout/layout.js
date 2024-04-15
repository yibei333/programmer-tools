export default {
    components: {
        menuitem: Vue.defineAsyncComponent(async () => await importComponent('menuitem')),
    },
    data() {
        return {
            isCollapse: false,
            activedId: "1",
            routes: [],
            menus: [],
        }
    },
    mounted() {
        this.menus = pagesConfig;
        this.routes = getVueRotes();
    },
    methods: {
        test(name) {
            this.$router.push({ name: name, replace: true });
        },
        handleSelect(id) {
            let route = this.routes.filter(x => x.id == id);
            let name = (route && route.length > 0) ? route[0].name : null;
            if (name) {
                this.$router.push({ name: name });
            }
        },
        selectLocale(command) {
            this.$i18n.locale = command;
        },
        openGithub() {
            invokeSharpMethod('OpenBrowser', 'https://github.com/yibei333/programmer-tools');
        }
    }
}