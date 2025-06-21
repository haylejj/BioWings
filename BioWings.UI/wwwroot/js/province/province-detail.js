async function showDetails(id) {
    try {
        const observationResponse = await fetch(`${API_CONFIG.BASE_URL}/Observations/${id}`);
        if (!observationResponse.ok) {
            throw new Error('Verileri alınırken bir hata oluştu.');
        }
        const item = await observationResponse.json();
        const modalElement = document.getElementById('kt_modal_detailobservation');
        const modal = new bootstrap.Modal(modalElement);

        modal.show();

        // Classification Information
        document.getElementById('modalScientificName').textContent = item.data.scientificName || '';
        document.getElementById('modalFamilyName').textContent = item.data.familyName || '';
        document.getElementById('modalGenusName').textContent = item.data.genusName || '';
        document.getElementById('modalEuName').textContent = item.data.euName || '';
        document.getElementById('modalFullName').textContent = item.data.fullName || '';
        document.getElementById('modalTurkishName').textContent = item.data.turkishName || '';
        document.getElementById('modalEnglishName').textContent = item.data.englishName || '';
        document.getElementById('modalAuthority').textContent = item.data.authorityName && item.data.year ? `${item.data.authorityName}, ${item.data.year}` : '';
        document.getElementById('modalName').textContent = item.data.name || '';
        document.getElementById('modalTurkishNamesTrakel').textContent = item.data.turkishNamesTrakel || '';
        document.getElementById('modalTrakel').textContent = item.data.trakel || '';

        // Location Details
        document.getElementById('modalProvince').textContent = item.data.provinceName || '';
        const coordinates = [];
        if (item.data.squareLatitude) coordinates.push(`Square Latitude: ${item.data.squareLatitude}`);
        if (item.data.squareLongitude) coordinates.push(`Square Longitude: ${item.data.squareLongitude}`);
        if (item.data.latitude) coordinates.push(`Latitude: ${item.data.latitude.toFixed(6)}`);
        if (item.data.longitude) coordinates.push(`Longitude: ${item.data.longitude.toFixed(6)}`);
        if (item.data.decimalDegrees) coordinates.push(`Decimal Degrees: ${item.data.decimalDegrees}`);
        if (item.data.degreesMinutesSeconds) coordinates.push(`DMS: ${item.data.degreesMinutesSeconds}`);
        if (item.data.decimalMinutes) coordinates.push(`Decimal Minutes: ${item.data.decimalMinutes}`);
        if (item.data.utmCoordinates) coordinates.push(`UTM: ${item.data.utmCoordinates}`);
        if (item.data.mgrsCoordinates) coordinates.push(`MGRS: ${item.data.mgrsCoordinates}`);

        document.getElementById('modalCoordinates').textContent = coordinates.join('\n');
        document.getElementById('modalSquareRef').textContent = item.data.squareRef || '';

        let altitudeText = '';
        if (item.data.altitude1) {
            altitudeText = `${item.data.altitude1}m`;
            if (item.data.altitude2) {
                altitudeText += ` - ${item.data.altitude2}m`;
            }
        }
        document.getElementById('modalAltitude').textContent = altitudeText;
        document.getElementById('modalUtmReference').textContent = item.data.utmReference || '';
        document.getElementById('modalCoordinatePrecisionLevel').textContent = getCoordinatePrecisionLevelName(item.data.coordinatePrecisionLevel);
        document.getElementById('modalLocationInfo').textContent = item.data.locationInfo || '';
        document.getElementById('modalObserver').textContent = item.data.observerFullName || '';
        document.getElementById('modalObservationDate').textContent = item.data.observationDate ? new Date(item.data.observationDate).toLocaleDateString() : '';
        document.getElementById('modalNumberSeen').textContent = item.data.numberSeen || '';
        document.getElementById('modalSex').textContent = item.data.sex || '';
        document.getElementById('modalLifeStage').textContent = item.data.lifeStage || '';
        document.getElementById('modalNotes').textContent = item.data.notes || '';
        document.getElementById('modalSource').textContent = item.data.source || '';
    }
    catch (error) {
        console.error('Hata:', error);
        Swal.fire({
            text: 'Gözlem verilerini alınırken bir hata oluştu: ' + error.message,
            icon: 'error',
            buttonsStyling: false,
            confirmButtonText: 'Tamam',
            customClass: {
                confirmButton: 'btn btn-primary'
            }
        });
    }
}

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