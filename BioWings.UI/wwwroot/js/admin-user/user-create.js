// User Create functionality
function openCreateUserModal() {
    document.getElementById('createUserFirstName').value = '';
    document.getElementById('createUserLastName').value = '';
    document.getElementById('createUserEmail').value = '';
    document.getElementById('createUserCountry').value = '';

    const createModal = new bootstrap.Modal(document.getElementById('kt_modal_create_user'));
    createModal.show();
}

document.getElementById('createUserForm').addEventListener('submit', async function(e) {
    e.preventDefault();

    const userData = {
        firstName: document.getElementById('createUserFirstName').value,
        lastName: document.getElementById('createUserLastName').value,
        email: document.getElementById('createUserEmail').value,
        countryId: parseInt(document.getElementById('createUserCountry').value)
    };

    try {
        const response = await fetch(`${API_CONFIG.BASE_URL}/Users/ByAdmin`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(userData)
        });

        if (!response.ok) {
            throw new Error('Kullanıcı oluşturma başarısız');
        }

        Swal.fire({
            icon: 'success',
            title: 'Başarılı!',
            text: 'Kullanıcı başarıyla oluşturuldu.',
            timer: 2000,
            timerProgressBar: true
        }).then(() => {
            bootstrap.Modal.getInstance(document.getElementById('kt_modal_create_user')).hide();
            location.reload();
        });
    } catch (error) {
        console.error('Hata:', error);
        Swal.fire({
            icon: 'error',
            title: 'Hata!',
            text: 'Kullanıcı oluşturulurken bir hata oluştu.'
        });
    }
}); 