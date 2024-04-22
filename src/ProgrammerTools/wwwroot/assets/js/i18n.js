﻿const i18nMessages = {
    中文: {
        appName: '程序员工具',
        menus: {
            home: '主页',
            crypto: '加密 / 解密',
            symmetric: '对称',
            aes: 'AES',
            des: 'DES',
            asymmetric: '非对称',
            rsa: 'RSA',
        },
        theme: {
            dark: '暗黑',
            light: '明亮',
            yellow: '黄色',
            purple: '紫色'
        },
        message: {
            platform: '运行平台',
            currentVersion: '当前版本',
            lastVersion: '最新版本',
            alreadyUptodate: '已是最新',
            checkUpdate: '检查更新',
            installUpdate: '安装更新',
            newVersionDetected: '发现新版本',
            progress: '进度',
            downloadSpeed: '下载速度',
            installPackageSavedAt: '安装包保存在位置',
            downloadFailed: '下载失败',
            sourceRepository: '源码仓库',
            languages: '语言',
            themes: '主题',
            menu: '菜单',
            txt: '文本',
            file: '文件',
            encrypt: '加密',
            decrypt: '解密',
            type: '类型',
            mode: '模式',
            padding: '填充',
            key: '密钥',
            iv: '向量',
            plainText: '明文',
            cipherText: '密文',
            outputFormat: '输出格式',
            textCase: '大小写'
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
        theme: {
            dark: 'dark',
            light: 'light',
            yellow: 'yellow',
            purple: 'purple'
        },
        message: {
            platform: 'platform',
            currentVersion: 'current version',
            lastVersion: 'last version',
            alreadyUptodate: 'already up to date',
            checkUpdate: 'check update',
            installUpdate: 'install update',
            newVersionDetected: 'new version detected',
            progress: 'progress',
            downloadSpeed: 'download speed',
            installPackageSavedAt: 'install package save at',
            downloadFailed: 'download failed',
            sourceRepository: 'source repository',
            languages: 'languages',
            themes: 'themes',
            menu: 'menu',
            txt: 'text',
            file: 'file',
            encrypt: 'encrypt',
            decrypt: 'decrypt',
            type: 'type',
            mode: 'mode',
            padding: 'padding',
            key: 'key',
            iv: 'iv',
            plainText: 'plain text',
            cipherText: 'cipher text',
            outputFormat: 'output format',
            textCase: 'text case'
        }
    },
};

function createI18n(viewuiZh,viewuiEn) {
    return VueI18n.createI18n({
        allowComposition: true,
        globalInjection: true,
        legacy: false,
        locale: '中文',
        fallbackLocale: '中文',
        messages: {
            中文: { ...viewuiZh, ...i18nMessages.中文 },
            English: { ...viewuiEn, ...i18nMessages.English },
        }
    });
}