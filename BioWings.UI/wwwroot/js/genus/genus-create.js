$(document).ready(function () {
    $('#kt_createmodal_genus').on('show.bs.modal', function () {
        populateFamilyDropdown('createFamilyId');
    });

    $('#createGenusButton').on('click', function () {
        if (!$('#create_genus_form')[0].checkValidity()) {
            Swal.fire({
                title: 'Validation Error!',
                text: 'Please fill in all required fields.',
                icon: 'error',
                confirmButtonText: 'OK'
            });
            return;
        }

        const formData = {
            name: $('#createName').val(),
            familyId: $('#createFamilyId').val() ? parseInt($('#createFamilyId').val()) : null
        };

        $.ajax({
            url: `${API_CONFIG.BASE_URL}/Genera`,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(formData),
            success: function (response) {
                if (response.isSuccess) {
                    Swal.fire({
                        title: 'Success!',
                        text: 'Genus created successfully',
                        icon: 'success',
                        timer: 2000,
                        showConfirmButton: true
                    }).then(() => {
                        $('#kt_createmodal_genus').modal('hide');
                        window.location.reload();
                    });
                } else {
                    Swal.fire({
                        title: 'Error!',
                        text: response.errorList ? response.errorList.join(', ') : 'Failed to create genus',
                        icon: 'error'
                    });
                }
            },
            error: function (xhr, status, error) {
                let errorMessage = 'Failed to create genus';
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
});