using Domain.Post.ValueObjects;
using Domain.Profile.Events;
using Domain.Profile.ValueObjects;
using FluentResults;
using MediatR;

namespace Domain.Profile;

public class Profile
{
    private Profile(){}
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
        Subscribers = new List<Guid>();
        Followers = new List<Guid>();
    }

    public Guid Id { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public string Login { get; set; }
    public DateTime LastLoginAtUtc { get; set; }
    public Name Names { get; set; }
    public Availability AvailabilityLevel { get; set; }

    /// <summary>
    /// Список подписчиков  
    /// </summary>
    public IReadOnlyCollection<Guid> Subscribers { get; set; }

    /// <summary>
    /// Список подписок
    /// </summary>
    public IReadOnlyCollection<Guid> Followers { get; set; }

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