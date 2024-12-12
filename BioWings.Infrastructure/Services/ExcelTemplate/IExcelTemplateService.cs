namespace BioWings.Infrastructure.Services.ExcelTemplate;
public interface IExcelTemplateService
{
    byte[] CreateImportTemplate();
    byte[] CreateImportTemplateWithMockData();
}
