// Sütun başlığına input alanları ekleyip modal açmak yerine
// doğrudan input ile filtreleme yapma
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

    // Sütun başlıklarına input alanları ekle
    $('#observationTable thead tr.text-start').before(`
        <tr class="filter-row">
            <th></th> <!-- ID sütunu boş bırakıldı -->
            <th><input type="text" class="form-control form-control-sm filter-input" data-column="genusName" placeholder="Genus Search"></th>
            <th><input type="text" class="form-control form-control-sm filter-input" data-column="familyName" placeholder="Family Search"></th>
            <th><input type="text" class="form-control form-control-sm filter-input" data-column="scientificName" placeholder="Scientific Search"></th>
            <th><input type="text" class="form-control form-control-sm filter-input" data-column="hesselbarthName" placeholder="Hesselbarth Search"></th>
            <th><input type="text" class="form-control form-control-sm filter-input" data-column="provinceName" placeholder="Province Search"></th>
            <th><input type="text" class="form-control form-control-sm filter-input" data-column="numberSeen" placeholder="Number Search"></th>
            <th><input type="text" class="form-control form-control-sm filter-input" data-column="observationDate" placeholder="Observation Date Search"></th>
            <th></th>
            <th>
                <button id="applyFilters" class="btn btn-sm btn-icon btn-light-primary" title="Filtreleri Uygula">
                    <i class="fas fa-filter"></i>
                </button>
                <button id="clearFilters" class="btn btn-sm btn-icon btn-light-danger ms-1" title="Filtreleri Temizle">
                    <i class="fas fa-times"></i>
                </button>
            </th>
        </tr>
    `);

    // İnput değerlerini URL'den doldur
    if (columnNames.length > 0 && columnValues.length > 0) {
        for (let i = 0; i < columnNames.length; i++) {
            $(`.filter-input[data-column="${columnNames[i]}"]`).val(columnValues[i]);
        }
    }

    // Enter tuşuna basınca filtreleme yap
    $('.filter-input').on('keypress', function (e) {
        if (e.which === 13) {
            applyFilters();
        }
    });

    // Filtre uygulama butonuna tıklama
    $('#applyFilters').on('click', function () {
        applyFilters();
    });

    // Filtreleri temizleme butonuna tıklama
    $('#clearFilters').on('click', function () {
        $('.filter-input').val('');
        clearAllFilters();
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

    // Filtreleri uygula
    function applyFilters() {
        let newColumnNames = [];
        let newColumnValues = [];

        // Dolu inputlardan filtreleri topla
        $('.filter-input').each(function () {
            const value = $(this).val().trim();
            if (value) {
                const column = $(this).data('column');
                newColumnNames.push(column);
                newColumnValues.push(value);
            }
        });

        // Filtre yoksa ana sayfaya dön
        if (newColumnNames.length === 0) {
            window.location.href = '/Observation/Index';
            return;
        }

        // URL oluştur ve yönlendir
        navigateWithFilters(newColumnNames, newColumnValues);
    }

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
});

// Filtreleme alanları için stil ekle
$(document).ready(function () {
    $('<style>').text(`
        .filter-row th {
            padding: 4px 8px;
        }
        .filter-input {
            font-size: 12px;
            height: 30px;
            padding: 2px 8px;
        }
        /* DataTable sıralama oklarını korumak için stil düzenlemeleri */
        .sorting:before, .sorting:after,
        .sorting_asc:before, .sorting_asc:after,
        .sorting_desc:before, .sorting_desc:after {
            bottom: 0.5em !important;
        }
    `).appendTo('head');
});