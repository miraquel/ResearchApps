using System.Text.Json.Serialization;

namespace ResearchApps.Service.Vm.Common;

/// <summary>
/// Standard response wrapper for service operations
/// Errors follow RFC 7807 principles via GlobalExceptionFilter
/// </summary>
public class ServiceResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; init; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<string>? Errors { get; init; }
    
    [JsonIgnore]
    public bool IsSuccess => Errors == null || Errors.Count == 0;
    
    [JsonIgnore] // Don't serialize - used by filter
    public int StatusCode { get; init; } = 200;
    
    // 2xx Success
    public static ServiceResponse Success(string? message = null, int statusCode = 200)
        => new() { Message = message, StatusCode = statusCode };

    public static ServiceResponse Created(string? message = null)
        => new() { Message = message, StatusCode = 201 };
    
    public static ServiceResponse NoContent(string? message = null)
        => new() { Message = message, StatusCode = 204 };

    // 4xx Client Errors
    public static ServiceResponse Failure(string errorMessage, int statusCode = 400)
        => new() { Errors = [errorMessage], StatusCode = statusCode };

    public static ServiceResponse Failure(List<string> errors, string? message = null, int statusCode = 400)
        => new() { Message = message, Errors = errors, StatusCode = statusCode };
        
    public static ServiceResponse NotFound(string message = "Resource not found") 
        => Failure(message, 404);
    
    public static ServiceResponse Conflict(string message = "Resource already exists") 
        => Failure(message, 409);
    
    public static ServiceResponse UnprocessableEntity(List<string> validationErrors, string? message = "Validation failed")
        => Failure(validationErrors, message, 422);
}

public class ServiceResponse<T> : ServiceResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T? Data { get; init; }

    // 2xx Success - Only successful responses return data
    public static ServiceResponse<T> Success(T data, string? message = null, int statusCode = 200)
        => new() { Data = data, Message = message, StatusCode = statusCode };
    
    public static ServiceResponse<T> Created(T data, string? message = null)
        => new() { Data = data, Message = message, StatusCode = 201 };
    
    // 4xx Client Errors - Factory methods for error conversion
    public new static ServiceResponse<T> Failure(string errorMessage, int statusCode = 400)
        => new() { Errors = [errorMessage], StatusCode = statusCode };

    public new static  ServiceResponse<T> Failure(List<string> errors, string? message = null, int statusCode = 400)
        => new() { Message = message, Errors = errors, StatusCode = statusCode };
    
    public new static ServiceResponse<T> NotFound(string message = "Resource not found")
        => Failure(message, 404);
    
    public new static ServiceResponse<T> Conflict(string message = "Resource already exists")
        => Failure(message, 409);
    
    public new static ServiceResponse<T> UnprocessableEntity(List<string> validationErrors, string? message = "Validation failed")
        => Failure(validationErrors, message, 422);
}
