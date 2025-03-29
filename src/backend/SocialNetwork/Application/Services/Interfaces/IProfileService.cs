using Domain.Profile;
using FluentResults;

namespace Application.Services.Interfaces;

public interface IProfileService
{
    Task<Result<Profile>> Register(RegisterDto registerDto);
    Task<Result> Subscribe(Guid subscriberId, Guid subscribeToId);
    Task<Result<Profile>> GetProfile(Guid userId);
}