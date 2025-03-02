using Domain.Post.ValueObjects;

namespace Domain.Post.Events;

public class PostUnliked 
{
    public PostUnliked(Like like, Guid postId)
    {
        Like = like;
        PostId = postId;
    }

    public Guid PostId { get; set; }
    public Like Like { get; set; }
}