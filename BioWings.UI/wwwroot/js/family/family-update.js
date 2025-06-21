// Update functionality
async function updateFamily(id) {
    try {
        const response = await fetch(`${API_CONFIG.BASE_URL}/Families/${id}`);
        if (!response.ok) {
            throw new Error('Failed to fetch family data');
        }

        const result = await response.json();
        const family = result.data;

        $('#updateId').val(family.id);
        $('#updateName').val(family.name);

        const modal = new bootstrap.Modal(document.getElementById('kt_updatemodal_family'));
        modal.show();
    } catch (error) {
        console.error('Error:', error);
        Swal.fire({
            title: 'Error!',
            text: 'Failed to fetch family data: ' + error.message,
            icon: 'error'
        });
    }
}

$('#updateFamilyButton').on('click', function () {
    if (!$('#update_family_form')[0].checkValidity()) {
        Swal.fire({
            title: 'Validation Error!',
            text: 'Please fill in all required fields.',
            icon: 'error',
            confirmButtonText: 'OK'
        });
        return;
    }

    const formData = {
        id: parseInt($('#updateId').val()),
        name: $('#updateName').val()
    };

    $.ajax({
        url: `${API_CONFIG.BASE_URL}/Families`,
        type: 'PUT',
        contentType: 'application/json',
        data: JSON.stringify(formData),
        success: function (response, status, xhr) {
            if (xhr.status === 204) {
                Swal.fire({
                    title: 'Success!',
                    text: 'Family updated successfully',
                    icon: 'success',
                    timer: 2000,
                    showConfirmButton: true
                }).then(() => {
                    $('#kt_updatemodal_family').modal('hide');
                    window.location.reload();
                });
            } else {
                Swal.fire({
                    title: 'Error!',
                    text: response?.errorList ? response.errorList.join(', ') : 'Failed to update family',
                    icon: 'error'
                });
            }
        },
        error: function (xhr, status, error) {
            let errorMessage = 'Failed to update family';
            if (xhr.responseJSON && xhr.responseJSON.errorList) {
                errorMessage = xhr.responseJSON.errorList.join(', ');
            }
            Swal.fire({
                title: 'Error!',
                text: errorMessage,
                icon: 'error'
            });
        }
    });
});