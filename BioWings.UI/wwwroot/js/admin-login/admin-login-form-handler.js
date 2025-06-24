document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('kt_sign_in_form');
    const submitButton = document.getElementById('kt_sign_in_submit');

    form.addEventListener('submit', function (e) {

        setTimeout(function () {
            if (!e.defaultPrevented) {
                submitButton.setAttribute('data-kt-indicator', 'on');
                submitButton.disabled = true;
            }
        }, 100);
    });

    window.resetSubmitButton = function () {
        submitButton.removeAttribute('data-kt-indicator');
        submitButton.disabled = false;
    };
}); 