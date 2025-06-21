// signup-form.js - Sign up form submission and validation

document.addEventListener('DOMContentLoaded', function () {
    // Form element
    const form = document.getElementById('kt_sign_up_form');
    const submitButton = document.getElementById('kt_sign_up_submit');
    const tocCheckbox = document.querySelector('input[name="toc"]');

    // Form submission
    form.addEventListener('submit', function (e) {
        e.preventDefault();

        // Check if all required fields are filled
        const firstName = form.querySelector('input[name="firstName"]').value;
        const lastName = form.querySelector('input[name="lastName"]').value;
        const email = form.querySelector('input[name="email"]').value;
        const country = form.querySelector('select[name="countryId"]').value;
        const password = document.getElementById('password-field').value;
        const confirmPassword = document.getElementById('confirm-password-field').value;

        // Validate required fields
        if (!firstName || !lastName || !email || !country || !password || !confirmPassword) {
            Swal.fire({
                title: 'Error!',
                text: 'Please fill in all required fields.',
                icon: 'error',
                confirmButtonText: 'OK'
            });
            return false;
        }

        // Validate password requirements
        if (!validatePassword()) {
            Swal.fire({
                title: 'Password Error!',
                text: 'Your password does not meet the requirements.',
                icon: 'error',
                confirmButtonText: 'OK'
            });
            return false;
        }

        // Confirm passwords match
        if (password !== confirmPassword) {
            Swal.fire({
                title: 'Error!',
                text: 'Passwords do not match.',
                icon: 'error',
                confirmButtonText: 'OK'
            });
            return false;
        }

        // Check if Terms and Conditions is checked
        if (!tocCheckbox.checked) {
            Swal.fire({
                title: 'Terms Required!',
                text: 'You must accept the Terms of Use and Privacy Policy to continue.',
                icon: 'warning',
                confirmButtonText: 'OK'
            });
            return false;
        }

        // Show loading indication
        submitButton.setAttribute('data-kt-indicator', 'on');
        submitButton.disabled = true;

        // Submit the form
        submitForm(form, submitButton);
    });
});

function submitForm(form, submitButton) {
    const formData = new FormData(form);
    
    fetch('/signup', {
        method: 'POST',
        headers: {
            'X-Requested-With': 'XMLHttpRequest'
        },
        body: formData
    })
    .then(response => {
        if (!response.ok && !response.headers.get('content-type').includes('application/json')) {
            throw new Error('Server error occurred');
        }
        return response.json();
    })
    .then(data => {
        if (data.success) {
            // Successful registration
            Swal.fire({
                title: 'Registration Successful!',
                text: 'A confirmation email has been sent to your address. Please check your inbox and click the verification link to activate your account.',
                icon: 'success',
                confirmButtonText: 'OK'
            }).then(() => {
                if (data.redirectUrl) {
                    window.location.href = data.redirectUrl;
                } else {
                    window.location.href = '/Login/Login';
                }
            });
        } else {
            // Error case
            Swal.fire({
                title: 'Registration Failed!',
                text: data.message || 'An error occurred during registration.',
                icon: 'error',
                confirmButtonText: 'OK'
            });
        }
    })
    .catch(error => {
        // General error
        Swal.fire({
            title: 'Registration Failed!',
            text: 'An error occurred during registration. Please try again later.',
            icon: 'error',
            confirmButtonText: 'OK'
        });
    })
    .finally(() => {
        // Remove loading state
        submitButton.removeAttribute('data-kt-indicator');
        submitButton.disabled = false;
    });
} 