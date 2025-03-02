namespace Application.Dto.Profile;

public class ProfileDto
{
    public Guid Id { get; set; }
    public string Login { get; set; }
    public DateTime LastLoginAtUtc { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Patronymic { get; set; }

    public static ProfileDto GetFromProfile(Domain.Profile.Profile profile)
    {
        return new ProfileDto()
        {
            Id = profile.Id,
            Login = profile.Login,
            LastLoginAtUtc = profile.LastLoginAtUtc,
            FirstName = profile.Names.First,
            LastName = profile.Names.Second,
            Patronymic = profile.Names.Patronymic
        };
    }
}


