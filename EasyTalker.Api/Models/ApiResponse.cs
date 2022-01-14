using System;

namespace EasyTalker.Api.Models;

public class ApiResponse<T>
{
    public static ApiResponse<T> Success(T data) => new(data);
    public static ApiResponse<T> Failure(string error) => new(error);
    public static ApiResponse<T> Failure(Exception exception) => new(exception.Message);

    public T Data { get; }
    public string Error { get; }

    public bool IsError => !string.IsNullOrEmpty(Error);
    public bool IsSuccess => string.IsNullOrEmpty(Error);

    private ApiResponse(T data)
    {
        Data = data;
    }

    private ApiResponse(string error)
    {
        Error = error;
    }
}

public class ApiResponse
{
    public static ApiResponse Success() => new();
    public static ApiResponse Failure(string error) => new(error);
    public static ApiResponse Failure(Exception exception) => new(exception.Message);

    public string Error { get; }

    public bool IsError => !string.IsNullOrEmpty(Error);
    public bool IsSuccess => string.IsNullOrEmpty(Error);

    private ApiResponse()
    {
    }

    private ApiResponse(string error)
    {
        Error = error;
    }
}