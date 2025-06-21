// species-delete.js - Silme işlemleri

function confirmDelete(id) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!',
        cancelButtonText: 'Cancel'
    }).then((result) => {
        if (result.isConfirmed) {
            deleteSpecies(id);
        }
    });
}

function deleteSpecies(id) {
    $.ajax({
        url: `${API_CONFIG.BASE_URL}/Species/${id}`,
        type: 'DELETE',
        success: function (response, textStatus, xhr) {
            // Check if status is 204 No Content (successful deletion with no content returned)
            if (xhr.status === 204 || (response && response.isSuccess)) {
                Swal.fire({
                    title: 'Deleted!',
                    text: 'Species has been deleted successfully.',
                    icon: 'success',
                    timer: 2000,
                    showConfirmButton: true
                }).then(() => {
                    window.location.reload();
                });
            } else {
                let errorMessage = 'Failed to delete species';
                if (response && response.errorList) {
                    errorMessage = response.errorList.join(', ');
                }
                Swal.fire({
                    title: 'Error!',
                    text: errorMessage,
                    icon: 'error'
                });
            }
        },
        error: function (xhr, status, error) {
            let errorMessage = 'Failed to delete species';
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
}