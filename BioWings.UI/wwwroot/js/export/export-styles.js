const exportStyles = `
    .hover-bg-secondary:hover {
        background-color: #f8f9fa !important;
    }
    .cursor-pointer {
        cursor: pointer;
    }
    .dragging {
        opacity: 0.5;
        cursor: move;
    }
    .drag-over {
        border: 2px dashed #009ef7 !important;
        background-color: rgba(0, 158, 247, 0.05);
    }
    .column-item {
        transition: all 0.2s ease;
    }
    .selected-columns {
        min-height: 150px;
        padding: 0.75rem;
        border: 1px dashed #dee2e6;
        display: flex;
        flex-wrap: wrap;
        gap: 0.5rem;
        align-items: flex-start;
    }
    .selected-column-item {
        display: inline-flex;
        align-items: center;
        padding: 0.35rem 0.75rem;
        background: #e8f3ff;
        border: 1px solid #b7d6ff;
        border-radius: 2rem;
        cursor: move;
        user-select: none;
        font-size: 0.875rem;
        color: #0066cc;
        margin: 0;
        transition: all 0.2s ease;
    }
    .selected-column-item .remove-column {
        width: 18px;
        height: 18px;
        padding: 0 !important;
        margin-left: 0.5rem;
        font-size: 0.75rem;
        display: flex;
        align-items: center;
        justify-content: center;
        border-radius: 50%;
        background: transparent;
        border: none;
        color: #0066cc;
        transition: all 0.2s ease;
    }
    .selected-column-item .remove-column:hover {
        background: rgba(0, 102, 204, 0.1);
        color: #004c99;
    }
    .selected-column-item:hover {
        background: #d9ebff;
        border-color: #99c7ff;
    }
    .selected-column-item.dragging {
        opacity: 0.5;
        border: 1px dashed #009ef7;
        background: #f0f9ff;
    }
    .form-check-input {
        width: 2rem !important;
        height: 2rem !important;
        border: 2px solid #B5B5C3 !important;
        cursor: pointer;
        margin: 0 !important;
    }
    .form-check-input:checked {
        background-color: var(--bs-primary) !important;
        border-color: var(--bs-primary) !important;
    }
    .form-check-label {
        cursor: pointer;
        font-size: 1rem;
        margin: 0;
        padding-top: 8px;
        user-select: none;
    }
    .form-check {
        cursor: pointer;
        padding-left: 0;
        display: flex;
        align-items: center;
        gap: 0.5rem;
    }
    input[readonly] {
        background-color: #f5f8fa !important;
        cursor: not-allowed !important;
        pointer-events: none;
    }
    .export-checkbox-container {
        padding: 0.5rem;
        margin-bottom: 1rem;
        display: flex;
        align-items: center;
    }
    input[type="checkbox"][readonly] {
        cursor: not-allowed !important;
    }
    #availableColumns {
        max-height: calc(100vh - 200px);
        overflow-y: auto;
        position: sticky;
        top: 20px;
    }
    .card {
        position: relative;
    }
    .available-columns {
        padding-right: 5px;
    }
    .available-columns::-webkit-scrollbar {
        width: 6px;
    }
    .available-columns::-webkit-scrollbar-track {
        background: #f1f1f1;
    }
    .available-columns::-webkit-scrollbar-thumb {
        background: #888;
        border-radius: 3px;
    }
    .available-columns::-webkit-scrollbar-thumb:hover {
        background: #555;
    }
    .remove-from-available {
        opacity: 0.7;
        transition: opacity 0.2s;
    }
    .remove-from-available:hover {
        opacity: 1;
    }
    .column-item:hover .remove-from-available {
        opacity: 0.9;
    }
`;

$('<style>').text(exportStyles).appendTo('head');