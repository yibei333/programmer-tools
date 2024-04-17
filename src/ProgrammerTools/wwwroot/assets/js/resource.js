const pagesConfig = [
    {
        id: 1,
        path: '/home',
        name: 'home',
        icon: 'ios-home-outline',
    },
    {
        id: 2,
        name: 'crypto',
        icon: 'md-key',
        children: [
            {
                id: 201,
                pid: 2,
                name: 'symmetric',
                children: [
                    {
                        id: 201001,
                        pid: 201,
                        path: '/crypto/symmetric/aes',
                        name: 'aes',
                    },
                    {
                        id: 201002,
                        pid: 201,
                        path: '/crypto/symmetric/des',
                        name: 'des',
                    }
                ]
            },
            {
                id: 202,
                pid: 2,
                name: 'asymmetric',
                children: [
                    {
                        id: 202001,
                        pid: 202,
                        path: '/crypto/asymmetric/rsa',
                        name: 'rsa',
                    }
                ]
            }
        ]
    }
];

function getVueRoutes() {
    let routes = [{ path: '/', redirect: '/home' }];
    for (let i = 0; i < pagesConfig.length; i++) {
        let config = pagesConfig[i];
        routes = routes.concat(getVueRoutesInternal(config));
    }
    routes.push({ path: '/:pathMatch(.*)*', name: 'notfound', component: () => importPage('notfound') });
    return routes;
}

function getVueRoutesInternal(current) {
    let routes = [];
    if (current.path) {
        routes.push({
            id: current.id,
            pid: current.pid,
            path: current.path,
            name: current.name,
            component: () => importPage(current.path),
        });
    } else if (current.children && current.children.length > 0) {
        for (let i = 0; i < current.children.length; i++) routes = routes.concat(getVueRoutesInternal(current.children[i]));
    }
    return routes;
}

function getPagesConfigList() {
    let result = [];
    for (let i = 0; i < pagesConfig.length; i++) {
        let config = pagesConfig[i];
        result = result.concat(getPagesConfigListInternal(config));
    }
    return result;
}

function getPagesConfigListInternal(config) {
    let result = [config];
    if (config.children && config.children.length > 0) {
        for (let i = 0; i < config.children.length; i++) result = result.concat(getPagesConfigListInternal(config.children[i]));
    }
    return result;
}

function getVueRouteParents(routes, id) {
    let route = routes.filter(x => x.id == id)[0];
    if (!route) return [];
    let result = [route];
    let pid = route.pid;
    let parent = routes.filter(x => x.id == pid)[0];
    while (pid && parent) {
        result.push(parent);
        pid = parent.pid;
        parent = routes.filter(x => x.id == pid)[0];
    }
    result.reverse();
    return result;
}

function getPageComponentNameByPath(path) {
    let array = path.split('/');
    return array[array.length - 1];
}

async function importPage(path) {
    let name = getPageComponentNameByPath(path);
    let html = await fetch(`/pages${path}/${name}.html`).then(r => r.text());
    let page = await import(`/pages${path}/${name}.js`);
    page.default.template = html;
    return page;
}

async function importComponent(path) {
    let name = getPageComponentNameByPath(path);
    let html = await fetch(`/components/${path}/${name}.html`).then(r => r.text());
    let component = await import(`/components/${path}/${name}.js`);
    component.default.template = html;
    return component;
}

