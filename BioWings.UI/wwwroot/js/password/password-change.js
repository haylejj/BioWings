document.addEventListener('DOMContentLoaded', function() {

    const form = document.getElementById('kt_change_password_form');
    const submitButton = document.getElementById('kt_change_password_submit');
    const passwordInput = document.getElementById('password');
    const confirmPasswordInput = document.getElementById('confirmPassword');
    const tokenInput = document.getElementById('token');
    function validatePassword(password) {
        // At least 8 characters
        if (password.length < 8) return false;

        // At least one uppercase letter
        if (!/[A-Z]/.test(password)) return false;

        // At least one lowercase letter
        if (!/[a-z]/.test(password)) return false;

        // At least one digit
        if (!/[0-9]/.test(password)) return false;

        // At least one special character
        if (!/[!@@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(password)) return false;

        return true;
    }


    form.addEventListener('submit', function(e) {
        e.preventDefault();

        const passwordFeedback = passwordInput.nextElementSibling;
        const confirmPasswordFeedback = confirmPasswordInput.nextElementSibling;
        passwordFeedback.textContent = '';
        confirmPasswordFeedback.textContent = '';
        passwordInput.classList.remove('is-invalid');
        confirmPasswordInput.classList.remove('is-invalid');

        const password = passwordInput.value;
        const confirmPassword = confirmPasswordInput.value;
        const token = tokenInput.value;

        if (!validatePassword(password)) {
            passwordInput.classList.add('is-invalid');
            passwordFeedback.textContent = 'Şifre gereksinimleri karşılanmıyor.';
            return;
        }

        if (password !== confirmPassword) {
            confirmPasswordInput.classList.add('is-invalid');
            confirmPasswordFeedback.textContent = 'Şifreler eşleşmiyor.';
            return;
        }

        submitButton.setAttribute('data-kt-indicator', 'on');
        submitButton.disabled = true;

        const formData = new FormData(form);
        formData.append('Token', token); 

        fetch('/password/change', {
            method: 'POST',
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            },
            body: formData
        })
        .then(response => response.json())
        .then(data => {
            submitButton.removeAttribute('data-kt-indicator');
            submitButton.disabled = false;

            if (data.success) {
                Swal.fire({
                    text: "Şifreniz başarıyla değiştirildi.",
                    icon: "success",
                    buttonsStyling: false,
                    confirmButtonText: "Giriş Yap",
                    customClass: {
                        confirmButton: "btn btn-primary"
                    }
                }).then((result) => {
                    if (result.isConfirmed) {
                        window.location.href = '/Login/Login';
                    }
                });
            } else {
                Swal.fire({
                    text: data.message || "Şifre değiştirme işlemi başarısız oldu.",
                    icon: "error",
                    buttonsStyling: false,
                    confirmButtonText: "Tamam",
                    customClass: {
                        confirmButton: "btn btn-primary"
                    }
                });
            }
        })
        .catch(error => {
            submitButton.removeAttribute('data-kt-indicator');
            submitButton.disabled = false;

            Swal.fire({
                text: "Şifre değiştirme işlemi başarısız oldu.",
                icon: "error",
                buttonsStyling: false,
                confirmButtonText: "Tamam",
                customClass: {
                    confirmButton: "btn btn-primary"
                }
            });
        });
    });
}); 