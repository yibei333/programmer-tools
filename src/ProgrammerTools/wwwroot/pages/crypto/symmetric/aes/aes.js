export default {
    data() {
        return {
            request: {
                type: "",
                mode: '',
                padding: '',
                key: '',
                iv: '',
                result: {
                    type: '',
                    case: ''
                }
            },
            types: ['txt', 'file'],
            modes: ['CBC', 'ECB', 'OFB', 'CFB', 'CTS'],
            paddings: ['None', 'PKCS7', 'Zeros', 'ANSIX923', 'ISO10126'],
            resultTypes: ['base64', 'hex'],
            resultCases: ['lower', 'upper']
        }
    },
    mounted() {

    },
    methods: {
        encrypt() {

        },
        decrypt() {

        }
    }
}