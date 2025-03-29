using Domain.Post;
using FluentResults;

namespace Application.Services.Interfaces;

public interface IPostService
{
    Task<Result<Post>> Create(CreatePostDto postDto);
    Task<Result> AddLike(Guid postId);
    Task<Result> UnLike(Guid postId);
    Task<Result> Delete(Guid postId);
    Task<Result> AddComment(Guid postId, CreateCommentDto comment);
    Task<Result> DeleteComment(Guid postId, Guid commentId);
    Task<Result<Post>> GetById(Guid id);
}

public class CreateCommentDto
{
    public Guid AuthorId { get; set; }
    public Guid PostId { get; set; }
    public string Text { get; set; }
}

public class CreatePostDto
{
    public Guid ProfileId { get; set; }
    public PostAvailabilityDto PostAvailability { get; private set; }
    public IReadOnlyCollection<MediaDto> Media { get; private set; }
}

public class MediaDto
{
}

public enum PostAvailabilityDto
{
    
}
