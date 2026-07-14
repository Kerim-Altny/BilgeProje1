namespace Backend.Services;

public enum ResultStatus
{
    Success,
    NotFound,
    Conflict,
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
}
