// species.js - Temel işlemler ve ortak fonksiyonlar

// DataTable initialization
var speciesTable;

// Sayfa yüklendiğinde çalışacak işlemler
$(document).ready(function () {
    // DataTable oluşturma
    speciesTable = $('#speciesTable').DataTable({
        "searching": false,
        "paging": false,
        "info": false,
        "ordering": true,
        "order": [[0, "asc"]]
    });

    // Init fonksiyonlarını çağır
    initializeSearchFunctionality();
    initializeViewDetails();
    initializeCreateModal();
});
  
// Debounce yardımcı fonksiyonu - arama için
function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

// Sayfalama işlemleri
function loadPage(pageNumber) {
    const searchTerm = $('#searchInput').val().trim();
    const baseUrl = '/Species/Index';
    const queryParams = new URLSearchParams(window.location.search);

    queryParams.set('pageNumber', pageNumber);
    const currentPageSize = queryParams.get('pageSize') || 25; // Model.PageSize yerine varsayılan değer
    queryParams.set('pageSize', currentPageSize);

    if (searchTerm) {
        queryParams.set('searchTerm', searchTerm);
    } else {
        queryParams.delete('searchTerm');
    }

    window.location.href = `${baseUrl}?${queryParams.toString()}`;
}

// Arama işlemi
function initializeSearchFunctionality() {
    const searchInput = $('#searchInput');

    const debouncedSearch = debounce(function (value) {
        if (value.trim().length === 0) {
            window.location.href = '/Species/Index';
        } else {
            const currentPageSize = new URLSearchParams(window.location.search).get('pageSize') || 25;
            window.location.href = `/Species/Index?searchTerm=${encodeURIComponent(value.trim())}&pageNumber=1&pageSize=${currentPageSize}`;
        }
    }, 500);

    searchInput.off('keyup').on('keyup', function () {
        debouncedSearch(this.value);
    });

    const urlParams = new URLSearchParams(window.location.search);
    const searchTerm = urlParams.get('searchTerm');
    if (searchTerm) {
        searchInput.val(decodeURIComponent(searchTerm));
    }
}

// Dropdown doldurma işlemleri
async function populateDropdowns(prefix) {
    try {
        await populateDropdown(`${prefix}GenusId`, 'https://localhost:7128/api/Genera', 'Genus');
    } catch (error) {
        console.error('Error loading dropdowns:', error);
        Swal.fire({
            title: 'Error!',
            text: 'Failed to load dropdown data: ' + error.message,
            icon: 'error'
        });
    }
}

// Dropdown doldurma yardımcı fonksiyonu
async function populateDropdown(elementId, url, dropdownType) {
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Failed to fetch ${dropdownType.toLowerCase()} data`);
        }

        const result = await response.json();
        const select = document.getElementById(elementId);
        if (!select) {
            throw new Error(`Select element with id ${elementId} not found`);
        }

        // Reset dropdown
        select.innerHTML = `<option value="">Select ${dropdownType}</option>`;

        // Ensure data exists and is an array
        if (!result || !result.data || !Array.isArray(result.data)) {
            console.warn(`No valid ${dropdownType.toLowerCase()} data received`);
            return;
        }

        // Filter and sort items
        const sortedItems = result.data
            .filter(item =>
                item != null &&
                item.name != null &&
                item.name.trim() !== ''
            )
            .map(item => ({
                id: item.id,
                name: item.name.trim()
            }))
            .sort((a, b) => {
                return a.name.localeCompare(b.name, 'tr-TR');
            });

        // Add options to select
        sortedItems.forEach(item => {
            const option = document.createElement('option');
            option.value = item.id;
            option.textContent = item.name;
            select.appendChild(option);
        });

    } catch (error) {
        console.error(`Error in populateDropdown for ${dropdownType}:`, error);
        throw error;
    }
}

// Hata gösterme
function showError(message) {
    Swal.fire({
        title: 'Error!',
        text: message,
        icon: 'error'
    });
}


// Template indirme
function downloadTemplate() {
    window.location.href = 'https://localhost:7128/api/ExcelTemplate/download/species';
}