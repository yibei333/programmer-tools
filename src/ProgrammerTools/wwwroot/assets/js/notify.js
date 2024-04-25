let notifyMessageSeted = false;

function notify(type, message) {
    if (!ViewUIPlus?.Message) {
        alert(message);
        return;
    }

    if (!notifyMessageSeted) {
        ViewUIPlus.Message.config({
            top: 50,
            duration: 5
        });
        notifyMessageSeted = true;
    }

    if (type == 'info') ViewUIPlus.Message.info(message);
    else if (type == 'success') ViewUIPlus.Message.success(message);
    else if (type == 'warning') ViewUIPlus.Message.warning(message);
    else if (type == 'error') ViewUIPlus.Message.error({
        content: message,
        duration: 30,
        closable: true
    });
    else if (type == 'loading') ViewUIPlus.Message.loading(message);
    else alert(`not supported notify type:${type}`);
}

function notifyInfo(message) { notify('info', message) }
function notifySuccess(message) { notify('success', message) }
function notifyWarning(message) { notify('warning', message) }
function notifyError(message) { notify('error', message) }
function notifyLoading(message) { notify('loading', message) }
