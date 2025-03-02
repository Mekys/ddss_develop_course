using Application.Dto.Profile;
using Application.Repositories;
using Domain.Profile;
using FluentResults;
using MediatR;

namespace Application.Command;

public class CreateProfileCommand : CreateProfileRequestDto, IRequest<Result<ProfileDto>>
{
    public static CreateProfileCommand GetFrom(CreateProfileRequestDto request)
    {
        return new CreateProfileCommand()
        {
            Login = request.Login,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Patronymic = request.Patronymic
        };
    }
}

class CreateProfileCommandHandler : IRequestHandler<CreateProfileCommand, Result<ProfileDto>>
{
    private readonly IProfileRepository _profileRepository;
    
    public CreateProfileCommandHandler(IProfileRepository profileRepository)
    {
        _profileRepository = profileRepository;
    }
    
    public async Task<Result<ProfileDto>> Handle(CreateProfileCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        
        var createdProfile = Profile.Create(
            request.Login,
            request.FirstName, 
            request.LastName, 
            request.Patronymic);
 
        if (createdProfile.IsFailed)
        {
            return Result.Fail<ProfileDto>(createdProfile.Errors);
        }
        
        var createdObject = await _profileRepository.Create(createdProfile.Value);
        return Result.Ok(ProfileDto.GetFromProfile(createdObject));
    }
}