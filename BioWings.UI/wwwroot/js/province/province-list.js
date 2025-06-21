let currentPage = 1;
const pageSize = 25;

$(document).ready(function() {
    loadProvinces();
    setupEventHandlers();
});

async function loadProvinces() {
    try {
        const response = await fetch(`${API_CONFIG.BASE_URL}/Provinces`);
        const result = await response.json();

        if (result.isSuccess) {
            const provinces = result.data.sort((a, b) => a.name.localeCompare(b.name));
            const select = $('#provinceSelect');

            provinces.forEach(province => {
                select.append(new Option(province.name, province.provinceCode));
            });
        }
    } catch (error) {
        console.error('Error loading provinces:', error);
        showError('Failed to load provinces');
    }
}

function setupEventHandlers() {
    $('#searchButton').click(() => {
        currentPage = 1;
        loadObservations();
    });

    $('#provinceSelect').on('keypress', function(e) {
        if (e.which === 13) {
            currentPage = 1;
            loadObservations();
        }
    });
}

function displayObservations(data) {
    const tbody = $('#observationTable tbody');
    tbody.empty();

    if (data.items && data.items.length > 0) {
        data.items.forEach(obs => {
            tbody.append(`
                <tr>
                    <td>${obs.speciesName || '-'}</td>
                    <td>${obs.scientificName || '-'}</td>
                    <td>${obs.familyName || '-'}</td>
                    <td>${obs.genusName || '-'}</td>
                    <td>${obs.authorityName || '-'}${obs.authorityYear ? `, ${obs.authorityYear}` : ''}</td>
                    <td>${obs.latitude ? `${obs.latitude}, ${obs.longitude}` : '-'}</td>
                    <td>${new Date(obs.observationDate).toLocaleDateString('tr-TR')}</td>
                    <td>${obs.numberSeen}</td>
                    <td>
                        <button class="btn btn-sm btn-light-primary" onclick="showDetails(${obs.id})">
                            Details
                        </button>
                    </td>
                </tr>
            `);
        });
        updatePagination(data);
    } else {
        tbody.append(`
            <tr>
                <td colspan="8" class="text-center">No observations found</td>
            </tr>
        `);
    }
}

function updatePagination(data) {
    const paginationInfo = $('#paginationInfo');
    const pagination = $('#pagination');

    paginationInfo.empty();
    pagination.empty();

    if (!data.items || data.items.length === 0) return;

    const totalPages = Math.ceil(data.totalCount / data.pageSize);
    if (totalPages <= 1) return;

    paginationInfo.html(`
        Showing ${(data.pageNumber - 1) * data.pageSize + 1} to ${Math.min(data.pageNumber * data.pageSize, data.totalCount)} of ${data.totalCount} entries
    `);

    let startPage = Math.max(1, data.pageNumber - 2);
    let endPage = Math.min(totalPages, startPage + 4);

    if (endPage - startPage < 4) {
        startPage = Math.max(1, endPage - 4);
    }

    pagination.append(`
        <li class="page-item previous ${data.pageNumber === 1 ? 'disabled' : ''}">
            <a href="javascript:;" onclick="changePage(${data.pageNumber - 1})" class="page-link">
                <i class="previous"></i>
            </a>
        </li>
    `);

    if (startPage > 1) {
        pagination.append(`
            <li class="page-item">
                <a href="javascript:;" onclick="changePage(1)" class="page-link">1</a>
            </li>
        `);
        if (startPage > 2) {
            pagination.append(`
                <li class="page-item">
                    <span class="page-link">...</span>
                </li>
            `);
        }
    }

    for (let i = startPage; i <= endPage; i++) {
        pagination.append(`
            <li class="page-item ${i === data.pageNumber ? 'active' : ''}">
                <a href="javascript:;" onclick="changePage(${i})" class="page-link">${i}</a>
            </li>
        `);
    }

    if (endPage < totalPages) {
        if (endPage < totalPages - 1) {
            pagination.append(`
                <li class="page-item">
                    <span class="page-link">...</span>
                </li>
            `);
        }
        pagination.append(`
            <li class="page-item">
                <a href="javascript:;" onclick="changePage(${totalPages})" class="page-link">${totalPages}</a>
            </li>
        `);
    }

    pagination.append(`
        <li class="page-item next ${data.pageNumber === totalPages ? 'disabled' : ''}">
            <a href="javascript:;" onclick="changePage(${data.pageNumber + 1})" class="page-link">
                <i class="next"></i>
            </a>
        </li>
    `);
}

function changePage(page) {
    if (page < 1) return;

    const provinceCode = $('#provinceSelect').val();
    if (!provinceCode) return;

    currentPage = page;
    loadObservations();
}

async function loadObservations() {
    const provinceCode = $('#provinceSelect').val();

    if (!provinceCode) {
        showError('Please select a province');
        return;
    }

    try {
        const response = await fetch(`${API_CONFIG.BASE_URL}/Observations/ByProvince/${provinceCode}?pageNumber=${currentPage}&pageSize=25`);
        const result = await response.json();

        if (result.isSuccess) {
            displayObservations(result.data);
        } else {
            showError(result.errorList?.join(', ') || 'Failed to load observations');
        }
    } catch (error) {
        console.error('Error loading observations:', error);
        showError('Failed to load observations');
    }
}

function showError(message) {
    Swal.fire({
        text: message,
        icon: 'error',
        buttonsStyling: false,
        confirmButtonText: 'OK',
        customClass: {
            confirmButton: 'btn btn-primary'
        }
    });
} 