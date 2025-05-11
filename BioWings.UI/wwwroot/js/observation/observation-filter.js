// Filtre işlevselliği
$(document).ready(function () {
    // URL'den aktif filtreleri al
    const urlParams = new URLSearchParams(window.location.search);
    const columnNames = urlParams.getAll('columnNames');
    const columnValues = urlParams.getAll('columnValues');

    // Eğer aktif filtre varsa, filtre alanını göster ve filtre chip'lerini oluştur
    if (columnNames.length > 0 && columnValues.length > 0) {
        showActiveFilters(columnNames, columnValues);
    }

    // Filtre kaldırma
    $(document).on('click', '.remove-filter', function () {
        const index = $(this).data('index');
        removeFilter(index);
    });

    // Tüm filtreleri temizleme
    $('#clearAllFilters').on('click', function () {
        clearAllFilters();
    });

    // Sütun başlıklarına tıklama işlevselliği
    $('#observationTable thead th').each(function (index) {
        const title = $(this).text().trim();

        // İşlemler sütunu hariç tüm sütunlara filtreleme özelliği ekle
        if (title && !$(this).hasClass('actions-column')) {
            $(this).addClass('filterable-column');
            $(this).css('cursor', 'pointer');

            // Sütun bilgilerini belirle
            let columnName = getColumnNameFromIndex(index);

            if (columnName) {
                $(this).data('column-name', columnName);

                // Tıklama olayı
                $(this).on('click', function () {
                    const colName = $(this).data('column-name');
                    const colTitle = $(this).text().trim();

                    // Filtre modal'ını aç
                    openFilterModal(colName, colTitle);
                });
            }
        }
    });

    // Filtre form submit
    $('#columnFilterForm').on('submit', function (e) {
        e.preventDefault();

        const columnName = $('#columnNameInput').val();
        const columnValue = $('#columnFilterInput').val().trim();

        if (!columnValue) return;

        // Mevcut filtreleri al
        let newColumnNames = [...columnNames];
        let newColumnValues = [...columnValues];

        // Aynı sütun için mevcut filtreyi kaldır
        const existingIndex = newColumnNames.indexOf(columnName);
        if (existingIndex !== -1) {
            newColumnNames.splice(existingIndex, 1);
            newColumnValues.splice(existingIndex, 1);
        }

        // Yeni filtreyi ekle
        newColumnNames.push(columnName);
        newColumnValues.push(columnValue);

        // URL oluştur ve yönlendir
        navigateWithFilters(newColumnNames, newColumnValues);
    });

    // Filtre gösterme fonksiyonu
    function showActiveFilters(names, values) {
        // Filtre container'ını göster
        $('#activeFiltersContainer').removeClass('d-none');

        // Filtre alanını temizle
        const filterArea = $('#activeFiltersArea');
        filterArea.empty();

        // Her filtre için bir chip oluştur
        names.forEach((name, index) => {
            const displayName = getColumnDisplayName(name);
            const value = values[index];

            const chipHtml = `
                <div class="filter-chip">
                    <span class="filter-name">${displayName}:</span>
                    ${value}
                    <button class="remove-filter" data-index="${index}">
                        <i class="fas fa-times"></i>
                    </button>
                </div>
            `;

            filterArea.append(chipHtml);
        });
    }

    // Tek filtre kaldırma
    function removeFilter(index) {
        // Mevcut filtreleri kopyala
        let newColumnNames = [...columnNames];
        let newColumnValues = [...columnValues];

        // Belirtilen indexteki filtreyi kaldır
        newColumnNames.splice(index, 1);
        newColumnValues.splice(index, 1);

        // Eğer hiç filtre kalmadıysa filtresiz sayfaya yönlendir
        if (newColumnNames.length === 0) {
            window.location.href = '/Observation/Index';
            return;
        }

        // Kalan filtrelerle yönlendir
        navigateWithFilters(newColumnNames, newColumnValues);
    }

    // Tüm filtreleri temizle
    function clearAllFilters() {
        window.location.href = '/Observation/Index';
    }

    // Filtrelerle yönlendirme
    function navigateWithFilters(names, values) {
        let url = '/Observation/Index?pageNumber=1&pageSize=25';

        // Arama terimi varsa ekle
        const searchTerm = urlParams.get('searchTerm');
        if (searchTerm) {
            url += `&searchTerm=${encodeURIComponent(searchTerm)}`;
        }

        // Filtreleri ekle
        for (let i = 0; i < names.length; i++) {
            url += `&columnNames=${encodeURIComponent(names[i])}&columnValues=${encodeURIComponent(values[i])}`;
        }

        // Yönlendir
        window.location.href = url;
    }

    // Sütun index'inden sütun adını alma
    function getColumnNameFromIndex(index) {
        switch (index) {
            case 1: return 'genusName';
            case 2: return 'familyName';
            case 3: return 'scientificName';
            case 4: return 'hesselbarthName';
            case 5: return 'provinceName';
            case 6: return 'numberSeen';
            case 7: return 'observationDate';
            default: return null; // Filtrelenemeyen sütunlar
        }
    }

    // Sütun adını gösterim adına çevirme
    function getColumnDisplayName(columnName) {
        switch (columnName) {
            case 'genusName': return 'Genus';
            case 'familyName': return 'Family';
            case 'scientificName': return 'Scientific';
            case 'hesselbarthName': return 'Hesselbarth';
            case 'provinceName': return 'Province';
            case 'numberSeen': return 'Number';
            case 'observationDate': return 'Date';
            default: return columnName;
        }
    }

    // Filtre modalını açma
    function openFilterModal(columnName, columnTitle) {
        // Modal içeriğini ayarla
        $('#columnFilterTitle').text(columnTitle + ' Filtresi');
        $('#columnNameInput').val(columnName);
        $('#columnFilterInput').val('').attr('placeholder', columnTitle + ' için ara...');

        // Modalı göster
        $('#columnFilterModal').modal('show');

        // Input'a odaklan
        setTimeout(() => {
            $('#columnFilterInput').focus();
        }, 500);
    }
});

$(document).ready(function () {
    // Filtrelenebilir sütunlara özel stil ve title ekle
    $('#observationTable thead th').each(function (index) {
        if (index > 0 && index < 8) {
            // Title ekle
            $(this).attr('title', 'Filtrelemek için tıklayınız');

            // Özel stiller ekle
            $(this).css({
                'cursor': 'pointer',
                'position': 'relative',
                'padding-right': '20px',  // İkon için ekstra alan
                'transition': 'color 0.2s ease'
            });

            // Hover ile renk değişimi
            $(this).hover(
                function () {
                    $(this).css('color', '#4d7cff');  // Mavi tonunda hover rengi
                    $(this).find('.filter-icon').css('opacity', '1');
                },
                function () {
                    $(this).css('color', '');  // Normal renge dön
                    $(this).find('.filter-icon').css('opacity', '0.5');
                }
            );

            // Filtre ikonu ekle
            const originalText = $(this).text();
            $(this).html(`
                <div style="display: flex; align-items: center; justify-content: space-between;">
                    <span>${originalText}</span>
                    <i class="fas fa-filter filter-icon" style="font-size: 0.9em; margin-left: 5px; opacity: 0.5;"></i>
                </div>
            `);
        }
    });
});