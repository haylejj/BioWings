using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioWings.Application.Results;
public class ApiPaginatedResponse<T>
{
    public ApiPaginatedListResult<T> Data { get; set; }
    public bool IsSuccess { get; set; }
    public List<string> ErrorList { get; set; }
    public string UrlAsCreated { get; set; }
}
