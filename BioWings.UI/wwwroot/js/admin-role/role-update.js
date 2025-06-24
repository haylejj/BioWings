// Role Update functionality
async function populateUpdateModal(roleId) {
    try {
        const response = await fetch(`${API_CONFIG.BASE_URL}/Roles/${roleId}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            throw new Error('Rol bilgileri alınamadı');
        }

        const roleData = await response.json();

        // Input alanlarını doldur
        document.getElementById('updateRoleId').value = roleData.data.id;
        document.getElementById('updateRoleName').value = roleData.data.name;

        const updateModal = new bootstrap.Modal(document.getElementById('kt_modal_update_role'));
        updateModal.show();

    } catch (error) {
        Swal.fire({
            icon: 'error',
            title: 'Hata',
            text: 'Rol bilgileri yüklenirken bir hata oluştu.'
        });
        console.error('Fetch error:', error);
    }
} 