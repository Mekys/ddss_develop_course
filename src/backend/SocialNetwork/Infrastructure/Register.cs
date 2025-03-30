using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class Register
{
    public static void RegisterDb(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContext<DdssContext>(x => x.UseNpgsql(@"Host=postgres:5432;Database=postgres_db;Username=postgres_user;Password=postgres_password"));
    }
}