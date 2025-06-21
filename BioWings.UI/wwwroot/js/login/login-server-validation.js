document.addEventListener('DOMContentLoaded', function () {
    // Login hatası varsa göster (TempData'dan gelen) yani server validation hatası
    const loginErrorElement = document.getElementById('login-error');
    if (loginErrorElement) {
        const loginError = loginErrorElement.getAttribute('data-error');
        if (loginError && loginError.trim()) {
            Swal.fire({
                title: 'Login Error',
                text: loginError,
                icon: 'error',
                confirmButtonText: 'OK'
            });
        }
    }
});