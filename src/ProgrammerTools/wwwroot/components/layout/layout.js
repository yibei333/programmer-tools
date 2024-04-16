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
            currentTabName: '',
            moveStartTab: null,
        }
    },
    watch: {
        $route(to) {
            let route = this.pagesConfigList.filter(x => x.name == to?.name)[0];
            if (route) {
                this.currentMenuId = route.id;
                this.openMenuArray = getVueRouteParents(this.pagesConfigList, route.id).map(x => x.id);
                if (this.tabList.indexOf(route) < 0) {
                    this.tabList.push(route);
                    this.setMenuTabEvents();
                }
                this.currentTabName = route.name;
                let timer = setTimeout(() => {
                    clearTimeout(timer);
                    this.$refs.menu.updateActiveName();
                    this.$refs.menu.updateOpened();
                }, 300);
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
            if (!tab) return;
            this.tabList.remove(tab);
            this.setMenuTabEvents();
        },
        handleDragDrop(startName, endName, startIndex, endIndex, names) {
            if (!this.moveStartTab) {
                console.log(startName, endName, startIndex, endIndex, names);
                return;
            }
            startIndex = this.tabList.indexOf(this.moveStartTab);//name and a always be '' and -1
            this.tabList.splice(endIndex, 1, ...this.tabList.splice(startIndex, 1, this.tabList[endIndex]));
            this.setMenuTabEvents();
            this.moveStartTab = null;
        },
        setMenuTabEvents() {
            let timer = setTimeout(() => {
                clearTimeout(timer);
                let tabs = document.querySelectorAll(".page-layout-route-tabs .ivu-tabs-tab") || [];
                let panels = document.querySelectorAll(".page-layout-route-tabs .ivu-tabs-tabpane") || [];

                for (let i = 0; i < tabs.length; i++) {
                    let tab = tabs[i];
                    let panel = (panels.length > i) ? panels[i] : null;
                    tab.id = panel?.getAttribute("data-id");
                    tab.removeEventListener('dragstart', this.setMenuTabDragStartEvents);
                    tab.addEventListener('dragstart', this.setMenuTabDragStartEvents);
                }
            }, 100);
        },
        setMenuTabDragStartEvents(event) {
            let id = event.currentTarget.id;
            if (!id) {
                this.moveStartTab = null;
                return;
            }
            this.moveStartTab = this.tabList.filter(x => x.id == id)[0];
        },
    }
}