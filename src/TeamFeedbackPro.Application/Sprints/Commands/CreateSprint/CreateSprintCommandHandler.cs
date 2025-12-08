using MediatR;
using Microsoft.Extensions.Logging;
using TeamFeedbackPro.Application.Common;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Application.Common.Models;
using TeamFeedbackPro.Domain.Entities;

namespace TeamFeedbackPro.Application.Sprints.Commands.CreateSprint;

public class CreateSprintCommandHandler : IRequestHandler<CreateSprintCommand, Result<SprintResult>>
{
    private readonly IUserRepository _userRepository;
    private readonly ISprintRepository _sprintRepository; 
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateSprintCommandHandler> _logger;

    public CreateSprintCommandHandler(
        IUserRepository userRepository,
        ISprintRepository sprintRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateSprintCommandHandler> logger)
    {
        _userRepository = userRepository;
        _sprintRepository = sprintRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<SprintResult>> Handle(CreateSprintCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating sprint");

        // Validate manager exists
        var manager = await _userRepository.GetByIdAsync(request.ManagerId);
        if (manager is null)
        {
            _logger.LogWarning("Manager not found {managerId}", request.ManagerId);
            return Result.Failure<SprintResult>(ErrorMessages.UserNotFound);
        }

        // Validate manager is associated with a team
        if (!manager.TeamId.HasValue)
        {
            _logger.LogWarning("Team of manager {managerId} not found", request.ManagerId);
            return Result.Failure<SprintResult>(ErrorMessages.TeamNotFound);
        }

        // // Validate if sprint already exist
        // var sprintExist = await _sprintRepository.ExistAsync(
        //     request.StartAt.Date,
        //     request.EndAt.Date,
        //     manager.TeamId.Value
        // );
        // if (sprintExist)
        // {
        //     _logger.LogWarning("Sprint between {start} and {end} already exists", request.StartAt, request.EndAt);
        //     return Result.Failure<SprintResult>(ErrorMessages.SprintAlreadyExistsByDate);
        // }

        var sprint = new Sprint(
            request.Name,
            request.Description,
            request.StartAt,
            request.EndAt,
            manager.TeamId.Value
        );

        await _sprintRepository.AddAsync(sprint, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Sprint created successfully {sprintId}", sprint.Id);

        return Result.Success(new SprintResult(
            sprint.Id,
            sprint.TeamId,
            sprint.Name,
            sprint.Description,
            sprint.StartAt,
            sprint.EndAt,
            sprint.CreatedAt
        ));
    }
}