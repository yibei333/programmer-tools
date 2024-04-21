export default {
    components: {
        menuitem: Vue.defineAsyncComponent(async () => await importComponent('menuitem')),
    },
    data() {
        return {
            themes: ['dark', 'light'],
            theme: 'light',
            isMenuOpen: false,
            pagesConfigList: [],
            menus: [],
            currentMenuId: 1,
            openMenuArray: [],
            isSmall: false,
            tabList: [],
            currentTabName: '',
            moveStartTab: null,
            currentVersion: {},
            showVersionModal: false,
            lastVersion: null,
            checking: false,
            upgrading: false,
            progress: null,
            installing: false,
            packagePath: null
        }
    },
    watch: {
        $route(to) {
            this.updateTabs(to?.name);
        },
        "$root.size"() {
            this.setSize();
        },
        currentTabName() {
            this.$router.push({ name: this.currentTabName });
        },
        theme() {
            this.setTheme();
        }
    },
    mounted() {
        this.setTheme();
        this.setSize();
        this.menus = pagesConfig;
        this.pagesConfigList = getPagesConfigList();
        this.updateTabs('home');
        this.getCurrentVersion();
    },
    methods: {
        setTheme() {
            let existed = document.querySelector('link[href*="/assets/css/theme-"]');
            if (existed) document.head.removeChild(existed);
            let link = document.createElement('link');
            link.rel = 'stylesheet';
            link.href = `/assets/css/theme-${this.theme}.css`;
            document.head.appendChild(link);
        },
        selectTheme(theme) {
            this.theme = theme;
        },
        updateTabs(name) {
            let route = this.pagesConfigList.filter(x => x.name == name)[0];
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
        nav(id) {
            let name = this.pagesConfigList.filter(x => x.id == id)[0]?.name;
            if (name) {
                this.$router.push({ name: name });
                if (this.isSmall) this.isMenuOpen = false;
            }
        },
        selectLocale(locale) {
            this.$i18n.locale = locale;
        },
        openGithub() {
            invokeSharpMethod('OpenBrowserAsync', staticConfigs.githubUrl);
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
        getCurrentVersion() {
            invokeSharpMethod('GetAppInfoAsync').then(res => {
                this.currentVersion = res;
            });
        },
        showVersion() {
            this.showVersionModal = true;
            if (!this.lastVersion) this.checkUpdate();
        },
        checkUpdate() {
            this.checking = true;
            var options = {
                url: staticConfigs.giteeUrl + '/raw/main/pack/version.txt',
                method: 'get',
            };
            invokeSharpMethod('HttpRequestAsync', options).then(res => {
                if (res.isSuccess) {
                    this.checkComplete(res);
                    return;
                }

                options.url = staticConfigs.githubRawUrl + '/main/pack/version.txt';
                invokeSharpMethod('HttpRequestAsync', options).then(res => this.checkComplete(res));
            });
        },
        checkComplete(response) {
            this.lastVersion = response.isSuccess ? response.data : null;
            this.checking = false;
            if (response.isSuccess) {
                if (this.lastVersion == this.currentVersion.version) this.$Message.info(this.$t('message.alreadyUptodate'));
                else this.$Message.info(this.$t('message.newVersionDetected'));
            }
            else this.$Message.error(response.message);
        },
        upgrade() {
            if (!this.lastVersion || this.lastVersion == this.currentVersion.version) return;
            if (this.packagePath) {
                this.upgradeComplete({ isSuccess: true, data: this.packagePath });
                return;
            }
            this.upgrading = true;

            let name = this.currentVersion.platform == 'android' ? `ProgrammerTools.android.${this.lastVersion}.apk` : `ProgrammerTools.win64.${this.lastVersion}.exe`;
            var options = {
                url: staticConfigs.giteeUrl + `/releases/download/${this.lastVersion}/${name}`,
                method: 'get',
                name: name
            };
            invokeSharpMethod('DownloadAsync', options, this).then(res => {
                if (res.isSuccess) {
                    this.$Message.success(`${this.$t('message.installPackageSavedAt')}:${res.data}`);
                    this.upgradeComplete(res);
                    return;
                }

                options.url = staticConfigs.githubUrl + `/releases/download/${this.lastVersion}/${name}`;
                invokeSharpMethod('DownloadAsync', options, this).then(res => {
                    if (res.isSuccess) this.$Message.success(`${this.$t('message.installPackageSavedAt')}:${res.data}`);
                    this.upgradeComplete(res);
                });
            });
        },
        setProgress(p) {
            this.progress = p;
        },
        upgradeComplete(response) {
            this.progress = null;
            this.upgrading = false;
            if (response.isSuccess) {
                this.packagePath = response.data;
                this.installUpdate();
            }
            else this.$Message.error(`${this.$t('message.downloadFailed')}:${response.message}`);
        },
        installUpdate() {
            if (!this.packagePath) return;
            this.installing = true;
            invokeSharpMethod('Upgrade', this.packagePath, this);
        },
        notifyInstallUpdate(res) {
            this.installing = false;
            if (!res.success) this.$Message.error(res.description);
        }
    }
}