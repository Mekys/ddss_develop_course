using Domain.Post.ValueObjects;
using Domain.Profile.Events;
using Domain.Profile.ValueObjects;
using FluentResults;
using MediatR;

namespace Domain.Profile;

public class Profile
{
    private Profile() { }
    private Profile(
        string login,
        Name names)
    {
        Login = login;
        Names = names;

        Id = Guid.NewGuid();
        CreatedAtUtc = DateTime.UtcNow;
        LastLoginAtUtc = DateTime.UtcNow;
        AvailabilityLevel = Availability.Public;
        Subscribers = [];
        Followers = [];
    }

    public Guid Id { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public string Login { get; init; } = null!;
    public DateTime LastLoginAtUtc { get; init; }
    public Name Names { get; init; } = null!;
    public Availability AvailabilityLevel { get; init; }

    /// <summary>
    /// Список подписчиков  
    /// </summary>
    public IReadOnlyCollection<Guid> Subscribers { get; set; } = null!;

    /// <summary>
    /// Список подписок
    /// </summary>
    public IReadOnlyCollection<Guid> Followers { get; set; } = null!;

    public static Result<Profile> Create(
        string login,
        string firstName,
        string lastName,
        string patronymic)
    {
        if (string.IsNullOrWhiteSpace(login))
        {
            return Result.Fail(new RequiredFieldNotSet(nameof(Login)));
        }

        var nameCreateResult = Name.Create(firstName, lastName, patronymic);
        if (nameCreateResult.IsFailed)
        {
            return Result.Fail(nameCreateResult.Errors);
        }

        return new Profile(login, nameCreateResult.Value);
    }

    public Result AddSubscriber(Guid subscriberId)
    {
        if (Subscribers.Contains(subscriberId))
        {
            return Result.Fail(new AlreadySubscribedError());
        }

        Subscribers = [.. Subscribers, subscriberId];
        return Result.Ok();
    }

    public Result AddFollower(Guid followerId, IMediator? mediator = null)
    {
        if (Followers.Contains(followerId))
        {
            return Result.Fail(new AlreadyFollowedError());
        }

        Followers = [.. Followers, followerId];
        mediator?.Publish(new UserFollowed(Id, followerId));
        return Result.Ok();
    }

    public Result RemoveFollower(Guid followerId)
    {
        if (!Followers.Contains(followerId))
        {
            return Result.Fail(new NotFollowedError());
        }

        Followers = Followers.Where(x => x != followerId).ToList();
        return Result.Ok();
    }

    public Result RemoveSubscriber(Guid subscriberId)
    {
        if (!Subscribers.Contains(subscriberId))
        {
            return Result.Fail(new NotSubscribedError());
        }

        Subscribers = Subscribers.Where(x => x != subscriberId).ToList();
        return Result.Ok();
    }
}