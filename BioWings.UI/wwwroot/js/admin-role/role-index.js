// Role Index - DataTable initialization and search functionality
$(document).ready(function () {
    var table = $("#kt_datatable").DataTable({
        "language": {
            "url": "/assets/i18n/Turkish.json",
            "emptyTable": "Kayıtlı hiç rol bulunamadı."
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
                orderable: false,
                className: 'text-center'
            },
            {
                targets: 0,
                className: 'text-left dt-body-left',
                orderable: true,
                width: "80px"
            },
            {
                targets: 1,
                className: 'text-left',
                width: "300px"
            },
            {
                targets: 2,
                className: 'text-center',
                width: "200px"
            }
        ]
    });

    $('#customSearch').keyup(function(){
        table.search($(this).val()).draw();
    });

    $('[data-bs-toggle="tooltip"]').tooltip();
}); 