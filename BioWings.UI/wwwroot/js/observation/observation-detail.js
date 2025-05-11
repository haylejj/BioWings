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

        // Coordinates display
        const coordinates = [];
        if (observation.squareLatitude) coordinates.push(`Square Latitude: ${observation.squareLatitude}`);
        if (observation.squareLongitude) coordinates.push(`Square Longitude: ${observation.squareLongitude}`);
        if (observation.latitude) coordinates.push(`Latitude: ${observation.latitude}`);
        if (observation.longitude) coordinates.push(`Longitude: ${observation.longitude}`);
        if (observation.decimalDegrees) coordinates.push(`Decimal Degrees: ${observation.decimalDegrees}`);
        if (observation.degreesMinutesSeconds) coordinates.push(`DMS: ${observation.degreesMinutesSeconds}`);
        if (observation.decimalMinutes) coordinates.push(`Decimal Minutes: ${observation.decimalMinutes}`);
        if (observation.utmCoordinates) coordinates.push(`UTM: ${observation.utmCoordinates}`);
        if (observation.mgrsCoordinates) coordinates.push(`MGRS: ${observation.mgrsCoordinates}`);

        setValue('modalCoordinates', coordinates.join('\n'));

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