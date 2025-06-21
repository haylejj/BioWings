function confirmDelete(id) {
    Swal.fire({
        title: 'Are you sure?',
        text: "This Genus will be permanently deleted!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!',
        cancelButtonText: 'Cancel'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: `${API_CONFIG.BASE_URL}/Genera/${id}`,
                type: 'DELETE',
                success: function (response) {
                    Swal.fire({
                        title: 'Deleted!',
                        text: 'Genus has been deleted successfully.',
                        icon: 'success',
                        timer: 2000,
                        showConfirmButton: true
                    }).then(() => {
                        window.location.reload();
                    });
                },
                error: function (xhr, status, error) {
                    Swal.fire({
                        title: 'Error!',
                        text: 'An error occurred during deletion.',
                        icon: 'error'
                    });
                }
            });
        }
    });
}