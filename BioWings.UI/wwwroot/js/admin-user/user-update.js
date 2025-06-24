// User Update functionality
async function populateUpdateModal(userId) {
    try {
        const response = await fetch(`${API_CONFIG.BASE_URL}/Users/${userId}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            throw new Error('Kullanıcı bilgileri alınamadı');
        }

        const userData = await response.json();

        // Input alanlarını doldur
        document.getElementById('updateUserId').value = userData.data.id;
        document.getElementById('updateUserFirstName').value = userData.data.firstName;
        document.getElementById('updateUserLastName').value = userData.data.lastName;
        document.getElementById('updateUserEmail').value = userData.data.email;
        document.getElementById('updateUserCountry').value = userData.data.countryId;
        document.getElementById('updateUserEmailConfirmed').checked = userData.data.isEmailConfirmed;

        const updateModal = new bootstrap.Modal(document.getElementById('kt_modal_update_user'));
        updateModal.show();

    } catch (error) {
        Swal.fire({
            icon: 'error',
            title: 'Hata',
            text: 'Kullanıcı bilgileri yüklenirken bir hata oluştu.'
        });
        console.error('Fetch error:', error);
    }
} 