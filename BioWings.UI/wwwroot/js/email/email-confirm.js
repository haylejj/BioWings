function resendConfirmationEmail(email) {

    const resendLabel = document.getElementById('resend-label');
    const resendProgress = document.getElementById('resend-progress');

    resendLabel.classList.add('d-none');
    resendProgress.classList.remove('d-none');


    fetch(`${API_CONFIG.BASE_URL}/Email/resendEmailConfirmation`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ email: email })
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            Swal.fire({
                title: 'Başarılı',
                text: 'Onay e-postası tekrar gönderildi.',
                icon: 'success',
                confirmButtonText: 'Tamam'
            });
        })
        .catch(error => {
            Swal.fire({
                title: 'Hata',
                text: 'Onay e-postası gönderilirken bir hata oluştu.',
                icon: 'error',
                confirmButtonText: 'Tamam'
            });
        })
        .finally(() => {
            resendLabel.classList.remove('d-none');
            resendProgress.classList.add('d-none');
        });
}