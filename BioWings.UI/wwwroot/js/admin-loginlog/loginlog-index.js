// LoginLog Index - DataTable initialization and search functionality
$(document).ready(function () {
    // Eğer DataTable zaten varsa, destroy et
    if ($.fn.DataTable.isDataTable('#kt_datatable')) {
        $('#kt_datatable').DataTable().destroy();
    }
    
    var table = $("#kt_datatable").DataTable({
        "language": {
            "url": "/assets/i18n/Turkish.json",
            "emptyTable": "Henüz giriş log'u bulunmuyor."
        },
        "responsive": {
            "details": false
        },
        "lengthChange": true,
        "pageLength": 10,
        "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "Tümü"]],
        "order": [[3, "desc"]], // Tarih sütununa göre azalan sıralama
        "columnDefs": [
            {
                targets: [5, 6], // User Agent ve Hata Sebebi sütunları
                orderable: false
            },
            {
                targets: 0, // ID sütunu
                className: 'dt-body-center',
                orderable: true
            }
        ]
    });
    
    // Özel arama kutusunu DataTable ile bağla
    $('#customSearch').keyup(function(){
        table.search($(this).val()).draw();
    });

    // Tooltip'leri aktif et
    $('[data-bs-toggle="tooltip"]').tooltip();
});

function showUserHistory(userId) {
    if (!userId || userId === -1 || userId <= 0) {
        Swal.fire({
            icon: 'warning',
            title: 'Uyarı!',
            text: 'Bu kullanıcı için geçmiş görüntülenemez.',
            timer: 3000,
            showConfirmButton: false
        });
        return;
    }
    window.location.href = '/Admin/LoginLog/UserHistory/' + userId;
} 