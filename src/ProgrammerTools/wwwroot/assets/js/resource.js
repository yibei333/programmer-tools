const staticImages = {
    home: 'data:image/png;base64,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',
    key: 'data:image/png;base64,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',
};

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

