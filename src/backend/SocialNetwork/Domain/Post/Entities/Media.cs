using Domain.Post.ValueObjects;
using FluentResults;

namespace Domain.Post;

public class Media
{
    public Guid Id { get; set; }
    public string Mime { get; set; }
    public string Url { get; set; }

    private Media(
        string mime,
        string url)
    {
        Id = Guid.NewGuid();

        Mime = mime;
        Url = url;
    }

    public static Result<Media> Create(
        string mime,
        string url)
    {
        if (string.IsNullOrWhiteSpace(mime))
        {
            return Result.Fail(new RequiredFieldNotSet(nameof(mime)));
        }

        if (string.IsNullOrWhiteSpace(url))
        {
            return Result.Fail(new RequiredFieldNotSet(nameof(url)));
        }

        return new Media(mime, url);
    }
}