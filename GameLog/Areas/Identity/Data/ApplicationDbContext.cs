using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GameLog.Models;


namespace GameLog.Areas.Identity.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public DbSet<Game> Games => Set<Game>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<UserGameList> UserGameLists => Set<UserGameList>();
    public DbSet<Follow> Follows => Set<Follow>();

    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<GameGenre> GameGenres => Set<GameGenre>();

    public DbSet<Platform> Platforms => Set<Platform>();
    public DbSet<GamePlatform> GamePlatforms => Set<GamePlatform>();

    public DbSet<ReviewComment> ReviewComments => Set<ReviewComment>();
    public DbSet<Report> Reports => Set<Report>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {

        base.OnModelCreating(builder);

        builder.Entity<Review>()
        .HasIndex(r => new { r.UserId, r.GameId })
        .IsUnique();

        builder.Entity<UserGameList>()
            .HasIndex(ugl => new { ugl.UserId, ugl.GameId })
            .IsUnique();

        builder.Entity<Follow>()
            .HasKey(f => new { f.FollowerId, f.FollowingId });

        builder.Entity<GameGenre>()
            .HasKey(gg => new { gg.GameId, gg.GenreId });

        builder.Entity<GamePlatform>()
            .HasKey(gp => new { gp.GameId, gp.PlatformId });
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
