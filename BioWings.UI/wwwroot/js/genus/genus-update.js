async function updateGenus(id) {
    try {
        const response = await fetch(`${API_CONFIG.BASE_URL}/Genera/${id}`);
        if (!response.ok) {
            throw new Error('Failed to fetch genus data');
        }

        const result = await response.json();
        const genus = result.data;

        $('#updateId').val(genus.id);
        $('#updateName').val(genus.genusName);
        await populateFamilyDropdown('updateFamilyId', genus.familyId);

        const modal = new bootstrap.Modal(document.getElementById('kt_updatemodal_genus'));
        modal.show();
    } catch (error) {
        console.error('Error:', error);
        Swal.fire({
            title: 'Error!',
            text: 'Failed to fetch genus data: ' + error.message,
            icon: 'error'
        });
    }
}

$('#updateGenusButton').on('click', function () {
    if (!$('#update_genus_form')[0].checkValidity()) {
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
        name: $('#updateName').val(),
        familyId: $('#updateFamilyId').val() ? parseInt($('#updateFamilyId').val()) : null
    };

    $.ajax({
        url: `${API_CONFIG.BASE_URL}/Genera`,
        type: 'PUT',
        contentType: 'application/json',
        data: JSON.stringify(formData),
        success: function (response, status, xhr) {
            if (xhr.status === 204) {
                Swal.fire({
                    title: 'Success!',
                    text: 'Genus updated successfully',
                    icon: 'success',
                    timer: 2000,
                    showConfirmButton: true
                }).then(() => {
                    $('#kt_updatemodal_genus').modal('hide');
                    window.location.reload();
                });
            } else {
                if (response && response.isSuccess) {
                    Swal.fire({
                        title: 'Success!',
                        text: 'Genus updated successfully',
                        icon: 'success',
                        timer: 2000,
                        showConfirmButton: true
                    }).then(() => {
                        $('#kt_updatemodal_genus').modal('hide');
                        window.location.reload();
                    });
                } else {
                    Swal.fire({
                        title: 'Error!',
                        text: response?.errorList ? response.errorList.join(', ') : 'Failed to update genus',
                        icon: 'error'
                    });
                }
            }
        },
        error: function (xhr, status, error) {
            let errorMessage = 'Failed to update genus';
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