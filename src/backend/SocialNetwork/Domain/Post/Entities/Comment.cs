using Domain.Post.ValueObjects;

namespace Domain.Post.Entities;

public class Comment
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public string Text { get; set; }
    public bool IsRemoved { get; set; }
    
    public IReadOnlyCollection<Like> Likes { get; set; }
}