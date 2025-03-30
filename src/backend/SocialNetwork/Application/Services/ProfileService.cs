using Application.Repositories;
using Application.Services.Interfaces;
using Domain.Post.ValueObjects;
using Domain.Profile;
using FluentResults;

namespace Application.Services;

public class ProfileService : IProfileService
{
    private readonly IProfileRepository _profileRepository;
    public ProfileService(IProfileRepository profileRepository) => _profileRepository = profileRepository;
    public async Task<Result<Profile>> Register(RegisterDto registerDto)
    {
        var existenceUser = await _profileRepository.GetByLogin(registerDto.Login);
        if (existenceUser is not null)
        {
            return Result.Fail("User with login exists");
        }

        var createdUserResult = Profile.Create(registerDto.Login, registerDto.Name?.First, registerDto.Name?.Second, registerDto.Name?.Patronymic);
        if (createdUserResult.IsFailed)
        {
            return Result.Fail(createdUserResult.Errors);
        }

        await _profileRepository.Create(createdUserResult.Value);
        return createdUserResult.Value;
    }

    public async Task<Result> Subscribe(Guid subscriberId, Guid subscribeToId)
    {
        var subscribeToUser = await _profileRepository.GetById(subscribeToId);
        if (subscribeToUser is null)
        {
            return Result.Fail("User with id not exists");
        }

        var subscriberUser = await _profileRepository.GetById(subscriberId);
        if (subscriberUser is null)
        {
            return Result.Fail("User with id not exists");
        }

        var result = subscriberUser.AddSubscriber(subscribeToId);
        var result2 = subscribeToUser.AddFollower(subscriberId);

        if (result.IsFailed || result2.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await Task.WhenAll(
            _profileRepository.Update(subscriberUser),
            _profileRepository.Update(subscribeToUser)
        );

        return Result.Ok();
    }

    public async Task<Result<Profile>> GetProfile(Guid userId)
    {
        var userProfile = await _profileRepository.GetById(userId);
        if (userProfile.AvailabilityLevel != Availability.Public)
        {
            return Result.Fail("User does not have access to this profile");
        }

        return userProfile;
    }
}