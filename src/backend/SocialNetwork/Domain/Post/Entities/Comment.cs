using Domain.Post.ValueObjects;
using FluentResults;

namespace Domain.Post.Entities;

public class Comment
{
    private Comment(Guid postId, string text, Guid authorId)
    {
        Id = new Guid();
        PostId = postId;
        Text = text;
        AuthorId = authorId;
    }

    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public string Text { get; set; }
    public Guid AuthorId { get; set; }
    public bool IsRemoved { get; set; }

    public static Result<Comment> Create(Guid postId, string text, Guid authorId)
    {
        if (postId == Guid.Empty)
        {
            return Result.Fail(new RequiredFieldNotSet(nameof(postId)));
        }

        if (String.IsNullOrEmpty(text))
        {
            return Result.Fail(new RequiredFieldNotSet(nameof(text)));
        }

        if (authorId == Guid.Empty)
        {
            return Result.Fail(new RequiredFieldNotSet(nameof(authorId)));
        }

        return new Comment(postId, text, authorId);
    }
}