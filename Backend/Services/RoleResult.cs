using Backend.DTOs;

namespace Backend.Services;

public enum RoleResultStatus
{
    Success,
    NotFound,
    Conflict,
}

public record RoleResult
{
    public RoleResultStatus Status { get; init; }
    public RoleResponse? Data { get; init; }
    public string? Message { get; init; }
}
