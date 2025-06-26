using BioWings.Application.DTOs.PermissionDTOs;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.PermissionCommands;

/// <summary>
/// Yetkilendirme tanımlarını veritabanıyla senkronize etme komutu
/// Hem yeni permission'ları ekler hem de silinmiş olanları temizler
/// </summary>
public class PermissionSyncCommand : IRequest<ServiceResult<PermissionSyncResult>>
{
    // Command parametresi yok, tüm permissions sync edilecek
}
