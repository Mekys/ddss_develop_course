using Domain.Post.Entities;
using Domain.Post.ValueObjects;
using MediatR;
using Domain.Post.Events;
using FluentResults;

namespace Domain.Post
{
    public class Post
    {
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public Guid ProfileId { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public bool IsRemoved { get; set; }
        public PostAvailability PostAvailability { get; set; } = new();

        private Post(
            Guid authorId,
            Guid profileId,
            DateTime createdAtUtc,
            PostAvailability postAvailability)
        {
            AuthorId = authorId;
            ProfileId = profileId;
            CreatedAtUtc = createdAtUtc;
            PostAvailability = postAvailability;
            
            Id = Guid.NewGuid();
            Comments = new List<Comment>();
            Likes = new List<Like>();
        }
        
        public IReadOnlyCollection<Comment> Comments { get; set; }
        public IReadOnlyCollection<Like> Likes { get; private set;}

        public void AddLike(Like like, IMediator? mediator = null)
        {
            Likes = [..Likes, like];
            mediator?.Publish(new PostLiked(like, Id));
        }

        public void AddComment(Comment comment, IMediator? mediator = null)
        {
            Comments = [..Comments, comment];
            mediator?.Publish(new PostCommented(comment));
        }

        public void RemoveLike(Like like, IMediator? mediator = null)
        {
            if (!Likes.Contains(like))
            {
                return;
            }
            
            Likes = Likes
                .Where(x => x != like)
                .ToArray();
             
            mediator?.Publish(new PostUnliked(like, Id));
        }

        public void RemoveComment(Guid commentId, IMediator? mediator = null)
        {
            var comment = Comments.FirstOrDefault(comment => comment.Id == commentId);
            if (comment is null)
            {
                return;
            }
            
            comment.IsRemoved = true;
        }

        public Result<Post> Create(
            Guid authorId,
            Guid profileId,
            PostAvailability postAvailability = null)
        {
            if (authorId == Guid.Empty)
            {
                return Result.Fail(new RequiredFieldNotSet(nameof(authorId)));
            }

            if (profileId == Guid.Empty)
            {
                return Result.Fail(new RequiredFieldNotSet(nameof(profileId)));
            }
            
            postAvailability ??= PostAvailability.Default;
            
            return new Post(authorId, profileId, DateTime.UtcNow, postAvailability);
        }
    }
}
