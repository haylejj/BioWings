// Leaflet haritası için gerekli değişkenler - detay haritası ekledik
let createMap = null;
let updateMap = null;
let detailMap = null;  // Detay haritası için yeni değişken
let createMarker = null;
let updateMarker = null;
let detailMarker = null;  // Detay markerı için yeni değişken

// Sayfa yüklendiğinde çalışacak fonksiyon
$(document).ready(function () {
    // Gerekli CSS dosyalarını dinamik olarak ekle
    if (!document.getElementById('leaflet-css')) {
        const leafletCSS = document.createElement('link');
        leafletCSS.id = 'leaflet-css';
        leafletCSS.rel = 'stylesheet';
        leafletCSS.href = 'https://unpkg.com/leaflet@1.9.4/dist/leaflet.css';
        leafletCSS.integrity = 'sha256-p4NxAoJBhIIN+hmNHrzRCf9tD/miZyoHS5obTRR9BMY=';
        leafletCSS.crossOrigin = '';
        document.head.appendChild(leafletCSS);
    }

    // Gerekli Script dosyalarını dinamik olarak ekle
    if (!window.L) {
        const leafletScript = document.createElement('script');
        leafletScript.src = 'https://unpkg.com/leaflet@1.9.4/dist/leaflet.js';
        leafletScript.integrity = 'sha256-20nQCchB9co0qIjJZRGuk2/Z9VM+kNiyxNV1lvTlZBo=';
        leafletScript.crossOrigin = '';
        document.body.appendChild(leafletScript);

        // Script yüklendikten sonra haritaları hazırla
        leafletScript.onload = initializeMaps;
    } else {
        // Leaflet zaten yüklüyse haritaları hazırla
        initializeMaps();
    }

    // Create modal açıldığında haritayı göster
    $('#kt_createmodal_observation').on('shown.bs.modal', function () {
        setTimeout(() => {
            if (createMap) {
                createMap.invalidateSize();
                setDefaultMapView(createMap);
            }
        }, 300);
    });

    // Update modal açıldığında haritayı göster
    $('#kt_updatemodal_observation').on('shown.bs.modal', function () {
        setTimeout(() => {
            if (updateMap) {
                updateMap.invalidateSize();

                // Eğer enlem boylam değerleri varsa, haritada göster
                const lat = parseFloat($('#updateLatitude').val());
                const lng = parseFloat($('#updateLongitude').val());

                if (!isNaN(lat) && !isNaN(lng) && lat >= -90 && lat <= 90 && lng >= -180 && lng <= 180) {
                    updateMap.setView([lat, lng], 10);
                    if (updateMarker) {
                        updateMarker.setLatLng([lat, lng]);
                    } else {
                        updateMarker = L.marker([lat, lng]).addTo(updateMap);
                    }
                } else {
                    setDefaultMapView(updateMap);
                }
            }
        }, 300);
    });

    // Detay modalı açıldığında haritayı göster - YENİ EKLENEN KOD
    $(document).on('click', '.view-details', function () {
        // observation değişkenini almak için modalın görünür olmasını bekleyelim
        setTimeout(() => {
            // modalCoordinates elementindeki koordinat bilgilerini alalım
            const coordinatesText = $('#modalCoordinates').text();

            // "Latitude:" ve "Longitude:" içeren satırları parse edelim
            const latMatch = coordinatesText.match(/Latitude: ([0-9.-]+)/);
            const lngMatch = coordinatesText.match(/Longitude: ([0-9.-]+)/);

            if (latMatch && lngMatch) {
                const lat = parseFloat(latMatch[1]);
                const lng = parseFloat(lngMatch[1]);

                // Modal görünür olduktan sonra, harita containerını ekleyelim
                if (!document.getElementById('detailMapContainer')) {
                    addDetailMapContainer();
                }

                // Haritayı ekledikten 300ms sonra (animasyon tamamlanması için)
                setTimeout(() => {
                    if (!detailMap && window.L) {
                        initializeDetailMap(lat, lng);
                    } else if (detailMap) {
                        detailMap.invalidateSize();
                        detailMap.setView([lat, lng], 10);

                        // Marker güncelleme
                        if (detailMarker) {
                            detailMarker.setLatLng([lat, lng]);
                        } else {
                            detailMarker = L.marker([lat, lng]).addTo(detailMap);
                        }
                    }
                }, 300);
            }
        }, 300);
    });

    // Create modalındaki enlem boylam inputlarına değişiklik event listeneri ekle
    $('#createLatitude, #createLongitude').on('input change', function () {
        if (!createMap) return;

        const lat = parseFloat($('#createLatitude').val());
        const lng = parseFloat($('#createLongitude').val());

        if (!isNaN(lat) && !isNaN(lng) && lat >= -90 && lat <= 90 && lng >= -180 && lng <= 180) {
            createMap.setView([lat, lng], 10);
            if (createMarker) {
                createMarker.setLatLng([lat, lng]);
            } else {
                createMarker = L.marker([lat, lng]).addTo(createMap);
            }
        }
    });

    // Update modalındaki enlem boylam inputlarına değişiklik event listeneri ekle
    $('#updateLatitude, #updateLongitude').on('input change', function () {
        if (!updateMap) return;

        const lat = parseFloat($('#updateLatitude').val());
        const lng = parseFloat($('#updateLongitude').val());

        if (!isNaN(lat) && !isNaN(lng) && lat >= -90 && lat <= 90 && lng >= -180 && lng <= 180) {
            updateMap.setView([lat, lng], 10);
            if (updateMarker) {
                updateMarker.setLatLng([lat, lng]);
            } else {
                updateMarker = L.marker([lat, lng]).addTo(updateMap);
            }
        }
    });
});

// Haritaları başlatan fonksiyon
function initializeMaps() {
    // Harita containerları için gerekli CSS'i ekle
    const mapStyle = document.createElement('style');
    mapStyle.textContent = `
        .map-container {
            height: 250px;
            margin-bottom: 15px;
            border-radius: 8px;
            overflow: hidden;
            border: 1px solid #e9ecef;
            transition: height 0.3s ease;
        }
        .map-container.expanded {
            height: 450px;
        }
        .map-toggle-button {
            position: absolute;
            top: 10px;
            right: 10px;
            z-index: 1000;
            background: white;
            border: 1px solid #ccc;
            border-radius: 4px;
            padding: 5px 10px;
            cursor: pointer;
            font-size: 12px;
            box-shadow: 0 0 5px rgba(0,0,0,0.1);
        }
        .map-caption {
            margin-top: 5px;
            font-size: 12px;
            color: #6c757d;
            text-align: center;
        }
    `;
    document.head.appendChild(mapStyle);

    // Create modalına harita ekle
    addMapToModal('createMapContainer', 'createLatitude', 'createLongitude').then(map => {
        createMap = map.map;
        createMarker = map.marker;
    });

    // Update modalına harita ekle
    addMapToModal('updateMapContainer', 'updateLatitude', 'updateLongitude').then(map => {
        updateMap = map.map;
        updateMarker = map.marker;
    });
}

// Modala harita ekleyen fonksiyon
async function addMapToModal(containerId, latInputId, lngInputId) {
    return new Promise((resolve) => {
        // Map container için yeni bir div oluştur
        if (!document.getElementById(containerId)) {
            const latInput = document.getElementById(latInputId);
            if (!latInput) {
                console.error(`Latitude input with id ${latInputId} not found`);
                return;
            }

            const latContainer = latInput.closest('.field-container');

            // Harita konteynerini oluştur
            const mapContainer = document.createElement('div');
            mapContainer.id = containerId;
            mapContainer.className = 'map-container';

            // Harita etrafında bir konteyner oluştur
            const mapWrapper = document.createElement('div');
            mapWrapper.className = 'field-container mb-5';
            mapWrapper.style.position = 'relative';

            // Başlık elementi oluştur
            const mapTitle = document.createElement('div');
            mapTitle.className = 'fs-6 fw-semibold mb-2';
            mapTitle.textContent = 'Location on Map';

            // Büyüt/Küçült butonu ekle
            const toggleButton = document.createElement('button');
            toggleButton.type = 'button';
            toggleButton.className = 'map-toggle-button';
            toggleButton.textContent = 'Expand Map';
            toggleButton.onclick = function () {
                const container = document.getElementById(containerId);
                if (container.classList.contains('expanded')) {
                    container.classList.remove('expanded');
                    this.textContent = 'Expand Map';
                } else {
                    container.classList.add('expanded');
                    this.textContent = 'Collapse Map';
                }

                // Haritayı yeniden boyutlandır
                const map = containerId === 'createMapContainer' ? createMap : (containerId === 'updateMapContainer' ? updateMap : detailMap);
                if (map) {
                    setTimeout(() => {
                        map.invalidateSize();
                    }, 300);
                }
            };

            // Yazı ekle
            const caption = document.createElement('div');
            caption.className = 'map-caption';
            caption.textContent = 'Click on the map to set the latitude and longitude values';

            // Elementleri DOM'a ekle
            mapWrapper.appendChild(mapTitle);
            mapWrapper.appendChild(mapContainer);
            mapWrapper.appendChild(toggleButton);
            mapWrapper.appendChild(caption);

            // Modal içindeki konuma ekle (latitude inputundan sonra)
            latContainer.parentNode.insertBefore(mapWrapper, latContainer.nextSibling);
        }

        // Haritayı başlat (Leaflet kütüphanesi hazır olduğunda)
        const checkLeaflet = setInterval(() => {
            if (window.L) {
                clearInterval(checkLeaflet);

                const mapElement = document.getElementById(containerId);
                const map = L.map(containerId).setView([39.92, 32.85], 6); // Türkiye'ye odaklanarak başla

                L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                    attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
                }).addTo(map);

                // Harita tıklama eventi
                map.on('click', function (e) {
                    const lat = e.latlng.lat.toFixed(6);
                    const lng = e.latlng.lng.toFixed(6);

                    // Inputlara değerleri yerleştir
                    document.getElementById(latInputId).value = lat;
                    document.getElementById(lngInputId).value = lng;

                    // Varsa diğer koordinat alanlarını da güncelle
                    updateCoordinateFields(lat, lng, latInputId === 'createLatitude');

                    // Markörü güncelle
                    let marker = null;
                    if (containerId === 'createMapContainer') {
                        if (createMarker) {
                            createMarker.setLatLng([lat, lng]);
                        } else {
                            createMarker = L.marker([lat, lng]).addTo(map);
                        }
                        marker = createMarker;
                    } else {
                        if (updateMarker) {
                            updateMarker.setLatLng([lat, lng]);
                        } else {
                            updateMarker = L.marker([lat, lng]).addTo(map);
                        }
                        marker = updateMarker;
                    }

                    // Input validation sınıflarını güncelle
                    const latInput = document.getElementById(latInputId);
                    const lngInput = document.getElementById(lngInputId);

                    if (latInput.classList.contains('is-invalid')) {
                        latInput.classList.remove('is-invalid');
                        latInput.nextElementSibling?.remove();
                    }

                    if (lngInput.classList.contains('is-invalid')) {
                        lngInput.classList.remove('is-invalid');
                        lngInput.nextElementSibling?.remove();
                    }
                });

                // Harita ve markör referanslarını döndür
                resolve({ map, marker: null });
            }
        }, 100);
    });
}

// Detay modalına harita eklemek için konteyner oluşturan fonksiyon - YENİ EKLENEN FONKSİYON
function addDetailMapContainer() {
    // Koordinatlar elementini bul
    const coordinatesElement = document.getElementById('modalCoordinates');
    if (!coordinatesElement) {
        console.error('Coordinates element not found in details modal');
        return;
    }

    const coordinatesContainer = coordinatesElement.closest('.field-container');

    // Harita konteynerini oluştur
    const mapContainer = document.createElement('div');
    mapContainer.id = 'detailMapContainer';
    mapContainer.className = 'map-container';
    mapContainer.style.marginTop = '15px';

    // Harita etrafında bir konteyner oluştur
    const mapWrapper = document.createElement('div');
    mapWrapper.className = 'field-container mb-5';
    mapWrapper.style.position = 'relative';

    // Başlık elementi oluştur
    const mapTitle = document.createElement('div');
    mapTitle.className = 'fs-6 fw-semibold mb-2';
    mapTitle.textContent = 'Haritada Konum';

    // Büyüt/Küçült butonu ekle
    const toggleButton = document.createElement('button');
    toggleButton.type = 'button';
    toggleButton.className = 'map-toggle-button';
    toggleButton.textContent = 'Haritayı Genişlet';
    toggleButton.onclick = function () {
        const container = document.getElementById('detailMapContainer');
        if (container.classList.contains('expanded')) {
            container.classList.remove('expanded');
            this.textContent = 'Haritayı Genişlet';
        } else {
            container.classList.add('expanded');
            this.textContent = 'Haritayı Daralt';
        }

        // Haritayı yeniden boyutlandır
        if (detailMap) {
            setTimeout(() => {
                detailMap.invalidateSize();
            }, 300);
        }
    };

    // Elementleri DOM'a ekle
    mapWrapper.appendChild(mapTitle);
    mapWrapper.appendChild(mapContainer);
    mapWrapper.appendChild(toggleButton);

    // Coordinates elementinden sonra ekle
    coordinatesContainer.after(mapWrapper);
}

// Detay haritasını başlatan fonksiyon - YENİ EKLENEN FONKSİYON
function initializeDetailMap(lat, lng) {
    if (!window.L) return;

    const mapElement = document.getElementById('detailMapContainer');
    if (!mapElement) return;

    detailMap = L.map('detailMapContainer').setView([lat, lng], 10);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
    }).addTo(detailMap);

    // Markerı ekle
    detailMarker = L.marker([lat, lng]).addTo(detailMap);

    // Popup ekle - daha açıklayıcı bir gösterim için
    detailMarker.bindPopup(`
        <b>Konum bilgisi:</b><br>
        Enlem: ${lat}<br>
        Boylam: ${lng}
    `).openPopup();
}

// Diğer koordinat alanlarını güncelleyen fonksiyon
function updateCoordinateFields(lat, lng, isCreate) {
    const prefix = isCreate ? 'create' : 'update';

    // Decimal Degrees formatında güncelle (varsa)
    const decimalDegreesField = document.getElementById(`${prefix}DecimalDegrees`);
    if (decimalDegreesField) {
        decimalDegreesField.value = `${lat}, ${lng}`;
    }

    // DMS formatında güncelle (varsa)
    const dmsField = document.getElementById(`${prefix}DegreesMinutesSeconds`);
    if (dmsField) {
        dmsField.value = convertToDMS(lat, lng);
    }

    // Decimal Minutes formatında güncelle (varsa)
    const dmField = document.getElementById(`${prefix}DecimalMinutes`);
    if (dmField) {
        dmField.value = convertToDM(lat, lng);
    }
}

// Decimal Degrees'i DMS (Degrees, Minutes, Seconds) formatına çeviren fonksiyon
function convertToDMS(lat, lng) {
    function toDMS(value, isLatitude) {
        const absolute = Math.abs(value);
        const degrees = Math.floor(absolute);
        const minutesNotTruncated = (absolute - degrees) * 60;
        const minutes = Math.floor(minutesNotTruncated);
        const seconds = ((minutesNotTruncated - minutes) * 60).toFixed(1);

        const direction = isLatitude
            ? (value >= 0 ? "N" : "S")
            : (value >= 0 ? "E" : "W");

        return `${degrees}°${minutes}'${seconds}"${direction}`;
    }

    return `${toDMS(parseFloat(lat), true)} ${toDMS(parseFloat(lng), false)}`;
}

// Decimal Degrees'i DM (Degrees, Decimal Minutes) formatına çeviren fonksiyon
function convertToDM(lat, lng) {
    function toDM(value, isLatitude) {
        const absolute = Math.abs(value);
        const degrees = Math.floor(absolute);
        const minutes = ((absolute - degrees) * 60).toFixed(3);

        const direction = isLatitude
            ? (value >= 0 ? "N" : "S")
            : (value >= 0 ? "E" : "W");

        return `${degrees}°${minutes}'${direction}`;
    }

    return `${toDM(parseFloat(lat), true)} ${toDM(parseFloat(lng), false)}`;
}

// Haritayı varsayılan görünüme ayarlayan fonksiyon
function setDefaultMapView(map) {
    // Türkiye'nin merkezi
    map.setView([39.92, 32.85], 6);
}