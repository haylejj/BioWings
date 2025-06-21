async function exportObservationsByProvince() {
    const provinceCode = $('#provinceSelect').val();
    if (!provinceCode) {
        Swal.fire({
            text: 'Please select a province first',
            icon: 'warning',
            buttonsStyling: false,
            confirmButtonText: 'OK',
            customClass: {
                confirmButton: 'btn btn-primary'
            }
        });
        return;
    }
    
    try {
        Swal.fire({
            text: 'Export is in progress...',
            icon: 'info',
            allowOutsideClick: false,
            showConfirmButton: false,
            willOpen: () => {
                Swal.showLoading();
            }
        });
        
        const response = await fetch(`/ExportData/${provinceCode}`, {
            method: 'GET'
        });

        if (response.status === 404) {
            Swal.fire({
                text: 'No data found for selected province',
                icon: 'info',
                buttonsStyling: false,
                confirmButtonText: 'OK',
                customClass: {
                    confirmButton: 'btn btn-primary'
                }
            });
            return;
        }

        if (!response.ok) {
            throw new Error('Export failed');
        }

        const blob = await response.blob();

        if (!blob || blob.size === 0) {
            Swal.fire({
                text: 'No data available for export',
                icon: 'info',
                buttonsStyling: false,
                confirmButtonText: 'OK',
                customClass: {
                    confirmButton: 'btn btn-primary'
                }
            });
            return;
        }

        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `Observations_${new Date().toISOString().split('T')[0]}.xlsx`;
        document.body.appendChild(a);
        a.click();
        window.URL.revokeObjectURL(url);
        a.remove();

        Swal.close();
        Swal.fire({
            text: 'Export completed successfully',
            icon: 'success',
            buttonsStyling: false,
            confirmButtonText: 'OK',
            customClass: {
                confirmButton: 'btn btn-primary'
            }
        });
    } catch (error) {
        console.error('Export error:', error);
        Swal.fire({
            text: 'An error occurred during export',
            icon: 'error',
            buttonsStyling: false,
            confirmButtonText: 'OK',
            customClass: {
                confirmButton: 'btn btn-primary'
            }
        });
    }
} 