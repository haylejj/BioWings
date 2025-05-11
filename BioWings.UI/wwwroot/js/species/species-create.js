// species-create.js - Oluşturma işlemleri

function initializeCreateModal() {
    $('#kt_createmodal_species').on('show.bs.modal', function () {
        populateDropdowns('create');
    });

    $('#createSpeciesButton').on('click', handleCreateSubmit);

    $('#kt_createmodal_species').on('hidden.bs.modal', function () {
        $('#create_species_form')[0].reset();
        $('.is-invalid').removeClass('is-invalid');
        $('.invalid-feedback').remove();
    });
}

function handleCreateSubmit() {
    if (!validateFormCreate()) {
        return;
    }

    if (!validateAuthorityFields()) {
        return;
    }

    const formData = {
        scientificName: $('#createScientificName').val() || null,
        name: $('#createName').val() || null,
        turkishName: $('#createTurkishName').val() || null,
        englishName: $('#createEnglishName').val() || null,
        genusId: $('#createGenusId').val() ? parseInt($('#createGenusId').val()) : null,
        /*familyId: $('#createFamilyId').val() ? parseInt($('#createFamilyId').val()) : null,*/
        authorityName: $('#createAuthorityName').val() || null,
        authorityYear: $('#createAuthorityYear').val() ? parseInt($('#createAuthorityYear').val()) : null,
        euName: $('#createEUName').val() || null,
        fullName: $('#createFullName').val() || null,
        hesselbarthName: $('#createHesselbarthName').val() || null,
        turkishNamesTrakel: $('#createTurkishNamesTrakel').val() || null,
        trakel: $('#createTrakel').val() || null,
        kocakName: $('#createKocakName').val() || null
    };

    const submitButton = $('#createSpeciesButton');
    submitButton.attr('disabled', true);
    submitButton.find('.indicator-label').html(`
        <span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
        Creating...
    `);

    submitCreate(formData);
}

function submitCreate(formData) {
    // API modeliyle uyumlu hale getir
    const apiModel = {
        scientificName: formData.scientificName,
        name: formData.name,
        turkishName: formData.turkishName,
        englishName: formData.englishName,
        genusId: formData.genusId,
        authorityName: formData.authorityName,
        authorityYear: formData.authorityYear,
        euName: formData.euName,
        fullName: formData.fullName,
        hesselbarthName: formData.hesselbarthName,
        turkishNamesTrakel: formData.turkishNamesTrakel,
        trakel: formData.trakel,
        kocakName: formData.kocakName
    };
    const jsonData = JSON.stringify(apiModel);

    $.ajax({
        url: 'https://localhost:7128/api/Species',
        type: 'POST',
        contentType: 'application/json',
        data: jsonData,
        success: function (response, textStatus, jqXHR) {
            if (jqXHR.status === 201) {
                Swal.fire({
                    title: 'Success!',
                    text: 'Species created successfully',
                    icon: 'success',
                    timer: 2000,
                    showConfirmButton: true
                }).then(() => {
                    $('#kt_createmodal_species').modal('hide');
                    window.location.reload();
                });
            } else {
                handleCreateSuccess(response);
            }
        },
        error: function (jqXHR) {
            handleError(jqXHR, 'create');
        }
    });
}

function handleError(jqXHR, operation) {
    $(`#${operation}SpeciesButton`).removeAttr('disabled');
    $(`#${operation}SpeciesButton`).find('.indicator-label').text('Submit');

    let errorMessage = `Failed to ${operation} species`;
    try {
        const responseJson = JSON.parse(jqXHR.responseText);
        if (responseJson && responseJson.errors) {
            const errors = Object.values(responseJson.errors).flat();
            errorMessage = errors.join(', ');
        } else if (responseJson && responseJson.errorList && responseJson.errorList.length > 0) {
            errorMessage = responseJson.errorList.join(', ');
        } else if (responseJson && responseJson.title) {
            errorMessage = responseJson.title;
        }
    } catch (e) {
    }

    showError(errorMessage);
}

function handleCreateSuccess(response) {
    if (response.isSuccess) {
        Swal.fire({
            title: 'Success!',
            text: 'Species created successfully',
            icon: 'success',
            timer: 2000,
            showConfirmButton: true
        }).then(() => {
            $('#kt_createmodal_species').modal('hide');
            window.location.reload();
        });
    } else {
        $('#createSpeciesButton').removeAttr('disabled');
        $('#createSpeciesButton').find('.indicator-label').text('Submit');
        showError(response.errorList ? response.errorList.join(', ') : 'Failed to create species');
    }
}

function validateFormCreate() {
    const form = $('#create_species_form');
    if (!form.length) {
        return false;
    }

    if (!form[0].checkValidity()) {
        Swal.fire({
            title: 'Validation Error!',
            text: 'Please fill in all required fields.',
            icon: 'error',
            confirmButtonText: 'OK'
        });

        $('#createSpeciesButton').removeAttr('disabled');
        $('#createSpeciesButton').find('.indicator-label').text('Submit');

        return false;
    }

    return true;
}

function validateAuthorityFields() {
    const authorityName = $('#createAuthorityName').val();
    const authorityYear = $('#createAuthorityYear').val();

    if ((authorityName && !authorityYear) || (!authorityName && authorityYear)) {
        Swal.fire({
            title: 'Validation Error!',
            text: 'Both Authority Name and Year must be provided together.',
            icon: 'error',
            confirmButtonText: 'OK'
        });

        $('#createSpeciesButton').removeAttr('disabled');
        $('#createSpeciesButton').find('.indicator-label').text('Submit');

        return false;
    }

    if (authorityYear && (authorityYear < 1700 || authorityYear > 2100)) {
        Swal.fire({
            title: 'Validation Error!',
            text: 'Authority Year must be between 1700 and 2100.',
            icon: 'error',
            confirmButtonText: 'OK'
        });

        $('#createSpeciesButton').removeAttr('disabled');
        $('#createSpeciesButton').find('.indicator-label').text('Submit');

        return false;
    }

    return true;
}