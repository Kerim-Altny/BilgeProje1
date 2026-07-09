using Backend.DTOs;

namespace Backend.Services;

public enum UserResultStatus
{
    Success,
    NotFound,
    Conflict,
}
public record UserResult
{
    public UserResultStatus Status { get; init; }
    public UserResponse? Data { get; init; }
    public string? Message { get; init; }
}
