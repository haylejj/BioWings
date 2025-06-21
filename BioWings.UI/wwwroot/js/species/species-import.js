// species-import.js - Import işlemleri

var KTImport = function () {
    var submitButton;
    var form;
    var selectedFile = null;

    // Private functions
    var initForm = function () {
        form = document.querySelector('#kt_import_form');
        submitButton = document.querySelector('#kt_import_submit');

        if (!form || !submitButton) return;

        const dropzone = document.getElementById('importDropzone');
        const fileInput = document.createElement('input');
        fileInput.type = 'file';
        fileInput.accept = '.xlsx';
        fileInput.style.display = 'none';
        document.body.appendChild(fileInput);

        // Form submit handler
        form.addEventListener('submit', function (e) {
            e.preventDefault();

            if (!selectedFile) {
                toastr.error('Lütfen önce bir dosya seçin');
                return;
            }

            submitButton.setAttribute('data-kt-indicator', 'on');
            submitButton.disabled = true;

            const formData = new FormData();
            formData.append('file', selectedFile);

            fetch(`${API_CONFIG.BASE_URL}/Species/Import`, {
                method: 'POST',
                body: formData,
                mode: 'cors',
                credentials: 'include'
            })
                .then(response => {
                    // API'den gelen yanıtı önce JSON'a dönüştürüyoruz
                    return response.json().then(data => {
                        // Başarısız yanıt durumunda
                        if (!response.ok) {
                            // data içindeki errorList'i kullanıyoruz
                            throw {
                                status: response.status,
                                errorList: data.errorList || ['Beklenmeyen bir hata oluştu']
                            };
                        }
                        return data; // Başarılı yanıt
                    });
                })
                .then(result => {
                    Swal.fire({
                        icon: 'success',
                        title: 'Başarılı',
                        text: 'Dosya başarıyla import edildi',
                        showConfirmButton: true
                    }).then((result) => {
                        const modal = document.getElementById('kt_import_modal');
                        const bootstrapModal = bootstrap.Modal.getInstance(modal);
                        if (bootstrapModal) {
                            bootstrapModal.hide();
                        }
                        resetDropzone();

                        if (typeof loadPage === 'function') {
                            loadPage(1);
                        }
                    });
                })
                .catch(error => {
                    if (error.errorList && error.errorList.length > 0) {
                        Swal.fire({
                            title: 'Import Hatası',
                            html: error.errorList.map(msg => `<div class="text-start mb-1">• ${msg}</div>`).join(''),
                            icon: 'error',
                            confirmButtonText: 'Tamam'
                        });
                    } else {
                        toastr.error('Dosya import edilirken bir hata oluştu');
                    }
                })
                .finally(() => {
                    submitButton.removeAttribute('data-kt-indicator');
                    submitButton.disabled = false;
                    selectedFile = null;
                });
        });

        // File handling
        function handleFiles(files) {
            if (files.length > 0) {
                const file = files[0];

                // Dosya boyutu kontrolü (50MB)
                if (file.size > 50 * 1024 * 1024) {
                    toastr.error('Dosya boyutu 50MB\'ı geçemez');
                    return;
                }

                // Dosya tipi kontrolü
                if (!file.name.toLowerCase().endsWith('.xlsx')) {
                    toastr.error('Lütfen sadece XLSX formatında dosya yükleyin');
                    return;
                }

                selectedFile = file;

                // Dosya bilgisini göster
                const dropzone = document.getElementById('importDropzone');
                const fileInfo = document.createElement('div');
                fileInfo.className = 'dz-message needsclick';
                fileInfo.innerHTML = `
                    <i class="bi bi-file-earmark-excel fs-3x text-primary"></i>
                    <div class="ms-4">
                        <h3 class="fs-5 fw-bold text-gray-900 mb-1">${file.name}</h3>
                        <span class="fs-7 fw-semibold text-gray-400 d-block">${(file.size / (1024 * 1024)).toFixed(2)} MB</span>
                    </div>
                `;
                dropzone.innerHTML = '';
                dropzone.appendChild(fileInfo);

                // Submit butonunu aktif et
                submitButton.disabled = false;
            }
        }

        function resetDropzone() {
            const dropzone = document.getElementById('importDropzone');
            dropzone.innerHTML = `
                <div class="dz-message needsclick">
                    <i class="bi bi-file-earmark-excel fs-3x text-primary"></i>
                    <div class="ms-4">
                        <h3 class="fs-5 fw-bold text-gray-900 mb-1">Drop files here or click to upload.</h3>
                        <span class="fs-7 fw-semibold text-gray-400 d-block">Upload Excel files only (.xlsx)</span>
                        <span class="fs-7 fw-semibold text-gray-400 d-block">Maximum file size: 50MB</span>
                    </div>
                </div>
            `;
            submitButton.disabled = true;
            selectedFile = null;
        }

        // Dropzone event listeners
        dropzone.addEventListener('click', () => fileInput.click());
        fileInput.addEventListener('change', (e) => handleFiles(e.target.files));

        ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
            dropzone.addEventListener(eventName, preventDefaults, false);
        });

        function preventDefaults(e) {
            e.preventDefault();
            e.stopPropagation();
        }

        ['dragenter', 'dragover'].forEach(eventName => {
            dropzone.addEventListener(eventName, () => {
                dropzone.classList.add('border-primary');
            });
        });

        ['dragleave', 'drop'].forEach(eventName => {
            dropzone.addEventListener(eventName, () => {
                dropzone.classList.remove('border-primary');
            });
        });

        dropzone.addEventListener('drop', (e) => {
            handleFiles(e.dataTransfer.files);
        });
    };

    return {
        init: function () {
            initForm();
        }
    };
}();

document.addEventListener('DOMContentLoaded', function () {
    KTImport.init();
});