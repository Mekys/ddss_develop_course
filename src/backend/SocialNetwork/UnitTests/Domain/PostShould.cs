using Domain.Post;
using Domain.Post.Entities;
using Domain.Post.Events;
using Domain.Post.ValueObjects;
using MediatR;
using Moq;

namespace UnitTests.Domain
{
    public class PostShould
    {
        private readonly Guid _validAuthorId = Guid.NewGuid();
        private readonly Guid _validProfileId = Guid.NewGuid();
        private readonly Guid _validLikedById = Guid.NewGuid();
        private readonly Guid _validCommentAuthorId = Guid.NewGuid();
        private readonly string _validCommentText = "Test comment";
        private readonly PostAvailability _validPostAvailability = PostAvailability.Default;

        private readonly string _validMimeType = "image/jpeg";
        private readonly string _validMediaUrl = "https://example.com/image.jpg";
        private readonly List<Media> _validMedia;

        public PostShould()
        {
            _validMedia = new List<Media>
            {
                Media.Create(_validMimeType, _validMediaUrl).Value
            };
        }

        [Fact]
        public void Create_WithValidParameters_ReturnsPost()
        {
            // Act
            var result = Post.Create(_validAuthorId, _validProfileId, _validMedia, _validPostAvailability);

            // Assert
            Assert.True(result.IsSuccess);
            var post = result.Value;
            Assert.Equal(_validAuthorId, post.AuthorId);
            Assert.Equal(_validProfileId, post.ProfileId);
            Assert.Equal(_validPostAvailability, post.PostAvailability);
            Assert.Equal(_validMedia, post.Media);
            Assert.NotEqual(Guid.Empty, post.Id);
            Assert.NotEqual(default(DateTime), post.CreatedAtUtc);
            Assert.Empty(post.Comments);
            Assert.Empty(post.Likes);
            Assert.False(post.IsRemoved);
        }

        [Fact]
        public void Create_WithEmptyAuthorId_ReturnsFailure()
        {
            // Act
            var result = Post.Create(Guid.Empty, _validProfileId, _validMedia);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Contains(result.Errors, e => e.Message.Contains("authorId is required"));
        }

        [Fact]
        public void Create_WithEmptyProfileId_ReturnsFailure()
        {
            // Act
            var result = Post.Create(_validAuthorId, Guid.Empty, _validMedia);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Contains(result.Errors, e => e.Message.Contains("profileId is required"));
        }

        [Fact]
        public void Create_WithNullMedia_InitializesEmptyMediaList()
        {
            // Act
            var result = Post.Create(_validAuthorId, _validProfileId, null);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value.Media);
            Assert.Empty(result.Value.Media);
        }

        [Fact]
        public void Create_WithNullPostAvailability_UsesDefault()
        {
            // Act
            var result = Post.Create(_validAuthorId, _validProfileId, _validMedia, null);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(PostAvailability.Default, result.Value.PostAvailability);
        }

        [Fact]
        public void AddLike_WithValidLike_AddsLikeToList()
        {
            // Arrange
            var post = Post.Create(_validAuthorId, _validProfileId, _validMedia).Value;
            var like = Like.Create(_validLikedById, DateTime.UtcNow.AddMinutes(-1)).Value;

            // Act
            post.AddLike(like);

            // Assert
            Assert.Single(post.Likes);
            Assert.Equal(like, post.Likes.First());
        }

        [Fact]
        public void AddLike_WithMediator_PublishesPostLikedEvent()
        {
            // Arrange
            var post = Post.Create(_validAuthorId, _validProfileId, _validMedia).Value;
            var like = Like.Create(_validLikedById, DateTime.UtcNow.AddMinutes(-1)).Value;
            var mediatorMock = new Mock<IMediator>();

            // Act
            post.AddLike(like, mediatorMock.Object);

            // Assert
            mediatorMock.Verify(m => m.Publish(
                It.Is<PostLiked>(e => e.Like == like && e.PostId == post.Id),
                default(CancellationToken)),
                Times.Once);
        }

        [Fact]
        public void RemoveLike_WithExistingLike_RemovesLikeFromList()
        {
            // Arrange
            var post = Post.Create(_validAuthorId, _validProfileId, _validMedia).Value;
            var like = Like.Create(_validLikedById, DateTime.UtcNow.AddMinutes(-1)).Value;
            post.AddLike(like);

            // Act
            post.RemoveLike(like);

            // Assert
            Assert.Empty(post.Likes);
        }

        [Fact]
        public void RemoveLike_WithNonExistentLike_DoesNothing()
        {
            // Arrange
            var post = Post.Create(_validAuthorId, _validProfileId, _validMedia).Value;
            var like1 = Like.Create(_validLikedById, DateTime.UtcNow.AddMinutes(-1)).Value;
            var like2 = Like.Create(Guid.NewGuid(), DateTime.UtcNow.AddMinutes(-1)).Value;
            post.AddLike(like1);

            // Act
            post.RemoveLike(like2);

            // Assert
            Assert.Single(post.Likes);
            Assert.Equal(like1, post.Likes.First());
        }

        [Fact]
        public void RemoveLike_WithMediator_PublishesPostUnlikedEvent()
        {
            // Arrange
            var post = Post.Create(_validAuthorId, _validProfileId, _validMedia).Value;
            var like = Like.Create(_validLikedById, DateTime.UtcNow.AddMinutes(-1)).Value;
            post.AddLike(like);
            var mediatorMock = new Mock<IMediator>();

            // Act
            post.RemoveLike(like, mediatorMock.Object);

            // Assert
            mediatorMock.Verify(m => m.Publish(
                It.Is<PostUnliked>(e => e.Like == like && e.PostId == post.Id),
                default(CancellationToken)),
                Times.Once);
        }

        [Fact]
        public void AddComment_WithValidComment_AddsCommentToList()
        {
            // Arrange
            var post = Post.Create(_validAuthorId, _validProfileId, _validMedia).Value;
            var comment = Comment.Create(post.Id, _validCommentText, _validCommentAuthorId).Value;

            // Act
            post.AddComment(comment);

            // Assert
            Assert.Single(post.Comments);
            Assert.Equal(comment, post.Comments.First());
        }

        [Fact]
        public void AddComment_WithMediator_PublishesPostCommentedEvent()
        {
            // Arrange
            var post = Post.Create(_validAuthorId, _validProfileId, _validMedia).Value;
            var comment = Comment.Create(post.Id, _validCommentText, _validCommentAuthorId).Value;
            var mediatorMock = new Mock<IMediator>();

            // Act
            post.AddComment(comment, mediatorMock.Object);

            // Assert
            mediatorMock.Verify(m => m.Publish(
                It.Is<PostCommented>(e => e.Comment == comment),
                default(CancellationToken)),
                Times.Once);
        }

        [Fact]
        public void RemoveComment_WithExistingComment_MarksCommentAsRemoved()
        {
            // Arrange
            var post = Post.Create(_validAuthorId, _validProfileId, _validMedia).Value;
            var comment = Comment.Create(post.Id, _validCommentText, _validCommentAuthorId).Value;
            post.AddComment(comment);

            // Act
            post.RemoveComment(comment.Id);

            // Assert
            Assert.True(comment.IsRemoved);
        }

        [Fact]
        public void RemoveComment_WithNonExistentComment_DoesNothing()
        {
            // Arrange
            var post = Post.Create(_validAuthorId, _validProfileId, _validMedia).Value;
            var comment = Comment.Create(post.Id, _validCommentText, _validCommentAuthorId).Value;
            post.AddComment(comment);

            // Act
            post.RemoveComment(Guid.NewGuid());

            // Assert
            Assert.False(comment.IsRemoved);
        }

        [Fact]
        public void RemoveComment_WithMediator_PublishesNoEvent()
        {
            // Arrange
            var post = Post.Create(_validAuthorId, _validProfileId, _validMedia).Value;
            var comment = Comment.Create(post.Id, _validCommentText, _validCommentAuthorId).Value;
            post.AddComment(comment);
            var mediatorMock = new Mock<IMediator>();

            // Act
            post.RemoveComment(comment.Id, mediatorMock.Object);

            // Assert
            mediatorMock.Verify(m => m.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public void AddLike_WithFutureTimestamp_FailsCreation()
        {
            // Arrange
            var post = Post.Create(_validAuthorId, _validProfileId, _validMedia).Value;
            var likeResult = Like.Create(_validLikedById, DateTime.UtcNow.AddMinutes(1));

            // Assert
            Assert.True(likeResult.IsFailed);
            Assert.Contains(likeResult.Errors, e => e is FutureTimeError);
        }

        [Fact]
        public void AddComment_WithEmptyText_FailsCreation()
        {
            // Arrange
            var post = Post.Create(_validAuthorId, _validProfileId, _validMedia).Value;
            var commentResult = Comment.Create(post.Id, "", _validCommentAuthorId);

            // Assert
            Assert.True(commentResult.IsFailed);
            Assert.Contains(commentResult.Errors, e => e.Message.Contains("text is required"));
        }

        [Fact]
        public void Media_Creation_WithInvalidParameters_Fails()
        {
            // Act
            var result1 = Media.Create("", _validMediaUrl);
            var result2 = Media.Create(_validMimeType, "");

            // Assert
            Assert.True(result1.IsFailed);
            Assert.Contains(result1.Errors, e => e.Message.Contains("mime is required"));

            Assert.True(result2.IsFailed);
            Assert.Contains(result2.Errors, e => e.Message.Contains("url is required"));
        }
    }
}