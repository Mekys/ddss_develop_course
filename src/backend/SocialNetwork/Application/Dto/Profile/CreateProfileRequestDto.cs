namespace Application.Dto.Profile;

public class CreateProfileRequestDto
{
    public string Login { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Patronymic { get; set; }
}