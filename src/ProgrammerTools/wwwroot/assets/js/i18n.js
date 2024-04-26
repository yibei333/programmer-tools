﻿function createI18n(viewuiZh, viewuiEn) {
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

const i18nMessages = {
    中文: {
        app: {
            name: '程序员工具',
            menus: {
                home: '主页',
                crypto: '加密 / 解密',
                symmetric: '对称',
                aes: 'AES',
                des: 'DES',
                tripledes: '3DES',
                asymmetric: '非对称',
                rsakey: 'RSA密钥',
                rsa: 'RSA',
            },
            theme: {
                dark: '暗黑',
                light: '明亮',
            },
        },
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
        textCase: '大小写',
        aesIVNotice: '向量长度固定为16个字节',
        aesKeyNotice: '密钥长度应为128位(bit)、192位(bit)、256位(bit),如果超出256位(bit)将截断,长度不足向上补足(字节为0)',
        desIVNotice: '向量长度固定为8个字节',
        desKeyNotice: '密钥长度应为64位(bit),如果超出64位将截断,长度不足向上补足(字节为0)',
        tripleDesIVNotice: '向量长度固定为8个字节',
        tripleDesKeyNotice: '密钥长度应为128位(bit)、192位(bit),如果超出192位(bit)将截断,长度不足向上补足(字节为0)',
        copy: '拷贝',
        convertToHex: '转换为Hex',
        copySuccess: '拷贝成功',
        ivFormatError: '向量格式错误',
        keyRequired: '请输入密钥',
        operateComplete: '操作完成',
        fileSavedAt: '文件保存在位置',
        waitExecute: '等待执行',
        executing: '执行中',
        success: '成功',
        fail: '失败',
        selectFile: '选择文件',
        please: '请',
        convertType: '格式转换',
        matchKeyPair: '密钥对校验',
        exportPublicKey: '导出公钥',
        generateKeyPair: '生成密钥对',
        length: '长度',
        password: '密码',
        generate: '生成',
        privateKey: '私钥',
        publicKey: '公钥',
        export: '导出',
    },
    English: {
        app: {
            name: 'ProgrammerTools',
            menus: {
                home: 'Home',
                crypto: 'Encrypt / Decrypt',
                symmetric: 'Symmetric',
                aes: 'AES',
                des: 'DES',
                tripledes: '3DES',
                asymmetric: 'Asymmetric',
                rsakey: 'RSA Key',
                rsa: 'RSA'
            },
            theme: {
                dark: 'dark',
                light: 'light',
            },
        },
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
        textCase: 'text case',
        aesIVNotice: 'vector lenght should be 16 bit',
        aesKeyNotice: 'key lenght should be 128 bit、192 bit、256 bit,if it exceeds 256 bit, it will be truncated, and if the length is insufficient, it will be supplemented upwards (byte is 0)',
        desIVNotice: 'vector lenght should be 8 bit',
        desKeyNotice: 'key lenght should be 64 bit,if it exceeds 64 bit, it will be truncated, and if the length is insufficient, it will be supplemented upwards (byte is 0)',
        tripleDesIVNotice: 'vector lenght should be 8 bit',
        tripleDesKeyNotice: 'key lenght should be 128 bit、192 bit,if it exceeds 192 bit, it will be truncated, and if the length is insufficient, it will be supplemented upwards (byte is 0)',
        copy: 'copy',
        convertToHex: 'convert to hex',
        copySuccess: 'copy succeed',
        ivFormatError: 'iv format error',
        keyRequired: 'key required',
        operateComplete: 'operate complete',
        fileSavedAt: 'file saved at',
        waitExecute: 'wait execute',
        executing: 'executing',
        success: 'success',
        fail: 'fail',
        selectFile: 'select files',
        please: 'please',
        convertType: 'convert type',
        matchKeyPair: 'match key pair',
        exportPublicKey: 'export public key',
        generateKeyPair: 'generate key pair',
        length: 'length',
        password: 'password',
        generate: 'generate',
        privateKey: 'private key',
        publicKey: 'public key',
        export: 'export',
    },
};

