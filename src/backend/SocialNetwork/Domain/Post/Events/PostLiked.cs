using Domain.Post.ValueObjects;
using MediatR;

namespace Domain.Post.Events
{
    public class PostLiked : INotification
    {
        public Like Like { get; private set; }
        public Guid PostId { get; private set; }

        public PostLiked(Like like, Guid postId)
        {
            Like = like;
            PostId = postId;
        }

    }
}