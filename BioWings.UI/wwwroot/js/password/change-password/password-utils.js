// password-utils.js - Utility functions for password operations

// Password toggle function
function togglePassword(fieldId) {
    const field = document.getElementById(fieldId);
    const icon = field.nextElementSibling.querySelector('i');
    
    if (field.type === 'password') {
        field.type = 'text';
        icon.classList.remove('fa-eye-slash');
        icon.classList.add('fa-eye');
    } else {
        field.type = 'password';
        icon.classList.remove('fa-eye');
        icon.classList.add('fa-eye-slash');
    }
}

// Reset password validation indicators
function resetPasswordValidation() {
    const requirements = ['min-length', 'has-uppercase', 'has-lowercase', 'has-number', 'has-special', 'passwords-match'];
    
    requirements.forEach(id => {
        const element = document.getElementById(id);
        if (element) {
            updateRequirement(element, false);
        }
    });
    
    const passwordsMatchElement = document.getElementById('passwords-match');
    if (passwordsMatchElement) {
        passwordsMatchElement.closest('.password-requirement').style.visibility = 'hidden';
    }
} 