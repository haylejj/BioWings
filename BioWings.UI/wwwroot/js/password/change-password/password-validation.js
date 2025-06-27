// password-validation.js - Password validation and strength indicators

document.addEventListener('DOMContentLoaded', function () {
    const newPasswordField = document.getElementById('NewPassword');
    const confirmPasswordField = document.getElementById('ConfirmNewPassword');

    // Password strength indicators
    const minLength = document.getElementById('min-length');
    const hasUppercase = document.getElementById('has-uppercase');
    const hasLowercase = document.getElementById('has-lowercase');
    const hasNumber = document.getElementById('has-number');
    const hasSpecial = document.getElementById('has-special');
    const passwordsMatch = document.getElementById('passwords-match');

    // Password validation
    if (newPasswordField) {
        newPasswordField.addEventListener('input', function () {
            const password = this.value;

            // Check requirements
            const meetsLength = password.length >= 8;
            const meetsUppercase = /[A-Z]/.test(password);
            const meetsLowercase = /[a-z]/.test(password);
            const meetsNumber = /[0-9]/.test(password);
            const meetsSpecial = /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(password);

            // Update indicators
            updateRequirement(minLength, meetsLength);
            updateRequirement(hasUppercase, meetsUppercase);
            updateRequirement(hasLowercase, meetsLowercase);
            updateRequirement(hasNumber, meetsNumber);
            updateRequirement(hasSpecial, meetsSpecial);

            // Check password match if confirm field has value
            checkPasswordMatch();
        });
    }

    // Confirm password validation
    if (confirmPasswordField) {
        confirmPasswordField.addEventListener('input', function() {
            checkPasswordMatch();
        });
    }

    function checkPasswordMatch() {
        const password = newPasswordField.value;
        const confirmPassword = confirmPasswordField.value;

        if (confirmPassword === '') {
            passwordsMatch.closest('.password-requirement').style.visibility = 'hidden';
        } else {
            passwordsMatch.closest('.password-requirement').style.visibility = 'visible';
            const matches = password === confirmPassword;
            updateRequirement(passwordsMatch, matches);
        }
    }
});

// Update requirement indicator
function updateRequirement(element, isMet) {
    if (!element) return;

    const parent = element.closest('.password-requirement');

    if (isMet) {
        element.classList.remove('requirement-unmet', 'fa-circle');
        element.classList.add('requirement-met', 'fa-check-circle');
        if (parent) {
            parent.classList.add('requirement-met');
        }
    } else {
        element.classList.remove('requirement-met', 'fa-check-circle');
        element.classList.add('requirement-unmet', 'fa-circle');
        if (parent) {
            parent.classList.remove('requirement-met');
        }
    }
}

// Password validation function
function validatePassword() {
    const password = document.getElementById('NewPassword').value;
    const confirmPassword = document.getElementById('ConfirmNewPassword').value;

    // Check requirements
    const meetsLength = password.length >= 8;
    const meetsUppercase = /[A-Z]/.test(password);
    const meetsLowercase = /[a-z]/.test(password);
    const meetsNumber = /[0-9]/.test(password);
    const meetsSpecial = /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(password);
    const passwordsMatch = password === confirmPassword;

    return meetsLength && meetsUppercase && meetsLowercase && 
           meetsNumber && meetsSpecial && passwordsMatch;
} 