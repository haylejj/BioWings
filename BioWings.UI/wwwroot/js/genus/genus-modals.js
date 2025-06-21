$('#kt_createmodal_genus').on('hidden.bs.modal', function () {
    $('#create_genus_form')[0].reset();
    $('.is-invalid').removeClass('is-invalid');
    $('.invalid-feedback').remove();
});

$('#kt_updatemodal_genus').on('hidden.bs.modal', function () {
    $('#update_genus_form')[0].reset();
    $('.is-invalid').removeClass('is-invalid');
    $('.invalid-feedback').remove();
});