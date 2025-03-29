using Domain.Profile;

namespace Application.Repositories;

public interface IProfileRepository : IBaseRepository<Profile>
{
    Task<Profile> GetByLogin(string login);
}