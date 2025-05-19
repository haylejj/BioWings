// Show Details Modal
function showDetails(observation) {
    try {
        const modal = document.getElementById('kt_modal_observation');

        // Helper function with null check
        const setValue = (elementId, value, suffix = '') => {
            const element = modal.querySelector('#' + elementId);
            if (element) {
                element.textContent = value ? value + suffix : '-';
            }
        };

        // Classification Info
        setValue('modalScientificName', observation.scientificName);
        setValue('modalFamilyName', observation.familyName);
        setValue('modalGenusName', observation.genusName);
        setValue('modalTurkishName', observation.turkishName);
        setValue('modalEnglishName', observation.englishName);
        setValue('modalAuthority', `${observation.authorityName || ''} ${observation.year ? ', ' + observation.year : ''}`);
        setValue('modalName', observation.name);
        setValue('modalEuName', observation.euName);
        setValue('modalFullName', observation.fullName);
        setValue('modalTurkishNamesTrakel', observation.turkishNamesTrakel);
        setValue('modalTrakel', observation.trakel);

        // Location Info
        setValue('modalProvince', observation.provinceName);
        setValue('modalSquareRef', observation.squareRef);

        // Coordinates display - daha güzel formatlama kullanıyoruz
        const coordinates = [];
        const coordinatesElement = modal.querySelector('#modalCoordinates');

        // Önce tüm koordinat bilgilerini HTML formatında oluşturuyoruz
        let coordinatesHTML = '<table class="coordinate-table">';

        if (observation.squareLatitude)
            coordinatesHTML += `<tr><td class="coord-label">Square Latitude:</td><td class="coord-value">${observation.squareLatitude}</td></tr>`;

        if (observation.squareLongitude)
            coordinatesHTML += `<tr><td class="coord-label">Square Longitude:</td><td class="coord-value">${observation.squareLongitude}</td></tr>`;

        if (observation.latitude)
            coordinatesHTML += `<tr><td class="coord-label">Latitude:</td><td class="coord-value">${observation.latitude}</td></tr>`;

        if (observation.longitude)
            coordinatesHTML += `<tr><td class="coord-label">Longitude:</td><td class="coord-value">${observation.longitude}</td></tr>`;

        if (observation.decimalDegrees)
            coordinatesHTML += `<tr><td class="coord-label">Decimal Degrees:</td><td class="coord-value">${observation.decimalDegrees}</td></tr>`;

        if (observation.degreesMinutesSeconds)
            coordinatesHTML += `<tr><td class="coord-label">DMS:</td><td class="coord-value">${observation.degreesMinutesSeconds}</td></tr>`;

        if (observation.decimalMinutes)
            coordinatesHTML += `<tr><td class="coord-label">Decimal Minutes:</td><td class="coord-value">${observation.decimalMinutes}</td></tr>`;

        if (observation.utmCoordinates)
            coordinatesHTML += `<tr><td class="coord-label">UTM:</td><td class="coord-value">${observation.utmCoordinates}</td></tr>`;

        if (observation.mgrsCoordinates)
            coordinatesHTML += `<tr><td class="coord-label">MGRS:</td><td class="coord-value">${observation.mgrsCoordinates}</td></tr>`;

        coordinatesHTML += '</table>';

        // Aynı zamanda düz metin versiyonunu da saklıyoruz (harita için)
        if (observation.latitude) coordinates.push(`Latitude: ${observation.latitude}`);
        if (observation.longitude) coordinates.push(`Longitude: ${observation.longitude}`);

        // HTML olarak koordinat bilgilerini yerleştiriyoruz
        if (coordinatesElement) {
            coordinatesElement.innerHTML = coordinatesHTML;

            // Koordinat tablosu için stil ekliyoruz
            const style = document.createElement('style');
            style.textContent = `
                .coordinate-table {
                    width: 100%;
                    border-collapse: separate;
                    border-spacing: 0;
                }
                .coordinate-table tr {
                    transition: background-color 0.2s;
                }
                .coordinate-table tr:hover {
                    background-color: #f5f8fa;
                }
                .coordinate-table td {
                    padding: 8px 5px;
                    border-bottom: 1px solid #f0f0f0;
                }
                .coord-label {
                    font-weight: 600;
                    color: #5e6278;
                    width: 40%;
                }
                .coord-value {
                    font-family: 'Courier New', monospace;
                    color: #212121;
                    font-weight: 500;
                }
            `;
            document.head.appendChild(style);

            // Görünmeyen bir alana düz metin versiyonunu da saklıyoruz (harita için)
            const plainTextCoordinates = document.createElement('div');
            plainTextCoordinates.id = 'plain-coordinates';
            plainTextCoordinates.style.display = 'none';
            plainTextCoordinates.textContent = coordinates.join('\n');
            coordinatesElement.appendChild(plainTextCoordinates);
        }

        // Altitude
        let altitudeText = '';
        if (observation.altitude1 || observation.altitude2) {
            if (observation.altitude1) altitudeText += observation.altitude1;
            if (observation.altitude2) altitudeText += ` - ${observation.altitude2}`;
            altitudeText += ' metre';
        }
        setValue('modalAltitude', altitudeText);

        setValue('modalUtmReference', observation.utmReference);
        setValue('modalLocationInfo', observation.locationInfo);
        setValue('modalCoordinatePrecisionLevel', getCoordinatePrecisionLevelName(observation.coordinatePrecisionLevel));

        // Observation Details
        setValue('modalObserver', observation.observerFullName);
        setValue('modalObservationDate', observation.observationDate ? new Date(observation.observationDate).toLocaleDateString('tr-TR') : null);
        setValue('modalNumberSeen', observation.numberSeen);
        setValue('modalSex', observation.sex);
        setValue('modalLifeStage', observation.lifeStage);
        setValue('modalNotes', observation.notes);
        setValue('modalSource', observation.source);

        // Show modal
        var myModal = new bootstrap.Modal(modal);
        myModal.show();

    } catch (error) {
        console.error('Error in showDetails:', error);
        Swal.fire({
            title: 'Hata!',
            text: 'Detaylar gösterilirken bir hata oluştu.',
            icon: 'error'
        });
    }
}

// CoordinatePrecisionLevel mapping
function getCoordinatePrecisionLevelName(level) {
    const precisionLevels = {
        0: "Tam/Hassas Koordinat",
        1: "UTM Koordinatı",
        2: "İlçe Koordinatı",
        3: "Kare Koordinatı",
        4: "İl Koordinatı"
    };

    return precisionLevels[level] || "Bilgi yok";
}