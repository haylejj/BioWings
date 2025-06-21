$('#kt_createmodal_family').on('hidden.bs.modal', function () {
    $('#create_family_form')[0].reset();
    $('.is-invalid').removeClass('is-invalid');
    $('.invalid-feedback').remove();
});

$('#kt_updatemodal_family').on('hidden.bs.modal', function () {
    $('#update_family_form')[0].reset();
    $('.is-invalid').removeClass('is-invalid');
    $('.invalid-feedback').remove();
});