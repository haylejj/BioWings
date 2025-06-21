// Template download functionality
document.querySelector('#downloadTemplate').addEventListener('click', async (e) => {
    e.preventDefault();

    try {
        const response = await fetch(`${API_CONFIG.BASE_URL}/ExcelTemplate/download`, {
            method: 'GET',
            headers: {
                'Accept': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
            },
            credentials: 'same-origin'
        });
        
        if (!response.ok) throw new Error('Download failed');

        const blob = await response.blob();
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = "observation_import_template.xlsx";
        document.body.appendChild(a);
        a.click();
        window.URL.revokeObjectURL(url);
        a.remove();

        toastr.success('Template downloaded successfully');
    } catch (error) {
        console.error('Download error:', error);
        toastr.error('Failed to download template');
    }
});