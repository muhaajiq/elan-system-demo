//window.jsInterop = {
//    showModal: function (selector) {
//        var modal = new bootstrap.Modal(document.querySelector(selector));
//        modal.show();
//    },
//    hideModal: function (selector) {
//        var modal = bootstrap.Modal.getInstance(document.querySelector(selector));
//        if (modal) modal.hide();
//    }
//};

window.jsInterop = window.jsInterop || {};

window.jsInterop.showModal = function (selector) {
    var modal = new bootstrap.Modal(document.querySelector(selector));
    modal.show();
};

window.jsInterop.hideModal = function (selector) {
    var modal = bootstrap.Modal.getInstance(document.querySelector(selector));
    if (modal) modal.hide();
};

window.jsInterop.scrollToTop = function () {
    window.scrollTo({
        top: 0,
        behavior: 'smooth'
    });
};

window.jsInterop.scrollToError = function (id) {
    var el = document.getElementById(id);
    if (el) {
        el.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
};

window.showSwalLoading = (title, message) => {
    Swal.fire({
        title: title || 'Processing Request',
        html: `<b class="swal-text-custom">${message || 'Please wait while we complete your request...'}</b>`,
        background: '#ffffff',
        allowOutsideClick: false,
        allowEscapeKey: false,
        showConfirmButton: false,
        didOpen: () => {
            Swal.showLoading();
        },
        customClass: {
            popup: 'swal-loading-popup',
            title: 'swal-title-custom',
            htmlContainer: 'swal-text-custom'
        }
    });
};

window.showSwalSuccess = (title) => {
    Swal.fire({
        position: 'center',
        icon: 'success',
        title: title,
        showConfirmButton: false,
        timer: 2000,
        background: '#ffffff',
        customClass: {
            title: 'swal-title-custom',
            popup: 'swal-success-popup'
        }
    });
};

window.showSwalError = (message) => {
    Swal.fire({
        icon: 'error',
        title: 'Error',
        text: message,
        customClass: {
            popup: 'swal-error-popup'
        }
    });
};

window.showSwalWarning = (title, text) => {
    Swal.fire({
        title: title,
        text: text,
        icon: 'warning',
        confirmButtonText: 'OK',
        customClass: {
            popup: 'swal-warning-popup',
            title: 'swal-title-custom',
            text: 'swal-text-custom'
        }
    });
};

window.showSwalValidationWarning = (title, htmlMessage) => {
    Swal.fire({
        title: title,
        html: htmlMessage,
        icon: 'warning',
        confirmButtonText: 'OK',
        customClass: {
            popup: 'swal-warning-popup',
            title: 'swal-title-custom',
            htmlContainer: 'swal-html-custom'
        }
    });
};

window.showSwalGeneralLoading = (title, message) => {
    Swal.fire({
        title: title || 'Loading',
        html: `<b class="swal-text-custom">${message || 'Please wait...'}</b>`,
        background: '#ffffff',
        allowOutsideClick: false,
        allowEscapeKey: false,
        showConfirmButton: false,
        didOpen: () => {
            Swal.showLoading();
        },
        customClass: {
            popup: 'swal-loading-popup',
            title: 'swal-title-custom',
            htmlContainer: 'swal-text-custom'
        }
    });
};
