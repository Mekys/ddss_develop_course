using Application.Dto.Profile;
using Application.Repositories;
using Application.Services.Interfaces;
using Domain.Post;
using Domain.Post.Entities;
using Domain.Post.ValueObjects;
using FluentResults;
using Mapster;

namespace Application.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;
    public PostService(IPostRepository postRepository) => _postRepository = postRepository;

    public async Task<Result<Post>> Create(CreatePostDto postDto)
    {
        var newPostResult = Post.Create(
            Guid.NewGuid(),
            postDto.ProfileId,
            postDto.Media.Adapt<List<Media>>(),
            postDto.PostAvailability.Adapt<PostAvailability>());

        if (newPostResult.IsFailed)
        {
            return Result.Fail<Post>(newPostResult.Errors);
        }

        return await _postRepository.Create(newPostResult.Value);
    }

    public async Task<Result> AddLike(Guid postId)
    {
        var post = await _postRepository.GetById(postId);
        if (post is null)
        {
            return Result.Fail("Not found");
        }

        var like = Like.Create(Guid.NewGuid(), DateTime.UtcNow);
        if (like.IsFailed)
        {
            return Result.Fail(like.Errors);
        }

        post.AddLike(like.Value);
        await _postRepository.Update(post);

        return Result.Ok();
    }

    public async Task<Result> UnLike(Guid postId)
    {
        var post = await _postRepository.GetById(postId);
        if (post is null)
        {
            return Result.Fail("Not found");
        }

        var like = Like.Create(Guid.NewGuid(), DateTime.UtcNow);
        if (like.IsFailed)
        {
            return Result.Fail(like.Errors);
        }

        post.AddLike(like.Value);
        await _postRepository.Update(post);

        return Result.Ok();
    }

    public async Task<Result> Delete(Guid postId)
    {
        var post = await _postRepository.GetById(postId);
        if (post is null)
        {
            return Result.Fail("Not found");
        }
        await _postRepository.Delete(post);

        return Result.Ok();
    }

    public async Task<Result> AddComment(Guid postId, CreateCommentDto comment)
    {
        var post = await _postRepository.GetById(postId);
        if (post is null)
        {
            return Result.Fail("Not found");
        }

        var commentResult = Comment.Create(comment.PostId, comment.Text, comment.AuthorId);
        if (commentResult.IsFailed)
        {
            return Result.Fail(commentResult.Errors);
        }

        post.AddComment(commentResult.Value);
        await _postRepository.Update(post);

        return Result.Ok();
    }

    public async Task<Result> DeleteComment(Guid postId, Guid commentId)
    {
        var post = await _postRepository.GetById(postId);
        if (post is null)
        {
            return Result.Fail("Not found");
        }
        post.RemoveComment(commentId);

        await _postRepository.Update(post);

        return Result.Ok();
    }

    public async Task<Result<Post>> GetById(Guid id)
    {
        var post = await _postRepository.GetById(id);
        if (post is null)
        {
            return Result.Fail("Not found");
        }

        return Result.Ok(post);
    }
}