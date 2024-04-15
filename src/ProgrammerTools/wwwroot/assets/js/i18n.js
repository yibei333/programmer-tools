const messages = {
    中文: {
        appName:'程序员工具',
        menus: {
            home: '主页',
            crypto: '加密/解密',
            symmetric: '对称',
            aes: 'aes',
            des: 'des',
            asymmetric: '非对称',
            rsa: 'rsa'
        },
        message: {
            hello: '你好世界'
        }
    },
    English: {
        appName: 'ProgrammerTools',
        menus: {
            home: 'home',
            crypto: 'encrypt/decrypt',
            symmetric: 'symmetric',
            aes: 'aes',
            des: 'des',
            asymmetric: 'asymmetric',
            rsa:'rsa'
        },
        message: {
            hello: 'hello world'
        }
    },
};

const i18n = VueI18n.createI18n({
    locale: '中文',
    fallbackLocale: '中文',
    messages,
})