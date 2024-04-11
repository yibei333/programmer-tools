export default {
    data() {
        return {
            isCollapse: false,
            activedId: "1",
            routes: [],
            menus: [],
            tabs: [
                {
                    label: '主页',
                    key: '1',
                },
            ],
        }
    },
    mounted() {
        this.menus = pagesConfig;
        this.routes = getVueRotes();
    },
    methods: {
        test(name) {
            this.$router.push({ name: name });
        },
        handleSelect(id) {
            let route = this.routes.filter(x => x.id == id);
            let name = (route && route.length > 0) ? route[0].name : null;
            if (name) {
                this.$router.push({ name: name });
            }
        }
    }
}