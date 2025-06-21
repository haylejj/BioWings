// species-update.js - Güncelleme işlemleri

async function updateSpecies(id) {
    try {
        const species = await fetchSpeciesData(id);
        await populateDropdowns('update');
        populateUpdateForm(species);
        showUpdateModal();
    } catch (error) {
        console.error('Error:', error);
        showError('Failed to fetch species data: ' + error.message);
    }
}

async function fetchSpeciesData(id) {
    const response = await fetch(`${API_CONFIG.BASE_URL}/Species/${id}`);
    if (!response.ok) {
        throw new Error('Failed to fetch species data');
    }
    const result = await response.json();
    return result.data;
}

function populateUpdateForm(species) {
    if (!species) return;

    $('#updateId').val(species.id || '');
    $('#updateScientificName').val(species.scientificName || '');
    $('#updateName').val(species.name || '');
    $('#updateTurkishName').val(species.turkishName || '');
    $('#updateEnglishName').val(species.englishName || '');
    $('#updateGenusId').val(species.genusId || '');
    $('#updateAuthorityName').val(species.authorityName || '');
    $('#updateAuthorityYear').val(species.authorityYear || '');
    $('#updateEUName').val(species.euName || '');
    $('#updateFullName').val(species.fullName || '');
    $('#updateHesselbarthName').val(species.hesselbarthName || '');
    $('#updateTurkishNamesTrakel').val(species.turkishNamesTrakel || '');
    $('#updateTrakel').val(species.trakel || '');
    $('#updateKocakName').val(species.kocakName || '');

}
// Update formu için ayrı doğrulama fonksiyonu
function validateFormUpdate() {
    const form = $('#update_species_form');
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

        $('#updateSpeciesButton').removeAttr('disabled');
        $('#updateSpeciesButton').find('.indicator-label').text('Update');

        return false;
    }

    return true;
}
function showUpdateModal() {
    const modal = new bootstrap.Modal(document.getElementById('kt_updatemodal_species'));
    modal.show();
}

// Update form submission handler
$(document).ready(function () {
    $('#updateSpeciesButton').on('click', function () {

        if (!validateFormUpdate()) return;
        if (!validateAuthorityFieldsUpdate()) return;

        const submitButton = $(this);
        submitButton.attr('disabled', true);
        submitButton.find('.indicator-label').html(`
            <span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
            Updating...
        `);

        const formData = {
            id: parseInt($('#updateId').val()) || 0,
            name: $('#updateName').val() || null,
            scientificName: $('#updateScientificName').val() || null,
            turkishName: $('#updateTurkishName').val() || null,
            englishName: $('#updateEnglishName').val() || null,
            genusId: $('#updateGenusId').val() ? parseInt($('#updateGenusId').val()) : null,
            /*familyId: $('#updateFamilyId').val() ? parseInt($('#updateFamilyId').val()) : null,*/
            authorityName: $('#updateAuthorityName').val() || null,
            authorityYear: $('#updateAuthorityYear').val() ? parseInt($('#updateAuthorityYear').val()) : null,
            euName: $('#updateEUName').val() || null,
            fullName: $('#updateFullName').val() || null,
            hesselbarthName: $('#updateHesselbarthName').val() || null,
            turkishNamesTrakel: $('#updateTurkishNamesTrakel').val() || null,
            trakel: $('#updateTrakel').val() || null,
            kocakName: $('#updateKocakName').val() || null
        };

        submitUpdate(formData);
    });
});

function validateAuthorityFieldsUpdate() {
    const authorityName = $('#updateAuthorityName').val();
    const authorityYear = $('#updateAuthorityYear').val();

    if ((authorityName && !authorityYear) || (!authorityName && authorityYear)) {
        Swal.fire({
            title: 'Validation Error!',
            text: 'Both Authority Name and Year must be provided together.',
            icon: 'error',
            confirmButtonText: 'OK'
        });

        // Düğmenin kilidini açın
        $('#updateSpeciesButton').removeAttr('disabled');
        $('#updateSpeciesButton').find('.indicator-label').text('Update');

        return false;
    }

    if (authorityYear && (authorityYear < 1700 || authorityYear > 2100)) {
        Swal.fire({
            title: 'Validation Error!',
            text: 'Authority Year must be between 1700 and 2100.',
            icon: 'error',
            confirmButtonText: 'OK'
        });

        // Düğmenin kilidini açın
        $('#updateSpeciesButton').removeAttr('disabled');
        $('#updateSpeciesButton').find('.indicator-label').text('Update');

        return false;
    }

    return true;
}

function submitUpdate(formData) {
    $.ajax({
        url: `${API_CONFIG.BASE_URL}/Species`,
        type: 'PUT',
        contentType: 'application/json',
        data: JSON.stringify(formData),
        success: function (response, status, xhr) {
            handleUpdateSuccess(response, xhr);
        },
        error: function (xhr, status, error) {
            handleError(xhr, 'update');
        }
    });
}

function handleUpdateSuccess(response, xhr) {
    if (xhr.status === 204 || (response && response.isSuccess)) {
        Swal.fire({
            title: 'Success!',
            text: 'Species updated successfully',
            icon: 'success',
            timer: 2000,
            showConfirmButton: true
        }).then(() => {
            $('#kt_updatemodal_species').modal('hide');
            window.location.reload();
        });
    } else {
        $('#updateSpeciesButton').removeAttr('disabled');
        $('#updateSpeciesButton').find('.indicator-label').text('Update');
        showError(response?.errorList ? response.errorList.join(', ') : 'Failed to update species');
    }
}