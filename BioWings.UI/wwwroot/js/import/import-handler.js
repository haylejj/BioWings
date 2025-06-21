"use strict";

var ImportHandler = function () {
    var submitButton;
    var cancelButton;
    var progressElement;
    var progressBar;
    var progressText;
    var progressPercentage;
    var resultsElement;
    var successCount;
    var errorCount;
    var dropzone;
    let connection;

    const initSignalR = async () => {
        connection = new signalR.HubConnectionBuilder()
            .withUrl(`${API_CONFIG.BASE_URL.replace('/api', '')}/progressHub`)
            .build();

        connection.on("ReceiveProgress", (progress) => {
            updateProgress(progress.progress, progress.message);
            if (progress.status === "Completed") {
                progressElement.classList.add('d-none');
            }
        });

        try {
            await connection.start();
            console.log("SignalR Connected");
        } catch (err) {
            console.error("SignalR connection error:", err);
            setTimeout(initSignalR, 5000);
        }
    };

    // Dropzone configuration
    const initDropzone = () => {
        dropzone = new Dropzone("#importDropzone", {
            url: "#",
            paramName: "file",
            maxFiles: 1,
            maxFilesize: 50,
            acceptedFiles: ".xlsx,.xls",
            autoProcessQueue: false,
            addRemoveLinks: true,
            createImageThumbnails: false,
            headers: {
                'Accept': 'application/json'
            },
            init: function() {
                this.on("addedfile", function(file) {
                    if (this.files.length > 1) {
                        this.removeFile(this.files[0]);
                    }
                    submitButton.disabled = false;
                });

                this.on("removedfile", function() {
                    submitButton.disabled = true;
                    resetUI();
                });

                this.on("error", function(file, errorMessage) {
                    toastr.error(errorMessage);
                });
            },
            dictDefaultMessage: "Drop Excel files here or click to upload",
            dictFileTooBig: "File is too big ({{filesize}}MB). Max filesize: {{maxFilesize}}MB.",
            dictInvalidFileType: "Only Excel files (.xlsx, .xls) are allowed.",
            dictMaxFilesExceeded: "Only one file can be uploaded at a time.",
            dictRemoveFile: "Remove file",
            dictCancelUpload: "Cancel upload"
        });
    };

    // Reset UI state
    const resetUI = () => {
        progressElement.classList.add('d-none');
        resultsElement.classList.add('d-none');
        submitButton.removeAttribute('data-kt-indicator');
        updateProgress(0);
        progressText.textContent = "Processing...";
        successCount.textContent = "0";
        errorCount.textContent = "0";
    };

    // Import edilecek dosya dropzone a eklendi.Api'ye istek atılarak işlem yapılacak.
    const handleSubmit = () => {
        submitButton.addEventListener('click', async e => {
            e.preventDefault();

            if (dropzone.files.length === 0) {
                toastr.error("Please select a file to import");
                return;
            }
            progressElement.classList.remove('d-none');
            const formData = new FormData();
            formData.append('file', dropzone.files[0]);

            try {
                updateProgress(0);
                progressText.textContent = "Starting import...";
                submitButton.setAttribute('data-kt-indicator', 'on');
                submitButton.disabled = true;
                progressElement.classList.remove('d-none');
                resultsElement.classList.add('d-none');

                const response = await fetch(`${API_CONFIG.BASE_URL}/Observations/ImportAllFormat`, {
                    method: 'POST',
                    body: formData,
                    credentials: 'include'
                });

                const result = await response.json();

                if (!response.ok) {
                    throw new Error(result.message || `Import failed with status: ${response.status}`);
                }

                if (result.isSuccess) {
                    toastr.success("Import completed successfully!");
                    dropzone.removeAllFiles();
                } else {
                    toastr.error(result.message || "Import failed");
                }
            } catch (error) {
                console.error('Import error:', error);
                toastr.error(error.message || "An error occurred during import");
            } finally {
                submitButton.removeAttribute('data-kt-indicator');
                submitButton.disabled = !dropzone.files.length;
                progressElement.classList.add('d-none');
            }
        });
    };

    // Update progress bar
    const updateProgress = (value) => {
        const percent = Math.min(100, Math.max(0, value));
        progressBar.style.width = percent + '%';
        progressBar.setAttribute('aria-valuenow', percent);
        progressPercentage.textContent = percent + '%';

        if (percent === 100) {
            progressText.textContent = "Processing complete";
        }
    };

    // Handle cancel operation
    const handleCancel = () => {
        cancelButton.addEventListener('click', e => {
            e.preventDefault();
            dropzone.removeAllFiles(true);
            resetUI();
            submitButton.disabled = true;
            toastr.info("Import cancelled");
        });
    };

    // Public methods
    return {
        init: function () {
            submitButton = document.querySelector('#submitButton');
            cancelButton = document.querySelector('#cancelButton');
            progressElement = document.querySelector('#importProgress');
            progressBar = document.querySelector('#progressBar');
            progressText = document.querySelector('#progressText');
            progressPercentage = document.querySelector('#progressPercentage');
            resultsElement = document.querySelector('#importResults');
            successCount = document.querySelector('#successCount');
            errorCount = document.querySelector('#errorCount');

            submitButton.disabled = true;
            initSignalR();
            initDropzone();
            handleSubmit();
            handleCancel();
        }
    };
}();

// Initialize on document ready
KTUtil.onDOMContentLoaded(function () {
    ImportHandler.init();
});