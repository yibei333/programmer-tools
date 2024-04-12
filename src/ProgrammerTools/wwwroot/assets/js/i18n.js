const messages = {
    English: {
        message: {
            hello: 'hello world'
        }
    },
    中文: {
        message: {
            hello: '你好世界'
        }
    }
};

const i18n = VueI18n.createI18n({
    locale: 'English',
    fallbackLocale: 'English',
    messages,
})