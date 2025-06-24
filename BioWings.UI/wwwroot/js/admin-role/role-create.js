// Role Create functionality
function openCreateRoleModal() {
    document.getElementById('createRoleName').value = '';

    const createModal = new bootstrap.Modal(document.getElementById('kt_modal_create_role'));
    createModal.show();
}

document.getElementById('createRoleForm').addEventListener('submit', async function(e) {
    e.preventDefault();

    const roleData = {
        name: document.getElementById('createRoleName').value
    };

    try {
        const response = await fetch(`${API_CONFIG.BASE_URL}/Roles`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(roleData)
        });

        if (!response.ok) {
            throw new Error('Rol oluşturma başarısız');
        }

        Swal.fire({
            icon: 'success',
            title: 'Başarılı!',
            text: 'Rol başarıyla oluşturuldu.',
            timer: 2000,
            timerProgressBar: true
        }).then(() => {
            bootstrap.Modal.getInstance(document.getElementById('kt_modal_create_role')).hide();
            location.reload();
        });
    } catch (error) {
        console.error('Hata:', error);
        Swal.fire({
            icon: 'error',
            title: 'Hata!',
            text: 'Rol oluşturulurken bir hata oluştu.'
        });
    }
}); 