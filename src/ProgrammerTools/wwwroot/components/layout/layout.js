export default {
    components: {
        menuitem: Vue.defineAsyncComponent(async () => await importComponent('menuitem')),
    },
    data() {
        return {
            isCollapse: false,
            pagesConfigList: [],
            menus: [],
            currentMenuId: 1,
            openMenuArray: [],
            isSmall: false
        }
    },
    watch: {
        $route(to) {
            let id = this.pagesConfigList.filter(x => x.name == to?.name)[0]?.id;
            if (id) {
                this.currentMenuId = id;
                this.openMenuArray = getVueRouteParents(this.pagesConfigList, id).map(x => x.id);
                let timer = setTimeout(() => {
                    clearTimeout(timer);
                    this.$refs.menu.updateActiveName();
                    this.$refs.menu.updateOpened();
                }, 200);
            }
        },
        "$root.size"() {
            this.isSmall = this.$root.size == 1;
        }
    },
    mounted() {
        this.isSmall = this.$root.size == 1;
        this.menus = pagesConfig;
        this.pagesConfigList = getPagesConfigList();
    },
    methods: {
        nav(id) {
            let name = this.pagesConfigList.filter(x => x.id == id)[0]?.name;
            if (name) {
                this.$router.push({ name: name });
            }
        },
        selectLocale(command) {
            this.$i18n.locale = command;
        },
        openGithub() {
            invokeSharpMethod('OpenBrowser', 'https://github.com/yibei333/programmer-tools');
        },
    }
}