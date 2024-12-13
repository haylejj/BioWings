namespace BioWings.Application.Results;
public class ApiResponse<T>
{
    public T Data { get; set; }
    public bool IsSuccess { get; set; }
    public List<string> ErrorList { get; set; }
    public string UrlAsCreated { get; set; }
}

