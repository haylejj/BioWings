using System.Net;
using System.Text.Json.Serialization;

namespace BioWings.Application.Results;
public class ServiceResult<T>
{
    public T? Data { get; set; }
    public bool IsSuccess { get; set; }
    public List<string>? ErrorList { get; set; }
    [JsonIgnore]
    public HttpStatusCode StatusCode { get; set; }
    public string? UrlAsCreated { get; set; }

    public static ServiceResult<T> Success(T data, HttpStatusCode statusCode = HttpStatusCode.OK) => new()
    {
        Data = data,
        StatusCode = statusCode,
        IsSuccess = true
    };
    public static ServiceResult<T> SuccessAsCreated(T data, string url) => new()
    {
        Data=data,
        StatusCode=HttpStatusCode.Created,
        UrlAsCreated=url,
        IsSuccess=true
    };
    public static ServiceResult<T> Error(List<string> errorList, HttpStatusCode statusCode = HttpStatusCode.BadRequest) => new()
    {
        ErrorList = errorList,
        StatusCode = statusCode,
        IsSuccess = false
    };
    public static ServiceResult<T> Error(string errorMessage, HttpStatusCode statusCode = HttpStatusCode.BadRequest) => new()
    {
        ErrorList = [errorMessage],
        StatusCode = statusCode,
        IsSuccess = false
    };

}
public class ServiceResult
{
    public bool IsSuccess { get; private set; }
    public List<string>? ErrorList { get; set; }
    [JsonIgnore]
    public HttpStatusCode StatusCode { get; set; }
    public string? UrlAsCreated { get; set; }

    public static ServiceResult Success(HttpStatusCode statusCode = HttpStatusCode.OK) => new()
    {
        StatusCode = statusCode,
        IsSuccess = true
    };
    public static ServiceResult SuccessAsCreated(string url) => new()
    {
        StatusCode=HttpStatusCode.Created,
        UrlAsCreated=url,
        IsSuccess=true
    };
    public static ServiceResult Error(List<string> errorList, HttpStatusCode statusCode = HttpStatusCode.BadRequest) => new()
    {
        ErrorList = errorList,
        StatusCode = statusCode,
        IsSuccess = false
    };
    public static ServiceResult Error(string errorMessage, HttpStatusCode statusCode = HttpStatusCode.BadRequest) => new()
    {
        ErrorList = [errorMessage],
        StatusCode = statusCode,
        IsSuccess = false
    };

}
