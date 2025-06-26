namespace BioWings.Application.DTOs.PermissionDTOs;

/// <summary>
/// Permission sync işleminin sonucunu temsil eden sınıf
/// </summary>
public class PermissionSyncResult
{
    /// <summary>
    /// Eklenen permission sayısı
    /// </summary>
    public int AddedCount { get; set; }

    /// <summary>
    /// Silinen permission sayısı
    /// </summary>
    public int RemovedCount { get; set; }

    /// <summary>
    /// Toplam permission sayısı
    /// </summary>
    public int TotalPermissions { get; set; }
} 