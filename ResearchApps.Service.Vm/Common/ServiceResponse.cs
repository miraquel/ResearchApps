using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace ResearchApps.Service.Vm.Common;

public class ServiceResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; init; }
    public List<string> Errors { get; init; } = [];
    public bool IsSuccess => Errors.Count == 0;
    public int StatusCode { get; init; } = StatusCodes.Status200OK;
    
    public static ServiceResponse Success(string message = "Request processed successfully.", int statusCode = StatusCodes.Status200OK)
    {
        return new ServiceResponse
        {
            Message = message,
            StatusCode = statusCode
        };
    }

    public static ServiceResponse Failure(string errorMessage, int statusCode = StatusCodes.Status400BadRequest)
    {
        return new ServiceResponse
        {
            Errors = [errorMessage],
            StatusCode = statusCode
        };
    }
}

public class ServiceResponse<T> : ServiceResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T? Data { get; init; }

    public static ServiceResponse<T> Success(T data, string message = "Request processed successfully.", int statusCode = StatusCodes.Status200OK)
    {
        return new ServiceResponse<T>
        {
            Data = data,
            Message = message,
            StatusCode = statusCode
        };
    }
}
