// signup-utils.js - Utility functions for signup

// Password toggle function
function togglePassword(fieldId) {
    const passwordField = document.getElementById(fieldId);
    const passwordToggle = passwordField.parentElement.querySelector('.toggle-password');

    if (passwordField.type === 'password') {
        passwordField.type = 'text';
        passwordToggle.classList.remove('fa-eye-slash');
        passwordToggle.classList.add('fa-eye');
    } else {
        passwordField.type = 'password';
        passwordToggle.classList.remove('fa-eye');
        passwordToggle.classList.add('fa-eye-slash');
    }
} 