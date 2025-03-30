using Application.Repositories;
using Domain.Post;
using Domain.Profile;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class PostRepository : IPostRepository
{
    private readonly DdssContext _context;

    public PostRepository(DdssContext context)
    {
        _context = context;
    }
    public Task<Post> GetById(Guid id)
    {
        return _context.Posts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    }

    public Task<List<Post>> GetAll()
    {
        return _context.Posts.AsNoTracking().ToListAsync();
    }

    public async Task<Post> Create(Post entity)
    {
        _context.Posts.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public Task Delete(Post entity)
    {
        _context.Posts.Remove(entity);
        return _context.SaveChangesAsync();
    }

    public async Task<Post> Update(Post entity)
    {
        _context.Posts.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
}

public class ProfileRepository : IProfileRepository
{
    private readonly DdssContext _context;
    public ProfileRepository(DdssContext context)
    {
        _context = context;
    }
    public Task<Profile> GetById(Guid id)
    {
        return _context.Profiles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    }

    public Task<List<Profile>> GetAll()
    {
        return _context.Profiles.AsNoTracking().ToListAsync();
    }

    public async Task<Profile> Update(Profile entity)
    {
        _context.Profiles.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public Task Delete(Profile entity)
    {
        _context.Profiles.Remove(entity);
        return _context.SaveChangesAsync();
    }

    public async Task<Profile> Create(Profile entity)
    {
        _context.Profiles.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public Task<Profile> GetByLogin(string login)
    {
        return _context.Profiles.AsNoTracking().FirstOrDefaultAsync(x => x.Login == login);
    }
}