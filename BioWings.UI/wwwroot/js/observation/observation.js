// Debounce fonksiyonu
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

// DataTable başlatma
var table = $('#observationTable').DataTable({
    "searching": false,
    "paging": false,
    "info": false,
    "ordering": false,
    "order": [[0, "asc"]]
});

// Server-side search implementasyonu
$(document).ready(function () {
    const searchInput = $('#searchInput');

    // View Details butonu için event listener
    $(document).on('click', '.view-details', function () {
        const observation = JSON.parse($(this).attr('data-observation'));
        showDetails(observation);
    });

    // Debounce ile search işlemi
    const debouncedSearch = debounce(function (value) {
        if (value.trim().length === 0) {
            window.location.href = '/Observation/Index';
        } else {
            window.location.href = `/Observation/Index?searchTerm=${encodeURIComponent(value.trim())}&pageNumber=1&pageSize=25`;
        }
    }, 500);

    // Search input event handler
    searchInput.off('keyup').on('keyup', function () {
        debouncedSearch(this.value);
    });

    // URL'den search term'i al ve input'a yerleştir
    const urlParams = new URLSearchParams(window.location.search);
    const searchTerm = urlParams.get('searchTerm');
    if (searchTerm) {
        searchInput.val(decodeURIComponent(searchTerm));
    }
});

function loadPage(pageNumber) {
    const searchTerm = $('#searchInput').val().trim();
    const baseUrl = '/Observation/Index';
    const queryParams = new URLSearchParams(window.location.search);

    // Update or add pageNumber
    queryParams.set('pageNumber', pageNumber);

    // Update or add pageSize if it exists in current URL
    if (!queryParams.has('pageSize')) {
        queryParams.set('pageSize', '25'); // Default page size
    }

    // Update or add searchTerm if it exists
    if (searchTerm) {
        queryParams.set('searchTerm', searchTerm);
    } else {
        queryParams.delete('searchTerm');
    }

    window.location.href = `${baseUrl}?${queryParams.toString()}`;
}

// Arama kısmı için bilgilendirme
document.getElementById('searchInfoBtn').addEventListener('click', function () {
    Swal.fire({
        title: 'Arama İpuçları',
        html: `
            <div class="text-start">
                <p>Arama yaparken aşağıdaki alanlar üzerinde arama yapılmaktadır:</p>
                <ul>
                    <li>Bilimsel İsim (Scientific Name)</li>
                    <li>Cins İsmi (Genus Name)</li>
                    <li>Familya İsmi (Family Name)</li>
                    <li>Hesselbarth İsmi (Hesselbarth Name)</li>
                    <li>İl İsmi (Province Name)</li>
                    <li>Tür İsim (Species Name)</li>
                    <li>Full İsim (Full Name)</li>
                </ul>
                <p>Büyük/küçük harf duyarlılığı olmadan arama yapabilirsiniz.</p>
            </div>
        `,
        icon: 'info',
        confirmButtonText: 'Tamam',
        confirmButtonColor: '#3085d6'
    });
});

// API konfigürasyonu - window.API_CONFIG Layout'tan geliyor
// apiConfig zaten global olarak tanımlı