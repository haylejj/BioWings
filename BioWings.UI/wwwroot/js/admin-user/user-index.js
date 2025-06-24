// User Index - DataTable initialization and search functionality
$(document).ready(function () {
    var table = $("#kt_datatable").DataTable({
        "language": {
            "url": "/assets/i18n/Turkish.json",
            "emptyTable": "Kayıtlı hiç kullanıcı bulunamadı."
        },
        "responsive": {
            "details": false
        },
        "lengthChange": true,
        "pageLength": 10,
        "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "Tümü"]],
        "columnDefs": [
            {
                targets: -1,
                orderable: false
            },
            {
                targets: 0,
                className: 'dt-body-center',
                orderable: true
            }
        ]
    });
    
    $('#customSearch').keyup(function(){
        table.search($(this).val()).draw();
    });

    $('[data-bs-toggle="tooltip"]').tooltip();
}); 