using BioWings.Application.Features.Commands.EncryptionCommands;
using BioWings.Application.Features.Results.EncryptionResults;
using BioWings.Application.Results;
using BioWings.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace BioWings.Application.Features.Handlers.EncryptionHandlers;
public class DecryptCommandHandler(IEncryptionService encryptionService, ILogger<DecryptCommandHandler> logger) : IRequestHandler<DecryptCommand, ServiceResult<DecryptResult>>
{
    public Task<ServiceResult<DecryptResult>> Handle(DecryptCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Şifrelenmiş token'ı çöz
            var decrypted = encryptionService.Decrypt(request.code);

            // Boş veya null kontrolü
            if (string.IsNullOrEmpty(decrypted))
            {
                return Task.FromResult(ServiceResult<DecryptResult>.Error("Doğrulama kodu çözülemedi."));
            }

            var parts = decrypted.Split('|');
            var result = new DecryptResult();

            if (parts.Length != 2)
            {
                result.IsSuccess = false;
                result.Message = "Geçersiz doğrulama kodu formatı.";
                return Task.FromResult(ServiceResult<DecryptResult>.Error("Geçersiz doğrulama kodu formatı"));
            }

            var email = parts[0];
            var token = parts[1];

            result.Email = email;
            result.Token = token;
            result.IsSuccess = true;

            return Task.FromResult(ServiceResult<DecryptResult>.Success(result));
        }
        catch (FormatException ex)
        {
            // Format hatası (örneğin Base64 çözümleme hatası)
            return Task.FromResult(ServiceResult<DecryptResult>.Error($"Doğrulama kodu formatı hatalı: {ex.Message}"));
        }
        catch (CryptographicException ex)
        {
            // Şifreleme/çözme hatası
            return Task.FromResult(ServiceResult<DecryptResult>.Error($"Şifre çözme işlemi başarısız: {ex.Message}"));
        }
        catch (InvalidOperationException ex)
        {
            // EncryptionService'den fırlatılan özel hata
            return Task.FromResult(ServiceResult<DecryptResult>.Error($"Doğrulama kodu işlemi başarısız: {ex.Message}"));
        }
        catch (Exception ex)
        {
            // Diğer tüm hatalar
            return Task.FromResult(ServiceResult<DecryptResult>.Error($"Beklenmeyen bir hata oluştu: {ex.Message}"));
        }
    }
}
