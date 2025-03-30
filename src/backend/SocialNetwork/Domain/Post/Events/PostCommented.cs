using Domain.Post.Entities;

namespace Domain.Post.Events;

public class PostCommented
{
    public Comment Comment { get; set; }
    public PostCommented(Comment comment)
    {
        Comment = comment;
    }
}