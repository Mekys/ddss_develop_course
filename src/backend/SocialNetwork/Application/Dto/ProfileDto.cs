namespace Application.Dto.Profile;

public class ProfileDto
{
    public Guid Id { get; set; }
    public string Login { get; set; }
    public DateTime LastLoginAtUtc { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Patronymic { get; set; }
}