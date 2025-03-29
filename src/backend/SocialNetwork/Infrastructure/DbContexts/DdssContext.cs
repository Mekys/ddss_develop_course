using Domain.Post;
using Domain.Post.ValueObjects;
using Domain.Profile;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Infrastructure.DbContexts;

public class DdssContext : DbContext
{
    public DdssContext(DbContextOptions<DdssContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<Post> Posts { get; set; } = null!;
    public DbSet<Profile> Profiles { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new PostConfiguration());
        modelBuilder.ApplyConfiguration(new ProfileConfiguration());
    }
}

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        // Primary key
        builder.HasKey(p => p.Id);

        // Properties
        builder.Property(p => p.AuthorId)
            .IsRequired();

        builder.Property(p => p.ProfileId)
            .IsRequired();

        builder.Property(p => p.CreatedAtUtc)
            .IsRequired();

        builder.Property(p => p.IsRemoved)
            .IsRequired()
            .HasDefaultValue(false);

        // Value object conversion
        builder.OwnsOne(p => p.PostAvailability, post =>
        {
            post
                .Property(p => p.Post)
                .HasColumnName("PostAvailability_Post");

            post
                .Property(p => p.Comment)
                .HasColumnName("PostAvailability_Comment");

            post
                .Property(p => p.Like)
                .HasColumnName("PostAvailability_Like");
        });

        builder.OwnsMany(p => p.Media, media =>
        {
            media.WithOwner().HasForeignKey("PostId");
            media.ToTable("PostMedia");
            media.HasKey(m => m.Id);
        });

        builder.HasMany(p => p.Comments)
            .WithOne()
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Property(e => e.Likes)
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<List<Like>>(v) ?? new List<Like>());
    }
}

public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        // Primary key
        builder.HasKey(p => p.Id);

        // Properties
        builder.Property(p => p.Login)
            .IsRequired()
            .HasMaxLength(100); // Adjust length as needed

        builder.Property(p => p.CreatedAtUtc)
            .IsRequired();

        builder.Property(p => p.LastLoginAtUtc)
            .IsRequired();

        // Value object conversion for Name
        builder.OwnsOne(p => p.Names, names =>
        {
            names.Property(n => n.First)
                .IsRequired()
                .HasMaxLength(100);

            names.Property(n => n.Second)
                .IsRequired()
                .HasMaxLength(100);

            names.Property(n => n.Patronymic)
                .HasMaxLength(100);
        });

        // Enum conversion for Availability
        builder.Property(p => p.AvailabilityLevel)
            .HasConversion(
                v => v.ToString(),
                v => (Availability)Enum.Parse(typeof(Availability), v))
            .IsRequired();

        // Collections of Guids (Subscribers and Followers)
        builder.Property(p => p.Subscribers)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .Select(Guid.Parse)
                      .ToList())
            .Metadata.SetValueComparer(
                new ValueComparer<IReadOnlyCollection<Guid>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));

        builder.Property(p => p.Followers)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .Select(Guid.Parse)
                      .ToList())
            .Metadata.SetValueComparer(
                new ValueComparer<IReadOnlyCollection<Guid>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));

        builder.ToTable("Profiles");
    }
}