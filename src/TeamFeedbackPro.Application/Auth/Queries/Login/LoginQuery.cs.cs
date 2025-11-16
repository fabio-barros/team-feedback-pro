using MediatR;
using TeamFeedbackPro.Application.Common;
using TeamFeedbackPro.Application.Common.Models;

namespace TeamFeedbackPro.Application.Auth.Queries.Login;

public record LoginQuery(
    string Email,
    string Password
) : IRequest<Result<AuthenticationResult>>;