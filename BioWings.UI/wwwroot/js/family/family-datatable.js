// DataTable initialization
var table = $('#familyTable').DataTable({
    "searching": false,
    "paging": false,
    "info": false,
    "ordering": true,
    "autoWidth": false,
    "order": [[0, "asc"]],
    "columnDefs": [
        {
            "targets": 0,
            "width": "10%",
            "className": "fw-bold"
        },
        {
            "targets": 1,
            "width": "75%",
            "className": "fw-semibold"
        },
        {
            "targets": -1,
            "width": "15%",
            "className": "text-end",
            "orderable": false
        }
    ]
});