using MediatR;
using Microsoft.Extensions.Logging;
using TeamFeedbackPro.Application.Common;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Application.Common.Models;

namespace TeamFeedbackPro.Application.Auth.Queries.Login;

public class LoginQueryHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator,
    ILogger<LoginQueryHandler> logger
    ) : IRequestHandler<LoginQuery, Result<AuthenticationResult>>
{
    public async Task<Result<AuthenticationResult>> Handle(
        LoginQuery request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Login attempt for email {Email}", request.Email);

        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            logger.LogWarning("Login failed for email {Email} - user not found", request.Email);
            return Result<AuthenticationResult>.Failure(ErrorMessages.InvalidCredentials);
        }

        if (!passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            logger.LogWarning("Login failed for UserId {UserId} (email {Email}) - invalid password", user.Id, request.Email);
            return Result<AuthenticationResult>.Failure(ErrorMessages.InvalidCredentials);
        }

        var token = jwtTokenGenerator.GenerateToken(user.Id, user.Email, user.Role);

        return Result.Success(new AuthenticationResult(
            token,
            user.Id,
            user.Email,
            user.Name,
            user.Role.ToString()
        ));
    }
}