namespace TeamFeedbackPro.Application.Common.Models;

public record PaginatedResult<T>(
    IEnumerable<T> Items,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages
);