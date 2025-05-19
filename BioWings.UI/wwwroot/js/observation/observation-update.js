async function populateProvinceDropdown(selectElement, selectedProvinceId) {
    try {
        const response = await fetch(`${apiConfig.baseUrl}/api/Provinces`);
        if (!response.ok) {
            throw new Error('Failed to fetch provinces');
        }

        const result = await response.json();
        const provinces = result.data;

        // Clear existing options except the first one
        selectElement.innerHTML = '<option value="">Select Province</option>';

        // Sort provinces by name
        provinces.sort((a, b) => a.name.localeCompare(b.name));

        // Add provinces to dropdown
        provinces.forEach(province => {
            const option = document.createElement('option');
            option.value = province.provinceCode;
            option.textContent = province.name.trim(); // Trim to remove any whitespace
            selectElement.appendChild(option);
        });

        // Set selected province if provided
        if (selectedProvinceId) {
            selectElement.value = selectedProvinceId;
        }
    } catch (error) {
        console.error('Error loading provinces:', error);
        // Show error message to user
        Swal.fire({
            text: 'Error loading provinces: ' + error.message,
            icon: 'error',
            buttonsStyling: false,
            confirmButtonText: 'OK',
            customClass: {
                confirmButton: 'btn btn-primary'
            }
        });
    }
}

async function updateObservation(id) {
    try {
        const observationResponse = await fetch(`${apiConfig.baseUrl}/api/Observations/${id}`);
        if (!observationResponse.ok) {
            throw new Error('Verileri alınırken bir hata oluştu.');
        }
        const item = await observationResponse.json();
        const modalElement = document.getElementById('kt_updatemodal_observation');
        const modal = new bootstrap.Modal(modalElement);

        modal.show();

        // Classification bilgileri
        document.getElementById('updateId').value = item.data.id || '';
        document.getElementById('updateScientificName').value = item.data.scientificName || '';
        document.getElementById('updateFamilyName').value = item.data.familyName || '';
        document.getElementById('updateGenusName').value = item.data.genusName || '';
        document.getElementById('updateEUName').value = item.data.euName || '';
        document.getElementById('updateFullName').value = item.data.fullName || '';
        document.getElementById('updateTurkishName').value = item.data.turkishName || '';
        document.getElementById('updateEnglishName').value = item.data.englishName || '';
        document.getElementById('updateAuthority').value = item.data.authorityName && item.data.year ? `${item.data.authorityName},${item.data.year}` : '';
        document.getElementById('updateName').value = item.data.name || '';
        document.getElementById('updateTurkishNamesTrakel').value = item.data.turkishNamesTrakel || '';
        document.getElementById('updateTrakel').value = item.data.trakel || '';
        document.getElementById('updateKocakName').value = item.data.kocakName || '';
        document.getElementById('updateHesselbarthName').value = item.data.hesselbarthName || '';

        // Location bilgileri - Province ID'yi seç
        const provinceSelect = document.getElementById('updateProvinceName');
        await populateProvinceDropdown(provinceSelect, item.data.provinceId);
        document.getElementById('updateSquareRef').value = item.data.squareRef || '';
        document.getElementById('updateSquareLatitude').value = item.data.squareLatitude || '';
        document.getElementById('updateSquareLongitude').value = item.data.squareLongitude || '';
        document.getElementById('updateAltitude1').value = item.data.altitude1 || '';
        document.getElementById('updateAltitude2').value = item.data.altitude2 || '';
        document.getElementById('updateUtmReference').value = item.data.utmReference || '';
        document.getElementById('updateLocationInfo').value = item.data.locationInfo || '';
        document.getElementById('updateDecimalDegrees').value = item.data.decimalDegrees || '';
        document.getElementById('updateDegreesMinutesSeconds').value = item.data.degreesMinutesSeconds || '';
        document.getElementById('updateDecimalMinutes').value = item.data.decimalMinutes || '';
        document.getElementById('updateUtmCoordinates').value = item.data.utmCoordinates || '';
        document.getElementById('updateMgrsCoordinates').value = item.data.mgrsCoordinates || '';

        // CoordinatePrecisionLevel için özel kontrol ekleyelim
        const precisionLevelSelect = document.getElementById('updateCoordinatePrecisionLevel');
        const precisionLevel = item.data.coordinatePrecisionLevel;
        console.log('CoordinatePrecisionLevel from API:', precisionLevel); // Debug log

        if (precisionLevelSelect) {
            precisionLevelSelect.value = precisionLevel !== null && precisionLevel !== undefined ? precisionLevel.toString() : '0';
            console.log('Set precision level to:', precisionLevelSelect.value); // Debug log
        }
        document.getElementById('updateLatitude').value = item.data.latitude || '';
        document.getElementById('updateLongitude').value = item.data.longitude || '';

        // Observation bilgileri
        document.getElementById('updateObserverName').value = item.data.observerFullName || '';
        document.getElementById('updateObservationDate').value = item.data.observationDate ? item.data.observationDate.split('T')[0] : '';
        document.getElementById('updateNumberSeen').value = item.data.numberSeen || '';
        document.getElementById('updateSex').value = item.data.sex || '';
        document.getElementById('updateLifeStage').value = item.data.lifeStage || '';
        document.getElementById('updateNotes').value = item.data.notes || '';
        document.getElementById('updateSource').value = item.data.source || '';

    } catch (error) {
        console.error('Hata:', error);
        Swal.fire({
            text: 'Veriler alınırken bir hata oluştu: ' + error.message,
            icon: 'error',
            buttonsStyling: false,
            confirmButtonText: 'Tamam',
            customClass: {
                confirmButton: 'btn btn-primary'
            }
        });
    }
}

$(document).ready(function () {
    // Doldurulması zorunlu alanlar.
    const requiredFields = [
        'updateLatitude',
        'updateLongitude',
        'updateProvinceName',
        'updateName',
        'updateScientificName',
        'updateObservationDate',
        'updateNumberSeen'
    ];

    // Zorunlu alan işaretlerini ekle
    requiredFields.forEach(fieldId => {
        const label = $(`#${fieldId}`).prev('.fs-6.fw-semibold');
        label.html(`${label.html()} <span class="text-danger">*</span>`);
        $(`#${fieldId}`).prop('required', true);
    });

    $('#updateObservationButton').on('click', function (e) {
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
        const latitude = parseFloat($('#updateLatitude').val());
        const longitude = parseFloat($('#updateLongitude').val());

        if (isNaN(latitude) || latitude < -90 || latitude > 90) {
            isValid = false;
            $('#updateLatitude').addClass('is-invalid');
            if ($('#updateLatitude').next('.invalid-feedback').length === 0) {
                $('#updateLatitude').after('<div class="invalid-feedback">Please enter a valid latitude (-90 to 90)</div>');
            }
            if (!firstInvalidField) firstInvalidField = $('#updateLatitude');
        }

        if (isNaN(longitude) || longitude < -180 || longitude > 180) {
            isValid = false;
            $('#updateLongitude').addClass('is-invalid');
            if ($('#updateLongitude').next('.invalid-feedback').length === 0) {
                $('#updateLongitude').after('<div class="invalid-feedback">Please enter a valid longitude (-180 to 180)</div>');
            }
            if (!firstInvalidField) firstInvalidField = $('#updateLongitude');
        }

        // Authority formatı kontrolü
        const authorityValue = $('#updateAuthority').val();
        if (authorityValue && !authorityValue.match(/^.+,\s*\d+$/)) {
            isValid = false;
            $('#updateAuthority').addClass('is-invalid');
            if ($('#updateAuthority').next('.invalid-feedback').length === 0) {
                $('#updateAuthority').after('<div class="invalid-feedback">Authority must be in format "Name, Year"</div>');
            }
            if (!firstInvalidField) firstInvalidField = $('#updateAuthority');
        }

        // Observer name surname kontrolü
        const observerValue = $('#updateObserverName').val();
        if (observerValue) {
            const nameParts = observerValue.trim().split(/\s+/);
            if (nameParts.length < 2) {
                isValid = false;
                $('#updateObserverName').addClass('is-invalid');
                if ($('#updateObserverName').next('.invalid-feedback').length === 0) {
                    $('#updateObserverName').after('<div class="invalid-feedback">Observer must include both name and surname</div>');
                }
                if (!firstInvalidField) firstInvalidField = $('#updateObserverName');
            }
        }

        // NumberSeen kontrolü
        const numberSeen = parseInt($('#updateNumberSeen').val());
        if (isNaN(numberSeen) || numberSeen <= 0) {
            isValid = false;
            $('#updateNumberSeen').addClass('is-invalid');
            if ($('#updateNumberSeen').next('.invalid-feedback').length === 0) {
                $('#updateNumberSeen').after('<div class="invalid-feedback">Please enter a valid positive number</div>');
            }
            if (!firstInvalidField) firstInvalidField = $('#updateNumberSeen');
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
        const authorityParts = authorityValue ? authorityValue.split(',').map(part => part.trim()) : [];
        const parsedAuthorityName = authorityParts.length === 2 ? authorityParts[0] : '';
        const parsedYear = authorityParts.length === 2 ? parseInt(authorityParts[1]) || 0 : 0;

        // Observer parsing - Gelişmiş, birden fazla isim desteği
        let parsedObserverName = '';
        let parsedSurname = '';

        if (observerValue) {
            const parts = observerValue.trim().split(/\s+/).filter(part => part);

            if (parts.length >= 2) {
                // Son kelime soyad, geri kalanı isim(ler)
                parsedSurname = parts[parts.length - 1];
                parsedObserverName = parts.slice(0, -1).join(' ');
            }
        }

        // ID kontrolü için
        const observationId = $('#updateId').val();
        console.log('Güncellenecek ID:', observationId);

        // Form data hazırlama
        const formData = {
            // Hidden ID
            id: observationId,

            // Classification bilgileri
            scientificName: $('#updateScientificName').val(),
            familyName: $('#updateFamilyName').val(),
            genusName: $('#updateGenusName').val(),
            euName: $('#updateEUName').val(),
            fullName: $('#updateFullName').val(),
            turkishName: $('#updateTurkishName').val(),
            englishName: $('#updateEnglishName').val(),
            name: $('#updateName').val(),
            turkishNamesTrakel: $('#updateTurkishNamesTrakel').val(),
            trakel: $('#updateTrakel').val(),
            kocakName: $('#updateKocakName').val(),
            hesselbarthName: $('#updateHesselbarthName').val(),
            authorityName: parsedAuthorityName,
            year: parsedYear,

            // Location bilgileri
            provinceId: parseInt($('#updateProvinceName').val()) || 0,
            squareRef: $('#updateSquareRef').val(),
            squareLatitude: parseFloat($('#updateSquareLatitude').val()) || 0,
            squareLongitude: parseFloat($('#updateSquareLongitude').val()) || 0,
            latitude: parseFloat($('#updateLatitude').val()) || 0,
            longitude: parseFloat($('#updateLongitude').val()) || 0,
            decimalDegrees: $('#updateDecimalDegrees').val(),
            degreesMinutesSeconds: $('#updateDegreesMinutesSeconds').val(),
            decimalMinutes: $('#updateDecimalMinutes').val(),
            utmCoordinates: $('#updateUtmCoordinates').val(),
            mgrsCoordinates: $('#updateMgrsCoordinates').val(),
            altitude1: parseFloat($('#updateAltitude1').val()) || 0,
            altitude2: parseFloat($('#updateAltitude2').val()) || 0,
            utmReference: $('#updateUtmReference').val(),
            locationInfo: $('#updateLocationInfo').val(),
            coordinatePrecisionLevel: parseInt($('#updateCoordinatePrecisionLevel').val()) || 0,

            // Observation bilgileri
            observerFullName: observerValue,
            observerName: parsedObserverName,
            surname: parsedSurname,
            observationDate: $('#updateObservationDate').val(),
            numberSeen: parseInt($('#updateNumberSeen').val()) || 0,
            sex: $('#updateSex').val(),
            lifeStage: $('#updateLifeStage').val(),
            notes: $('#updateNotes').val(),
            source: $('#updateSource').val()
        };

        console.log('Gönderilen veriler:', formData);

        // AJAX isteği
        $.ajax({
            url: `${apiConfig.baseUrl}/api/Observations`,
            type: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(formData),
            success: function (response) {
                Swal.fire({
                    title: 'Başarılı!',
                    text: 'Gözlem başarıyla güncellendi.',
                    icon: 'success',
                    timer: 2000,
                    showConfirmButton: true
                }).then(() => {
                    window.location.reload();
                });
            },
            error: function (xhr, status, error) {
                let errorMessage = 'Güncelleme işlemi sırasında bir hata oluştu.';
                if (xhr.responseJSON && xhr.responseJSON.errorList) {
                    errorMessage = xhr.responseJSON.errorList.join('<br>');
                }
                Swal.fire({
                    title: 'Hata!',
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

    // Modal kapandığında validasyon mesajlarını temizle
    $('#kt_updatemodal_observation').on('hidden.bs.modal', function () {
        $('.is-invalid').removeClass('is-invalid');
        $('.invalid-feedback').remove();
    });
});