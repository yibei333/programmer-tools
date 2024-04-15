export default {
    components: {
        menuitem: Vue.defineAsyncComponent(async () => await importComponent('menuitem')),
    },
    data() {
        return {
            isMenuOpen: false,
            pagesConfigList: [],
            menus: [],
            currentMenuId: 1,
            openMenuArray: [],
            isSmall: false,
            tabList: [],
            currentTabName: ''
        }
    },
    watch: {
        $route(to) {
            let route = this.pagesConfigList.filter(x => x.name == to?.name)[0];
            if (route) {
                this.currentMenuId = route.id;
                this.openMenuArray = getVueRouteParents(this.pagesConfigList, route.id).map(x => x.id);
                if (this.tabList.indexOf(route) < 0) this.tabList.push(route);
                this.currentTabName = route.name;
                let timer = setTimeout(() => {
                    clearTimeout(timer);
                    this.$refs.menu.updateActiveName();
                    this.$refs.menu.updateOpened();
                }, 200);
            }
        },
        "$root.size"() {
            this.setSize();
        },
        currentTabName() {
            this.$router.push({ name: this.currentTabName });
        }
    },
    mounted() {
        this.setSize();
        this.menus = pagesConfig;
        this.pagesConfigList = getPagesConfigList();
    },
    methods: {
        nav(id) {
            let name = this.pagesConfigList.filter(x => x.id == id)[0]?.name;
            if (name) {
                this.$router.push({ name: name });
                if (this.isSmall) this.isMenuOpen = false;
            }
        },
        selectLocale(command) {
            this.$i18n.locale = command;
        },
        openGithub() {
            invokeSharpMethod('OpenBrowser', 'https://github.com/yibei333/programmer-tools');
        },
        setSize() {
            this.isSmall = this.$root.size == 1;
            this.isMenuOpen = !this.isSmall;
        },
        handleTabRemove(name) {
            let tab = this.tabList.filter(x => x.name == name)[0];
            if (tab) this.tabList.remove(tab);
        },
        handleDragDrop(name, newName, a, b, names) {
            this.tabList.splice(b, 1, ...this.tabList.splice(a, 1, this.tabList[b]));
        }
    }
}