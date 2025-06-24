using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.PermissionCommands;

/// <summary>
/// Yetkilendirme tanımlarını veritabanıyla senkronize etme komutu
/// </summary>
public class PermissionSyncCommand : IRequest<ServiceResult<int>>
{
    // Command parametresi yok, tüm permissions sync edilecek
}
