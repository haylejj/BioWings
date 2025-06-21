// Column management functions
function renderColumns(data) {
    const container = $('#availableColumns');
    container.empty();

    Object.keys(data).forEach(groupName => {
        const tableGroup = $(`
            <div class="table-group mb-4">
            <h5 class="fw-bold text-primary mb-2">${groupName}</h5>
            <div class="column-list ps-2">
            </div>
            </div>
        `);

        const columnList = tableGroup.find('.column-list');

        if (Array.isArray(data[groupName])) {
            data[groupName].forEach(column => {
                const columnItem = $(`
                    <div class="column-item p-2 mb-2 bg-light rounded cursor-pointer hover-bg-secondary"
                         draggable="true"
                         data-property-path="${column.propertyPath}"
                         data-display-name="${column.displayName}"
                         data-table-name="${groupName}">
                        <div class="d-flex align-items-center justify-content-between">
                            <div>
                                <i class="fas fa-grip-vertical me-2 text-muted"></i>
                                <span>${column.displayName}</span>
                            </div>
                            <button type="button" class="btn btn-icon btn-sm btn-light-primary remove-from-available d-none">
                                <i class="fas fa-times"></i>
                            </button>
                        </div>
                    </div>
                `);

                columnItem.on('dragstart', function (e) {
                    e.originalEvent.dataTransfer.setData('text/plain', JSON.stringify({
                        propertyPath: column.propertyPath,
                        displayName: column.displayName,
                        tableName: groupName
                    }));
                    $(this).addClass('dragging');
                });

                columnItem.on('dragend', function () {
                    $(this).removeClass('dragging');
                });

                columnItem.find('.remove-from-available').click(function (e) {
                    e.stopPropagation();
                    const propertyPath = columnItem.data('property-path');
                    $(`.selected-column-item[data-property-path="${propertyPath}"]`).remove();
                    updateAvailableColumnButtons();
                });

                columnList.append(columnItem);
            });
        }

        container.append(tableGroup);
    });

    setupDropZone();
    updateAvailableColumnButtons();
}

function updateAvailableColumnButtons() {
    // Reset all remove buttons
    $('.remove-from-available').addClass('d-none');

    // Show remove buttons for selected columns
    $('#selectedColumns .selected-column-item').each(function () {
        const propertyPath = $(this).data('property-path');
        $(`.column-item[data-property-path="${propertyPath}"] .remove-from-available`)
            .removeClass('d-none');
    });
}