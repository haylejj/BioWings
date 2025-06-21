
function initializeViewDetails() {
    $('.view-details').on('click', function () {
        const speciesData = $(this).data('species');
        populateDetailModal(speciesData);
        $('#kt_modal_species').modal('show');
    });
}

function populateDetailModal(speciesData) {
    $('#modalScientificName').text(speciesData.scientificName || '-');
    $('#modalName').text(speciesData.name || '-');
    $('#modalFamilyName').text(speciesData.familyName || '-');
    $('#modalGenusName').text(speciesData.genusName || '-');
    $('#modalAuthorityName').text(speciesData.authorityName || '-');
    $('#modalTurkishName').text(speciesData.turkishName || '-');
    $('#modalEnglishName').text(speciesData.englishName || '-');
    $('#modalEUName').text(speciesData.euName || '-');
    $('#modalFullName').text(speciesData.fullName || '-');
    $('#modalHesselbarthName').text(speciesData.hesselbarthName || '-');
    $('#modalTurkishNamesTrakel').text(speciesData.turkishNamesTrakel || '-');
    $('#modalTrakel').text(speciesData.trakel || '-');
    $('#modalKocakName').text(speciesData.kocakName || '-');
}