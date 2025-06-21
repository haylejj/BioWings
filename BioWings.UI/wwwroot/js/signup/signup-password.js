// signup-password.js - Password validation and strength indicators

document.addEventListener('DOMContentLoaded', function () {
    const passwordField = document.getElementById('password-field');
    const confirmPasswordField = document.getElementById('confirm-password-field');

    // Password strength indicators
    const minLength = document.getElementById('min-length');
    const hasUppercase = document.getElementById('has-uppercase');
    const hasLowercase = document.getElementById('has-lowercase');
    const hasNumber = document.getElementById('has-number');
    const hasSpecial = document.getElementById('has-special');

    // Password validation
    passwordField.addEventListener('input', function () {
        const password = this.value;

        // Check requirements
        const meetsLength = password.length >= 8;
        const meetsUppercase = /[A-Z]/.test(password);
        const meetsLowercase = /[a-z]/.test(password);
        const meetsNumber = /[0-9]/.test(password);
        const meetsSpecial = /[^A-Za-z0-9]/.test(password);

        // Update indicators
        updateRequirement(minLength, meetsLength);
        updateRequirement(hasUppercase, meetsUppercase);
        updateRequirement(hasLowercase, meetsLowercase);
        updateRequirement(hasNumber, meetsNumber);
        updateRequirement(hasSpecial, meetsSpecial);
    });

    // Confirm password validation
    confirmPasswordField.addEventListener('input', function() {
        const password = passwordField.value;
        const confirmPassword = this.value;

        if (password === confirmPassword) {
            this.setCustomValidity('');
        } else {
            this.setCustomValidity('Passwords do not match');
        }
    });
});

// Update requirement indicator
function updateRequirement(element, isMet) {
    if (isMet) {
        element.classList.remove('requirement-unmet');
        element.classList.add('requirement-met');
    } else {
        element.classList.remove('requirement-met');
        element.classList.add('requirement-unmet');
    }
}

// Password validation function
function validatePassword() {
    const password = document.getElementById('password-field').value;

    // Check requirements
    const meetsLength = password.length >= 8;
    const meetsUppercase = /[A-Z]/.test(password);
    const meetsLowercase = /[a-z]/.test(password);
    const meetsNumber = /[0-9]/.test(password);
    const meetsSpecial = /[^A-Za-z0-9]/.test(password);

    // Ensure all requirements are met
    return meetsLength && meetsUppercase && meetsLowercase &&
           meetsNumber && meetsSpecial;
} 