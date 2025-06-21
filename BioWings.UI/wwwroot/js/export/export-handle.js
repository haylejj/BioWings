// Export handling function
function handleExport(e) {
    const button = $(e.currentTarget);
    const maxRecords = parseInt($('#recordLimit').attr('max'));
    const exportAllDates = $('#exportAllDates').is(':checked');
    const exportAllRecords = $('#exportAllRecords').is(':checked');

    const selectedColumns = [];
    $('#selectedColumns .selected-column-item').each(function () {
        const $this = $(this);
        selectedColumns.push({
            propertyPath: $this.data('property-path'),
            displayName: $this.data('display-name'),
            tableName: $this.data('table-name')
        });
    });

    if (selectedColumns.length === 0) {
        toastr.warning("Please select at least one column to export");
        return;
    }

    let startDate = null;
    let endDate = null;
    let recordLimit = null;

    // Sadece "Export All Dates" seçili değilse tarih kontrolü yap
    if (!exportAllDates) {
        startDate = $('#startDate').val();
        endDate = $('#endDate').val();
        if (!startDate || !endDate) {
            toastr.warning("Please select both start and end dates or check 'Export All Dates'");
            return;
        }
    }

    // Sadece "Export All Records" seçili değilse kayıt limiti kontrolü yap
    if (!exportAllRecords) {
        recordLimit = parseInt($('#recordLimit').val());
        if (!recordLimit || recordLimit <= 0) {
            toastr.warning("Please enter a valid record limit or check 'Export All Records'");
            return;
        }
        if (recordLimit > maxRecords) {
            toastr.warning(`Record limit cannot exceed ${maxRecords}`);
            return;
        }
    }

    button.attr('data-kt-indicator', 'on');
    button.prop('disabled', true);

    const requestData = {
        columns: selectedColumns,
        startDate: startDate,
        endDate: endDate,
        recordLimit: recordLimit,
        exportAllDates: exportAllDates,
        exportAllRecords: exportAllRecords
    };

    fetch('/Export/ExportData', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(requestData)
    })
        .then(response => {
            if (!response.ok) throw new Error('Export failed');
            return response.blob();
        })
        .then(blob => {
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = `export_${new Date().toISOString().slice(0, 10)}.xlsx`;
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
        })
        .catch(error => {
            console.error('Export error:', error);
            toastr.error("An error occurred during export");
        })
        .finally(() => {
            button.removeAttr('data-kt-indicator');
            button.prop('disabled', false);
        });
}