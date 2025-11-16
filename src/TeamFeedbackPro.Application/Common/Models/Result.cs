namespace TeamFeedbackPro.Application.Common.Models;

public sealed record Result<T>
{
    public bool IsSuccess { get; init; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; init; }
    public string? Error { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = [];

    private Result(bool isSuccess, T? value, string? error, IReadOnlyList<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        Errors = errors ?? [];
    }

    public static Result<T> Success(T value) => new(true, value, null, null);
    public static Result<T> Failure(string error) => new(false, default, error, null);
    public static Result<T> Failure(IEnumerable<string> errors) => new(false, default, null, errors?.ToList() ?? new List<string>());
}

// Non-generic helper to preserve existing usage style: Result.Success(value)
public static class Result
{
    public static Result<T> Success<T>(T value) => Result<T>.Success(value);
    public static Result<T> Failure<T>(string error) => Result<T>.Failure(error);
    public static Result<T> Failure<T>(IEnumerable<string> errors) => Result<T>.Failure(errors);
}