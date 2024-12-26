namespace BioWings.Application.Services;
public interface IExcelTemplateService
{
    byte[] CreateImportTemplate();
    byte[] CreateImportTemplateWithMockData();
}
