async function populateProvinceDropdown2(selectElementId) {
    try {
        const result = await $.ajax({
            url: `${apiConfig.baseUrl}/api/Provinces`,
            type: 'GET',
            contentType: 'application/json'
        });

        if (!result.isSuccess) {
            throw new Error('Failed to fetch provinces');
        }

        const selectElement = document.getElementById(selectElementId);
        selectElement.innerHTML = '<option value="">Select Province</option>';

        const sortedProvinces = result.data.sort((a, b) => {
            return a.name.localeCompare(b.name, 'tr-TR');
        });

        sortedProvinces.forEach(province => {
            const option = document.createElement('option');
            option.value = province.id;
            option.textContent = province.name.trim();
            selectElement.appendChild(option);
        });
    }
    catch (error) {
        console.error('Error loading provinces:', error);
        Swal.fire({
            title: 'Error!',
            text: 'Failed to load provinces: ' + error.message,
            icon: 'error',
            confirmButtonText: 'OK'
        });
    }
}

document.getElementById('kt_createmodal_observation').addEventListener('show.bs.modal', function () {
    populateProvinceDropdown2('createProvinceName');
});

$(document).ready(function () {
    // Doldurulması zorunlu alanlar.
    const requiredFields = [
        'createLatitude',
        'createLongitude',
        'createProvinceName',
        'createName',
        'createScientificName',
        'createObservationDate',
        'createNumberSeen'
    ];

    // Zorunlu alan işaretlerini ekle
    requiredFields.forEach(fieldId => {
        const label = $(`#${fieldId}`).prev('.fs-6.fw-semibold');
        label.html(`${label.html()} <span class="text-danger">*</span>`);
        $(`#${fieldId}`).prop('required', true);
    });

    $('#createObservationButton').on('click', function (e) {
        // Validasyon kontrolü
        let isValid = true;
        let firstInvalidField = null;

        // Zorunlu alanları kontrol et
        requiredFields.forEach(fieldId => {
            const field = $(`#${fieldId}`);
            const value = field.val();

            if (!value || value.trim() === '') {
                isValid = false;
                field.addClass('is-invalid');
                if (!firstInvalidField) firstInvalidField = field;

                if (field.next('.invalid-feedback').length === 0) {
                    field.after(`<div class="invalid-feedback">This field is required</div>`);
                }
            } else {
                field.removeClass('is-invalid');
                field.next('.invalid-feedback').remove();
            }
        });

        // Koordinat validasyonu
        const latitude = parseFloat($('#createLatitude').val());
        const longitude = parseFloat($('#createLongitude').val());

        if (isNaN(latitude) || latitude < -90 || latitude > 90) {
            isValid = false;
            $('#createLatitude').addClass('is-invalid');
            if ($('#createLatitude').next('.invalid-feedback').length === 0) {
                $('#createLatitude').after('<div class="invalid-feedback">Please enter a valid latitude (-90 to 90)</div>');
            }
            if (!firstInvalidField) firstInvalidField = $('#createLatitude');
        }

        if (isNaN(longitude) || longitude < -180 || longitude > 180) {
            isValid = false;
            $('#createLongitude').addClass('is-invalid');
            if ($('#createLongitude').next('.invalid-feedback').length === 0) {
                $('#createLongitude').after('<div class="invalid-feedback">Please enter a valid longitude (-180 to 180)</div>');
            }
            if (!firstInvalidField) firstInvalidField = $('#createLongitude');
        }

        // Authority formatı kontrolü
        const authorityValue = $('#createAuthority').val();
        if (authorityValue && !authorityValue.match(/^.+,\s*\d+$/)) {
            isValid = false;
            $('#createAuthority').addClass('is-invalid');
            if ($('#createAuthority').next('.invalid-feedback').length === 0) {
                $('#createAuthority').after('<div class="invalid-feedback">Authority must be in format "Name, Year"</div>');
            }
            if (!firstInvalidField) firstInvalidField = $('#createAuthority');
        }

        // Observer name surname kontrolü

            const observerValue = $('#createObserverName').val();
            if (observerValue) {
                const nameParts = observerValue.trim().split(/\s+/);
                if (nameParts.length < 2) {
                    isValid = false;
                    $('#createObserverName').addClass('is-invalid');
                    if ($('#createObserverName').next('.invalid-feedback').length === 0) {
                        $('#createObserverName').after('<div class="invalid-feedback">Observer must include both name and surname</div>');
                    }
                    if (!firstInvalidField) firstInvalidField = $('#createObserverName');
                }
            }

            // NumberSeen kontrolü
            const numberSeen = parseInt($('#createNumberSeen').val());
            if (isNaN(numberSeen) || numberSeen <= 0) {
                isValid = false;
                $('#createNumberSeen').addClass('is-invalid');
                if ($('#createNumberSeen').next('.invalid-feedback').length === 0) {
                    $('#createNumberSeen').after('<div class="invalid-feedback">Please enter a valid positive number</div>');
                }
                if (!firstInvalidField) firstInvalidField = $('#createNumberSeen');
            }

            if (!isValid) {
                e.preventDefault();
                if (firstInvalidField) {
                    firstInvalidField[0].scrollIntoView({ behavior: 'smooth', block: 'center' });
                    firstInvalidField.focus();
                }

                Swal.fire({
                    title: 'Validation Error!',
                    text: 'Please fill in all required fields correctly',
                    icon: 'error',
                    confirmButtonText: 'OK'
                });
                return false;
            }

            // Authority parsing
            let authorityName = '';
            let year = 0;
            if (authorityValue) {
                const parts = authorityValue.split(/,(?=[^,]+$)/).map(part => part.trim());
                if (parts.length === 2) {
                    authorityName = parts[0];
                    year = parseInt(parts[1]) || 0;
                }
            }

            // Observer parsing
            let observerName = '';
            let surname = '';
            if (observerValue) {
                const parts = observerValue.split(' ').map(part => part.trim()).filter(part => part);
                if (parts.length >= 2) {
                    surname = parts[parts.length - 1];
                    observerName = parts.slice(0, -1).join(' ');
                }
            }

            // Form data hazırlama
            const formData = {
                // Classification bilgileri
                scientificName: $('#createScientificName').val() || null,
                familyName: $('#createFamilyName').val() || null,
                genusName: $('#createGenusName').val() || null,
                euName: $('#createEUName').val() || null,
                fullName: $('#createFullName').val() || null,
                turkishName: $('#createTurkishName').val() || null,
                englishName: $('#createEnglishName').val() || null,
                name: $('#createName').val() || null,
                turkishNamesTrakel: $('#createTurkishNamesTrakel').val() || null,
                trakel: $('#createTrakel').val() || null,
                kocakName: $('#createKocakName').val() || null,
                hesselbarthName: $('#createHesselbarthName').val() || null,
                authorityName: authorityName || null,
                year: year || 0,

                // Location bilgileri
                provinceId: $('#createProvinceName').val() ? parseInt($('#createProvinceName').val()) : 0,
                squareRef: $('#createSquareRef').val() || null,
                squareLatitude: $('#createSquareLatitude').val() ? parseFloat($('#createSquareLatitude').val()) : 0,
                squareLongitude: $('#createSquareLongitude').val() ? parseFloat($('#createSquareLongitude').val()) : 0,
                latitude: $('#createLatitude').val() ? parseFloat($('#createLatitude').val()) : 0,
                longitude: $('#createLongitude').val() ? parseFloat($('#createLongitude').val()) : 0,
                decimalDegrees: $('#createDecimalDegrees').val() || null,
                degreesMinutesSeconds: $('#createDegreesMinutesSeconds').val() || null,
                decimalMinutes: $('#createDecimalMinutes').val() || null,
                utmCoordinates: $('#createUtmCoordinates').val() || null,
                mgrsCoordinates: $('#createMgrsCoordinates').val() || null,
                altitude1: $('#createAltitude1').val() ? parseFloat($('#createAltitude1').val()) : 0,
                altitude2: $('#createAltitude2').val() ? parseFloat($('#createAltitude2').val()) : 0,
                utmReference: $('#createUtmReference').val() || null,
                locationInfo: $('#createLocationInfo').val() || null,
                coordinatePrecisionLevel: $('#createCoordinatePrecisionLevel').val() ? parseInt($('#createCoordinatePrecisionLevel').val()) : 0,

                // Observation bilgileri
                observerFullName: observerValue || null,
                observerName: observerName || null,
                surname: surname || null,
                observationDate: $('#createObservationDate').val() || null,
                numberSeen: $('#createNumberSeen').val() ? parseInt($('#createNumberSeen').val()) : 0,
                sex: $('#createSex').val() || null,
                lifeStage: $('#createLifeStage').val() || null,
                notes: $('#createNotes').val() || null,
                source: $('#createSource').val() || null
            };

            // AJAX isteği
            $.ajax({
                url: `${apiConfig.baseUrl}/api/Observations`,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(formData),
                success: function (response) {
                    if (response.isSuccess) {
                        Swal.fire({
                            title: 'Success!',
                            text: 'Observation created successfully',
                            icon: 'success',
                            timer: 2000,
                            showConfirmButton: true
                        }).then(() => {
                            $('#kt_createmodal_observation').modal('hide');
                            window.location.reload();
                        });
                    } else {
                        Swal.fire({
                            title: 'Error!',
                            text: response.errorList ? response.errorList.join(', ') : 'Failed to create observation',
                            icon: 'error'
                        });
                    }
                },
                error: function (xhr, status, error) {
                    let errorMessage = 'Failed to create observation';
                    if (xhr.responseJSON && xhr.responseJSON.errorList) {
                        errorMessage = xhr.responseJSON.errorList.join(', ');
                    }
                    Swal.fire({
                        title: 'Error!',
                        text: errorMessage,
                        icon: 'error'
                    });
                }
            });
        });

    // Input değişikliklerinde validasyon mesajlarını temizle
    $('input, select').on('input change', function () {
        $(this).removeClass('is-invalid');
        $(this).next('.invalid-feedback').remove();
    });

    // Modal kapandığında formu ve validasyon mesajlarını temizle
    $('#kt_createmodal_observation').on('hidden.bs.modal', function () {
        $('#create_observation_form')[0].reset();
        $('.is-invalid').removeClass('is-invalid');
        $('.invalid-feedback').remove();
    });
});