// Export page initialization
$(document).ready(function () {
    $('#startDate, #endDate').prop('disabled', false);
    $('#recordLimit').prop('disabled', false);

    $('#exportAllDates').on('change', function () {
        const isChecked = $(this).is(':checked');
        if (isChecked) {
            $('#dateInputs').addClass('d-none');
            $('#startDate, #endDate').prop('disabled', true);
        } else {
            $('#dateInputs').removeClass('d-none');
            $('#startDate, #endDate').prop('disabled', false);
        }
    });

    $('#exportAllRecords').on('change', function () {
        const isChecked = $(this).is(':checked');
        if (isChecked) {
            $('#limitInputs').addClass('d-none');
            $('#recordLimit').prop('disabled', true);
        } else {
            $('#limitInputs').removeClass('d-none');
            $('#recordLimit').prop('disabled', false);
        }
    });

    try {
        fetch('/Export/GetColumnNames')
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                console.log('MVC Response:', data);
                if (data && data.data) {
                    console.log('Data to render:', data.data);
                    renderColumns(data.data);
                } else {
                    console.error('Data structure is not as expected:', data);
                }
            })
            .catch(error => {
                console.error('MVC Error:', error);
            });

        $("#exportButton").click(handleExport);

    } catch (error) {
        console.error('Error during initialization:', error);
        toastr.error("An error occurred during page initialization");
    }
});