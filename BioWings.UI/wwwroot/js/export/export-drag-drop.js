// Drag and drop functionality
function setupDropZone() {
    const selectedColumns = $('#selectedColumns');
    selectedColumns.off();

    selectedColumns.on('dragover', function (e) {
        e.preventDefault();
        e.originalEvent.dataTransfer.dropEffect = 'move';
        $(this).addClass('drag-over');
    });

    selectedColumns.on('drop', function (e) {
        e.preventDefault();
        $(this).removeClass('drag-over');

        try {
            const data = JSON.parse(e.originalEvent.dataTransfer.getData('text/plain'));
            const existingColumn = $(`.selected-column-item[data-property-path="${data.propertyPath}"]`);

            if (existingColumn.length === 0) {
                const selectedColumn = createSelectedColumnItem(data);
                setupDragForSelectedColumn(selectedColumn);
                $(this).append(selectedColumn);
                updateAvailableColumnButtons();
            }
        } catch (error) {
            console.error('Error during drop:', error);
        }
    });

    selectedColumns.on('dragleave', function () {
        $(this).removeClass('drag-over');
    });
}

function createSelectedColumnItem(data) {
    return $(`
        <div class="selected-column-item"
             draggable="true"
             data-property-path="${data.propertyPath}"
             data-display-name="${data.displayName}"
             data-table-name="${data.tableName}">
            <span>${data.displayName}</span>
            <button type="button" class="remove-column">
                <i class="fas fa-times"></i>
            </button>
        </div>
    `);
}

function setupDragForSelectedColumn(column) {
    column.on('dragstart', function (e) {
        e.stopPropagation();
        $(this).addClass('dragging');
        e.originalEvent.dataTransfer.setData('text/plain', JSON.stringify({
            propertyPath: $(this).data('property-path'),
            displayName: $(this).data('display-name'),
            tableName: $(this).data('table-name')
        }));
    });

    column.on('dragend', function () {
        $(this).removeClass('dragging');
    });

    column.on('dragover', function (e) {
        e.preventDefault();
        e.stopPropagation();

        const afterElement = getDragAfterElement($(this).parent(), e.clientX);
        const draggable = $('.dragging');

        if (draggable.length && draggable.hasClass('selected-column-item')) {
            if (afterElement == null) {
                $(this).parent().append(draggable);
            } else {
                $(afterElement).before(draggable);
            }
        }
    });

    column.find('.remove-column').click(function () {
        column.remove();
        updateAvailableColumnButtons();
    });
}

function getDragAfterElement(container, x) {
    const draggableElements = [...container.find('.selected-column-item:not(.dragging)')];

    return draggableElements.reduce((closest, child) => {
        const box = child.getBoundingClientRect();
        const offset = x - box.left - box.width / 2;

        if (offset < 0 && offset > closest.offset) {
            return { offset: offset, element: child };
        } else {
            return closest;
        }
    }, { offset: Number.NEGATIVE_INFINITY }).element;
}