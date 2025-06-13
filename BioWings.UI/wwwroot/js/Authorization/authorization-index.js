
document.addEventListener('DOMContentLoaded', function() {
    initializeSearch();
    initializeSyncButton();
});

function initializeSearch() {
    const searchInput = document.getElementById('customSearch');
    const table = document.getElementById('kt_authorization_table');
    
    if (!searchInput || !table) return;
    
    const rows = table.getElementsByTagName('tbody')[0].getElementsByTagName('tr');

    searchInput.addEventListener('keyup', function() {
        const filter = this.value.toLowerCase();
        
        for (let i = 0; i < rows.length; i++) {
            const row = rows[i];
            const cells = row.getElementsByTagName('td');
            let found = false;
            
            // İlk 6 sütunda arama yap
            for (let j = 0; j < Math.min(cells.length, 6); j++) {
                if (cells[j].textContent.toLowerCase().indexOf(filter) > -1) {
                    found = true;
                    break;
                }
            }
            
            row.style.display = found ? '' : 'none';
        }
    });
}

function initializeSyncButton() {
    const syncButton = document.getElementById('syncButton');
    const syncForm = document.getElementById('syncForm');
    
    if (!syncButton || !syncForm) return;
    
    syncButton.addEventListener('click', function() {
        showSyncConfirmation(syncForm);
    });
}

/**
 * Senkronizasyon onay dialog'unu gösterir
 * @param {HTMLFormElement} form - Submit edilecek form
 */
function showSyncConfirmation(form) {
    Swal.fire({
        title: 'Yetkileri Güncelle',
        text: 'Yetkilendirme tanımları veritabanıyla senkronize edilecek. Bu işlem biraz zaman alabilir. Devam etmek istiyor musunuz?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#f1b34b',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Evet, güncelle!',
        cancelButtonText: 'İptal',
        reverseButtons: true,
        customClass: {
            confirmButton: 'btn btn-warning',
            cancelButton: 'btn btn-secondary'
        },
        buttonsStyling: false
    }).then((result) => {
        if (result.isConfirmed) {
            showSyncLoading();
            form.submit();
        }
    });
}

function showSyncLoading() {
    Swal.fire({
        title: 'İşlem yapılıyor...',
        text: 'Yetkilendirmeler güncelleniyor, lütfen bekleyin.',
        icon: 'info',
        allowOutsideClick: false,
        showConfirmButton: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
} 