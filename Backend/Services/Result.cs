namespace Backend.Services;

public enum ResultStatus
{
    Success,
    NotFound,
    Conflict,
    Forbidden,
    Invalid,
    Expired
}

public sealed record Result<T>
{
    public ResultStatus Status { get; init; }
    public T? Data { get; init; }
    public string? Message { get; init; }

    public static Result<T> Ok(T data) =>
        new() { Status = ResultStatus.Success, Data = data };

    public static Result<T> NotFound(string? message = null) =>
        new() { Status = ResultStatus.NotFound, Message = message };

    public static Result<T> Conflict(string message) =>
        new() { Status = ResultStatus.Conflict, Message = message };

    public static Result<T> Forbidden(string? message = null) =>
        new() { Status = ResultStatus.Forbidden, Message = message };

    public static Result<T> Invalid(string? message = null) =>
        new() { Status = ResultStatus.Invalid, Message = message };

    public static Result<T> Expired(string? message = null) =>
        new() { Status = ResultStatus.Expired, Message = message };
}
