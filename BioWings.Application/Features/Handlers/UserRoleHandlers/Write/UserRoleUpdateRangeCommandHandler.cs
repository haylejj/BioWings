using BioWings.Application.Features.Commands.UserRoleCommands;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.UserRoleHandlers.Write;
public class UserRoleUpdateRangeCommandHandler(IUserRoleRepository userRoleRepository, ILogger<UserRoleUpdateRangeCommandHandler> logger, IUnitOfWork unitOfWork) : IRequestHandler<UserRoleUpdateRangeCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(UserRoleUpdateRangeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Kullanıcı rolleri güncelleme işlemi başlatıldı. Toplam {roleCount} rol için güncelleme talebi alındı.",
                request.UserRoles.Count);

            if (request.UserRoles == null || !request.UserRoles.Any())
            {
                logger.LogWarning("Güncellenecek rol listesi boş veya null.");
                return ServiceResult.Error("Güncellenecek roller bulunamadı.");
            }

            // Tüm rollerin aynı kullanıcıya ait olduğundan emin olalım
            var userId = request.UserRoles.FirstOrDefault()?.UserId;
            if (userId == 0 || request.UserRoles.Any(r => r.UserId != userId))
            {
                logger.LogWarning("Rol güncelleme işleminde kullanıcı ID'lerinde tutarsızlık tespit edildi.");
                return ServiceResult.Error("Tutarsız kullanıcı bilgileri tespit edildi.");
            }

            // Kullanıcının mevcut rollerini al
            var existingUserRoles = await userRoleRepository.GetUserRolesByUserIdAsync(userId.Value, cancellationToken);
            if (existingUserRoles == null)
            {
                logger.LogWarning("Kullanıcı ID: {userId} için mevcut roller bulunamadı.", userId);
                return ServiceResult.Error($"Kullanıcı ID: {userId} için roller bulunamadı.");
            }

            // Seçili olan roller için yeni rol ekleyin veya mevcut rolü koruyun
            var selectedRoles = request.UserRoles.Where(r => r.IsSelected).ToList();
            foreach (var selectedRole in selectedRoles)
            {
                // Bu rol zaten kullanıcıda yoksa ekle
                if (!existingUserRoles.Any(er => er.RoleId == selectedRole.RoleId))
                {
                    await userRoleRepository.AddUserRoleAsync(userId.Value, selectedRole.RoleId, cancellationToken);
                    logger.LogInformation("Kullanıcı ID: {userId} için yeni rol eklendi: {roleName}", userId, selectedRole.RoleName);
                }
            }

            // Seçili olmayan roller için mevcut rolleri kaldır
            var deselectedRoles = request.UserRoles.Where(r => !r.IsSelected).ToList();
            foreach (var deselectedRole in deselectedRoles)
            {
                // Bu rol kullanıcıda varsa kaldır
                if (existingUserRoles.Any(er => er.RoleId == deselectedRole.RoleId))
                {
                    await userRoleRepository.RemoveUserRoleAsync(userId.Value, deselectedRole.RoleId, cancellationToken);
                    logger.LogInformation("Kullanıcı ID: {userId} için rol kaldırıldı: {roleName}", userId, deselectedRole.RoleName);
                }
            }

            // Değişiklikleri kaydet
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Kullanıcı ID: {userId} için rol güncelleme işlemi başarıyla tamamlandı.", userId);
            return ServiceResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Rol güncelleme işleminde hata oluştu.");
            return ServiceResult.Error("Roller güncellenirken bir hata oluştu: " + ex.Message);
        }
    }
}
