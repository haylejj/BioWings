// Search functionality
$(document).ready(function () {
    const searchInput = $('#searchInput');

    const debouncedSearch = debounce(function (value) {
        if (value.trim().length === 0) {
            window.location.href = '/Family/Index';
        } else {
            const currentPageSize = new URLSearchParams(window.location.search).get('pageSize') || '@Model.PageSize';
            window.location.href = `/Family/Index?searchTerm=${encodeURIComponent(value.trim())}&pageNumber=1&pageSize=${currentPageSize}`;
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
});

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

function loadPage(pageNumber) {
    const searchTerm = $('#searchInput').val().trim();
    const baseUrl = '/Family/Index';
    const queryParams = new URLSearchParams(window.location.search);

    queryParams.set('pageNumber', pageNumber);
    const currentPageSize = queryParams.get('pageSize') || '@Model.PageSize';
    queryParams.set('pageSize', currentPageSize);

    if (searchTerm) {
        queryParams.set('searchTerm', searchTerm);
    } else {
        queryParams.delete('searchTerm');
    }

    window.location.href = `${baseUrl}?${queryParams.toString()}`;
}