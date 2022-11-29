function DisplaySuccessfulNotification(title, message) {
    iziToast.success({
        title: title,
        message: message,
    });
}

function DisplayInfoNotification(title, message) {
    iziToast.info({
        title: title,
        message: message,
    });
}

function DisplayWaringNotification(title, message) {
    iziToast.warning({
        title: title,
        message: message,
    });
}

function DisplayErrorNotification(title, message) {
    iziToast.error({
        title: title,
        message: message,
    });
}