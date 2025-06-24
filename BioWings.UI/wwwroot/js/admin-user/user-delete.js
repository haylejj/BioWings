// User Delete functionality
async function deleteUser(userId) {
    const result = await Swal.fire({
        title: 'Emin misiniz?',
        text: "Bu kullanıcıyı silmek istediğinizden emin misiniz?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Evet, sil!',
        cancelButtonText: 'İptal'
    });

    if (result.isConfirmed) {
        try {
            const response = await fetch(`${API_CONFIG.BASE_URL}/Users/${userId}`, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            // Sadece 204 No Content durumunda başarılı
            if (response.status === 204) {
                Swal.fire(
                    'Silindi!',
                    'Kullanıcı başarıyla silindi.',
                    'success'
                ).then(() => {
                    location.reload();
                });
            } else {
                // Diğer tüm durumlarda standart hata mesajı
                Swal.fire(
                    'Hata!',
                    'Silme işlemi sırasında bir hata oluştu.',
                    'error'
                );
            }
        } catch (error) {
            console.error("Hata:", error);
            Swal.fire(
                'Hata!',
                'Silme işlemi sırasında bir hata oluştu.',
                'error'
            );
        }
    }
} 