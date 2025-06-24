// User Roles Management functionality
let currentUserId = null;
let userRoles = [];


async function updateRole(userId) {
    currentUserId = userId;

    // Modal açılmadan önce loading spinner'ı göster
    document.getElementById('rolesLoadingSpinner').classList.remove('d-none');
    document.getElementById('userRolesContainer').classList.add('d-none');

    // Modal'ı aç
    const rolesModal = new bootstrap.Modal(document.getElementById('kt_modal_user_roles'));
    rolesModal.show();

    try {
        // API'den rolleri çek
        const response = await fetch(`${API_CONFIG.BASE_URL}/UserRole/${userId}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            throw new Error('Kullanıcı rolleri alınamadı');
        }

        const data = await response.json();

        if (data.isSuccess && data.data && data.data.userRoles) {
            userRoles = data.data.userRoles;

            // Kullanıcı bilgilerini al ve kullanıcı adını göster
            await fetchAndDisplayUserName(userId);

            // Rolleri ekranda göster
            displayRoles(userRoles);
        } else {
            throw new Error('Rol verisi bulunamadı');
        }

    } catch (error) {
        console.error('Hata:', error);
        Swal.fire({
            icon: 'error',
            title: 'Hata',
            text: 'Kullanıcı rolleri yüklenirken bir hata oluştu.'
        });
    } finally {
        // Loading spinner'ı gizle, içerik alanını göster
        document.getElementById('rolesLoadingSpinner').classList.add('d-none');
        document.getElementById('userRolesContainer').classList.remove('d-none');
    }
}

// Kullanıcı adını getir ve göster
async function fetchAndDisplayUserName(userId) {
    try {
        const response = await fetch(`${API_CONFIG.BASE_URL}/Users/${userId}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            throw new Error('Kullanıcı bilgileri alınamadı');
        }

        const userData = await response.json();
        if (userData.data && userData.data.fullName) {
            document.getElementById('roleUserFullName').textContent = userData.data.fullName;
        } else {
            document.getElementById('roleUserFullName').textContent = `Kullanıcı ID: ${userId}`;
        }
    } catch (error) {
        console.error('Kullanıcı bilgisi alınamadı:', error);
        document.getElementById('roleUserFullName').textContent = `Kullanıcı ID: ${userId}`;
    }
}

// Rolleri modal içinde göster
function displayRoles(roles) {
    const rolesContent = document.getElementById('rolesContent');
    rolesContent.innerHTML = '';

    if (roles.length === 0) {
        rolesContent.innerHTML = '<div class="alert alert-info">Kullanıcı için tanımlanmış rol bulunamadı.</div>';
        return;
    }

    // Her bir rol için checkbox oluştur
    roles.forEach(role => {
        const roleItem = document.createElement('div');
        roleItem.className = 'form-check form-check-custom form-check-solid mb-5';
        roleItem.innerHTML = `
            <input class="form-check-input role-checkbox"
                   type="checkbox"
                   id="role_${role.roleId}"
                   value="${role.roleId}"
                   ${role.isSelected ? 'checked' : ''} />
            <label class="form-check-label ms-2" for="role_${role.roleId}">
                ${role.roleName}
            </label>
        `;
        rolesContent.appendChild(roleItem);
    });
}

// Rolleri kaydet
document.getElementById('saveUserRoles').addEventListener('click', async function() {
    if (!currentUserId) {
        Swal.fire({
            icon: 'error',
            title: 'Hata',
            text: 'Kullanıcı ID bulunamadı.'
        });
        return;
    }

    // Seçili rolleri topla
    const selectedRoleIds = Array.from(document.querySelectorAll('.role-checkbox:checked'))
        .map(checkbox => parseInt(checkbox.value));

    // Gönderilecek veriyi hazırla (güncellenmiş role listesi)
    const updatedRoles = userRoles.map(role => {
        return {
            roleId: role.roleId,
            roleName: role.roleName,
            isSelected: selectedRoleIds.includes(role.roleId)
        };
    });

    try {
        // Yükleniyor durumunu göster
        Swal.fire({
            title: 'İşleniyor...',
            text: 'Kullanıcı rolleri güncelleniyor',
            allowOutsideClick: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });

        // API'ye güncelleme isteği gönder (PUT metodu ile)
        const response = await fetch(`${API_CONFIG.BASE_URL}/UserRole/Range/${currentUserId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                userRoles: updatedRoles
            })
        });

        if (!response.ok) {
            throw new Error('Roller güncellenirken bir hata oluştu.');
        }

        const result = await response.json();

        if (result.isSuccess) {
            // Başarılı olursa modal'ı kapat ve başarı mesajı göster
            bootstrap.Modal.getInstance(document.getElementById('kt_modal_user_roles')).hide();

            Swal.fire({
                icon: 'success',
                title: 'Başarılı',
                text: 'Kullanıcı rolleri başarıyla güncellendi.',
                timer: 2000,
                timerProgressBar: true
            });
        } else {
            throw new Error(result.errorList ? result.errorList.join(', ') : 'Bilinmeyen bir hata oluştu');
        }

    } catch (error) {
        console.error('Hata:', error);
        Swal.fire({
            icon: 'error',
            title: 'Hata',
            text: error.message || 'Roller güncellenirken bir hata oluştu.'
        });
    }
}); 