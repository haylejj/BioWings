function confirmDelete(id) {
    Swal.fire({
        title: 'Emin misiniz?',
        text: "Bu Gözlem kalıcı olarak silinecektir!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Evet, sil!',
        cancelButtonText: 'İptal'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: `${apiConfig.baseUrl}/api/Observations/${id}`,
                type: 'DELETE',
                success: function (response) {
                    Swal.fire({
                        title: 'Silindi!',
                        text: 'Gözlem başarıyla silindi.',
                        icon: 'success',
                        timer: 2000,
                        showConfirmButton: true
                    }).then(() => {
                        window.location.reload();
                    });
                },
                error: function (xhr, status, error) {
                    Swal.fire({
                        title: 'Hata!',
                        text: 'Silme işlemi sırasında bir hata oluştu.',
                        icon: 'error'
                    });
                }
            });
        }
    });
}