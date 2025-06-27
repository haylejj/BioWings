// change-password.js - Change password form submission

document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('changePasswordForm');
    const submitButton = document.getElementById('submitBtn');

    // Form submission
    if (form) {
        form.addEventListener('submit', function (e) {
            e.preventDefault();

            // Validate password before submission
            if (!validatePassword()) {
                Swal.fire({
                    icon: 'error',
                    title: 'Şifre Hatası!',
                    text: 'Şifreniz gereksinimları karşılamıyor.',
                    confirmButtonText: 'Tamam'
                });
                return false;
            }

            // Show loading state
            submitButton.setAttribute('data-kt-indicator', 'on');
            submitButton.disabled = true;

            // Submit the form
            submitChangePasswordForm(form, submitButton);
        });
    }
});

function submitChangePasswordForm(form, submitButton) {
    const formData = new FormData(form);
    
    fetch('/User/ChangePassword', {
        method: 'POST',
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
        },
        body: formData
    })
    .then(response => {
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        return response.json();
    })
    .then(data => {
        if (data.success) {
            // Successful password change
            Swal.fire({
                icon: 'success',
                title: 'Başarılı!',
                text: 'Şifreniz başarıyla değiştirildi.',
                confirmButtonText: 'Tamam'
            }).then(() => {
                form.reset();
                resetPasswordValidation();
            });
        } else {
            // Error case
            Swal.fire({
                icon: 'error',
                title: 'Hata!',
                text: data.message || 'Şifre değiştirme işlemi başarısız.',
                confirmButtonText: 'Tamam'
            });
        }
    })
    .catch(error => {
        // Handle different error types
        let errorMessage = 'Bir hata oluştu.';
        
        if (error.message.includes('400')) {
            errorMessage = 'Girilen bilgiler geçersiz.';
        } else if (error.message.includes('401')) {
            errorMessage = 'Oturum süreniz dolmuş. Lütfen tekrar giriş yapın.';
        } else if (error.message.includes('500')) {
            errorMessage = 'Sunucu hatası oluştu.';
        }

        Swal.fire({
            icon: 'error',
            title: 'Hata!',
            text: errorMessage,
            confirmButtonText: 'Tamam'
        });
    })
    .finally(() => {
        // Remove loading state
        submitButton.removeAttribute('data-kt-indicator');
        submitButton.disabled = false;
    });
} 