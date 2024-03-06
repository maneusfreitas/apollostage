using ApolloStage.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApolloStage.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ApplicationUser> TempRegisterData { get; set; }

    public DbSet<FavoriteAlbum> FavoriteAlbum { get; set; }

    public DbSet<AlbumReview> AlbumReview { get; set; }

    public DbSet<ReviewReports> ReviewReports { get; set; }

    public DbSet<Models.Extra.Classification> AlbumRatings { get; set; }

    public DbSet<Models.Product.Product> Product { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Adicione o log para o Console para depuração
        optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
        base.OnConfiguring(optionsBuilder);
    }
}

