const messages = {
    中文: {
        appName: '程序员工具',
        menus: {
            home: '主页',
            crypto: '加密 / 解密',
            symmetric: '对称',
            aes: 'AES',
            des: 'DES',
            asymmetric: '非对称',
            rsa: 'RSA'
        },
        message: {
            hello: '你好世界',
            checkUpdate:'检查更新'
        }
    },
    English: {
        appName: 'ProgrammerTools',
        menus: {
            home: 'Home',
            crypto: 'Encrypt / Decrypt',
            symmetric: 'Symmetric',
            aes: 'AES',
            des: 'DES',
            asymmetric: 'Asymmetric',
            rsa: 'RSA'
        },
        message: {
            hello: 'hello world',
            checkUpdate: 'check update'
        }
    },
};

const i18n = VueI18n.createI18n({
    locale: '中文',
    fallbackLocale: '中文',
    messages,
});