using MediatR;

namespace Domain.Profile.Events;

public class UserFollowed : INotification
{
    public UserFollowed(
        Guid userId,
        Guid followerId)
    {
        UserId = userId;
        FollowerId = followerId;
    }

    /// <summary>
    /// Id человека, который подписался
    /// </summary>
    public Guid FollowerId { get; set; }

    /// <summary>
    ///  Id человека, на которого подписались
    /// </summary>
    public Guid UserId { get; set; }
}