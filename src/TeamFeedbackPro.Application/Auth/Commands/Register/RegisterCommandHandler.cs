using MediatR;
using Microsoft.Extensions.Logging;
using TeamFeedbackPro.Application.Common;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Application.Common.Interfaces;
using TeamFeedbackPro.Application.Common.Models;
using TeamFeedbackPro.Domain.Entities;

namespace TeamFeedbackPro.Application.Auth.Commands.Register;

public sealed class RegisterCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator,
    IUnitOfWork unitOfWork,
    ILogger<RegisterCommandHandler> logger)
    : IRequestHandler<RegisterCommand, Result<RegisterResult>>
{
    public async Task<Result<RegisterResult>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Register attempt for email {Email}", request.Email);

        if (await userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            logger.LogWarning("Registration failed - email already exists: {Email}", request.Email);
            return Result.Failure<RegisterResult>(ErrorMessages.UserAlreadyExists);
        }

        var passwordHash = passwordHasher.HashPassword(request.Password);

        var user = new User(
            request.Email,
            passwordHash,
            request.Name,
            request.Role,
            request.TeamId
        );

        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User created with Id {UserId} for email {Email}", user.Id, user.Email);

        //var token = jwtTokenGenerator.GenerateToken(user.Id, user.Email, user.Role);

        logger.LogInformation("Registration successful for user {UserId}", user.Id);

        return Result.Success(new RegisterResult(
            user.Id,
            user.Email,
            user.Name,
            user.Role,
            user.TeamId,
            user.CreatedAt

        ));
    }
}