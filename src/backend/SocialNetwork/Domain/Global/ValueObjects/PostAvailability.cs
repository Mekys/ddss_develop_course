namespace Domain.Post.ValueObjects;

public record PostAvailability
{
    private PostAvailability() { }
    public Availability Comment { get; set; }
    public Availability Like { get; set; }
    public Availability Post { get; set; }

    public static PostAvailability Default => new PostAvailability()
    {
        Comment = Availability.Public,
        Like = Availability.Public,
        Post = Availability.Public
    };
}