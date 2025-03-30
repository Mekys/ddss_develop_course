using Domain.Post.ValueObjects;
using FluentResults;

namespace Domain.Profile.ValueObjects;

public record Name
{
    public string First { get; set; }
    public string Second { get; set; }
    public string Patronymic { get; set; }

    private Name(string first, string second, string patronymic)
    {
        First = first;
        Second = second;
        Patronymic = patronymic;
    }

    public static Result<Name> Create(string first, string second, string patronymic)
    {
        first = first?.Trim();
        second = second?.Trim();
        patronymic = patronymic?.Trim();

        if (string.IsNullOrEmpty(first))
        {
            return Result.Fail(new RequiredFieldNotSet("FirstName"));
        }

        if (string.IsNullOrEmpty(second))
        {
            return Result.Fail(new RequiredFieldNotSet("SecondName"));
        }

        return new Name(first, second, patronymic);
    }
}