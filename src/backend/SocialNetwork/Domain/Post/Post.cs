using Domain.Post.Entities;
using Domain.Post.Events;
using Domain.Post.ValueObjects;
using FluentResults;
using MediatR;

namespace Domain.Post
{
    public class Post
    {
        private Post() { }
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public Guid ProfileId { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public bool IsRemoved { get; set; }
        public PostAvailability PostAvailability { get; private set; }
        public IReadOnlyCollection<Comment> Comments { get; private set; }
        public List<Like> Likes { get; private set; }
        public List<Media> Media { get; private set; }

        private Post(
            Guid authorId,
            Guid profileId,
            DateTime createdAtUtc,
            PostAvailability postAvailability,
            List<Media> media)
        {
            AuthorId = authorId;
            ProfileId = profileId;
            CreatedAtUtc = createdAtUtc;
            PostAvailability = postAvailability;
            Media = media;

            Id = Guid.NewGuid();
            Comments = new List<Comment>();
            Likes = new List<Like>();
        }

        public void AddLike(Like like, IMediator? mediator = null)
        {
            Likes = [.. Likes, like];
            mediator?.Publish(new PostLiked(like, Id));
        }

        public void AddComment(Comment comment, IMediator? mediator = null)
        {
            Comments = [.. Comments, comment];
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
                .ToList();

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

        public static Result<Post> Create(
            Guid authorId,
            Guid profileId,
            List<Media> media,
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
            media ??= new List<Media>();

            return new Post(authorId, profileId, DateTime.UtcNow, postAvailability, media);
        }
    }
}