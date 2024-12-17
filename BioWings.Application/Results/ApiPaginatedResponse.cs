namespace BioWings.Application.Results;
public class ApiPaginatedResponse<T>
{
    public ApiPaginatedListResult<T> Data { get; set; }
    public bool IsSuccess { get; set; }
    public List<string> ErrorList { get; set; }
    public string UrlAsCreated { get; set; }
}
