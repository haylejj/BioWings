document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('kt_password_reset_form');
    const submitButton = document.getElementById('kt_password_reset_submit');

    form.addEventListener('submit', function (e) {
        e.preventDefault();

        submitButton.setAttribute('data-kt-indicator', 'on');
        submitButton.disabled = true;

        const formData = new FormData(form);

        fetch('/password/forget', {
            method: 'POST',
            body: formData
        })
            .then(response => {
                if (response.ok) {
                    return response.json();
                } else {
                    throw new Error('Sunucu hatası');
                }
            })
            .then(data => {
                if (data.success) {
                    Swal.fire({
                        text: data.message || "Şifre sıfırlama bağlantısı e-posta adresinize gönderilmiştir.",
                        icon: "success",
                        buttonsStyling: false,
                        confirmButtonText: "Tamam",
                        customClass: {
                            confirmButton: "btn btn-primary"
                        }
                    });
                } else {
                    Swal.fire({
                        text: data.message || "İşlem başarısız oldu.",
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
                console.error('Fetch error:', error);
                Swal.fire({
                    text: "Bir hata oluştu. Lütfen tekrar deneyin.",
                    icon: "error",
                    buttonsStyling: false,
                    confirmButtonText: "Tamam",
                    customClass: {
                        confirmButton: "btn btn-primary"
                    }
                });
            })
            .finally(() => {
                submitButton.removeAttribute('data-kt-indicator');
                submitButton.disabled = false;
            });
    });
});