document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('kt_sign_in_form');
    const passwordInput = document.querySelector('[name="Password"]');

    function showValidationError(message) {
        Swal.fire({
            title: 'Validation Error',
            text: message,
            icon: 'warning',
            confirmButtonText: 'OK'
        });
    }

    form.addEventListener('submit', function (e) {
        const password = passwordInput.value;

        // Password validation
        if (!password) {
            e.preventDefault();
            showValidationError('Password is required.');
            passwordInput.focus();
            if (window.resetSubmitButton) {
                window.resetSubmitButton();
            }
            return false;
        }

        if (password.length < 6) {
            e.preventDefault();
            showValidationError('Password must be at least 6 characters long.');
            passwordInput.focus();
            if (window.resetSubmitButton) {
                window.resetSubmitButton();
            }
            return false;
        }
    });
});