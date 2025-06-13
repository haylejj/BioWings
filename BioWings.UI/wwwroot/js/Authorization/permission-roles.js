/**
 * Permission-Role eşleşmeleri sayfası JavaScript fonksiyonları
 */

document.addEventListener('DOMContentLoaded', function() {
    // Arama fonksiyonu
    initializeSearch();
    
    // Form submit işlemi
    initializeForm();
});

/**
 * Tablo arama fonksiyonunu başlatır
 */
function initializeSearch() {
    const searchInput = document.getElementById('permissionSearch');
    const table = document.getElementById('kt_permission_roles_table');
    
    if (!searchInput || !table) return;
    
    const rows = table.getElementsByTagName('tbody')[0].getElementsByTagName('tr');

    searchInput.addEventListener('keyup', function() {
        const filter = this.value.toLowerCase();
        
        for (let i = 0; i < rows.length; i++) {
            const row = rows[i];
            const cells = row.getElementsByTagName('td');
            let found = false;
            
            // İlk 5 sütunda arama yap (roller sütunu hariç)
            for (let j = 0; j < Math.min(cells.length - 1, 5); j++) {
                if (cells[j].textContent.toLowerCase().indexOf(filter) > -1) {
                    found = true;
                    break;
                }
            }
            
            row.style.display = found ? '' : 'none';
        }
    });
}

/**
 * Form işlemlerini başlatır
 */
function initializeForm() {
    const form = document.getElementById('permissionRolesForm');
    if (!form) return;

    let isSubmitting = false;

    form.addEventListener('submit', function (e) {
        // Eğer zaten submit ediliyorsa, tekrar submit etme
        if (isSubmitting) {
            e.preventDefault();
            return;
        }

        isSubmitting = true;

        const saveButton = document.getElementById('saveButton');
        if (saveButton) {
            const originalText = saveButton.innerHTML;
            saveButton.innerHTML = `
                <span class="spinner-border spinner-border-sm me-2" role="status"></span>
                Kaydediliyor...
            `;

            // Görsel olarak tıklanamaz göster ama disable etme
            saveButton.style.pointerEvents = 'none';
            saveButton.style.opacity = '0.65';
        }
    });
}
/**
 * Save butonu için loading durumu gösterir
 */
function showSaveLoading(button) {
    const originalText = button.innerHTML;
    
    button.disabled = true;
    button.innerHTML = `
        <span class="spinner-border spinner-border-sm me-2" role="status"></span>
        Kaydediliyor...
    `;
    
    // 30 saniye sonra timeout (güvenlik için)
    setTimeout(() => {
        button.disabled = false;
        button.innerHTML = originalText;
    }, 30000);
} 