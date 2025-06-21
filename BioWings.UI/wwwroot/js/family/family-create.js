// Create functionality
$('#createFamilyButton').on('click', function () {
    if (!$('#create_family_form')[0].checkValidity()) {
        Swal.fire({
            title: 'Validation Error!',
            text: 'Please fill in all required fields.',
            icon: 'error',
            confirmButtonText: 'OK'
        });
        return;
    }
    console.log(API_CONFIG);
    const formData = {
        name: $('#createName').val()
    };

    $.ajax({
        url: `${API_CONFIG.BASE_URL}/Families`,
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(formData),
        success: function (response) {
            if (response.isSuccess) {
                Swal.fire({
                    title: 'Success!',
                    text: 'Family created successfully',
                    icon: 'success',
                    timer: 2000,
                    showConfirmButton: true
                }).then(() => {
                    $('#kt_createmodal_family').modal('hide');
                    window.location.reload();
                });
            } else {
                Swal.fire({
                    title: 'Error!',
                    text: response.errorList ? response.errorList.join(', ') : 'Failed to create family',
                    icon: 'error'
                });
            }
        },
        error: function (xhr, status, error) {
            let errorMessage = 'Failed to create family';
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