using FluentResults;

namespace Domain.Post.ValueObjects;

public class RequiredFieldNotSet : Error
{
    public RequiredFieldNotSet(string fieldName) : base(fieldName + " is required")
    { }
}