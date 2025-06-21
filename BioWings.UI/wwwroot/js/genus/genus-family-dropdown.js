async function populateFamilyDropdown(selectElementId, selectedFamilyId = null) {
    try {
        const response = await fetch(`${API_CONFIG.BASE_URL}/Families`);
        if (!response.ok) {
            throw new Error('Failed to fetch families');
        }

        const result = await response.json();
        const selectElement = document.getElementById(selectElementId);
        selectElement.innerHTML = '<option value="">Select Family</option>';

        const sortedFamilies = result.data.sort((a, b) => {
            return a.name.localeCompare(b.name, 'tr-TR');
        });

        sortedFamilies.forEach(family => {
            const option = document.createElement('option');
            option.value = family.id;
            option.textContent = family.name.trim();
            selectElement.appendChild(option);
        });

        if (selectedFamilyId) {
            selectElement.value = selectedFamilyId;
        }
    } catch (error) {
        console.error('Error loading families:', error);
        Swal.fire({
            title: 'Error!',
            text: 'Failed to load families: ' + error.message,
            icon: 'error',
            confirmButtonText: 'OK'
        });
    }
}