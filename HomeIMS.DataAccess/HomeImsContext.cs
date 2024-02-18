using HomeIMS.Contracts;
using Microsoft.EntityFrameworkCore;

namespace HomeIMS.DataAccess;

public class HomeImsContext : DbContext
{
    public HomeImsContext(DbContextOptions<HomeImsContext> options) : base(options)
    {
    }

    public DbSet<Article> Articles { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Article>()
                    .HasData(
                        new Article
                        {
                            Id = 1,
                            EAN = "978-3-16-148410-0",
                            Description = "The Hitchhiker's Guide to the Galaxy"
                        },
                        new Article
                        {
                            Id = 2,
                            EAN = "978-3-16-148410-1",
                            Description = "The Restaurant at the End of the Universe"
                        },
                        new Article
                        {
                            Id = 3,
                            EAN = "978-3-16-148410-2",
                            Description = "Life, the Universe and Everything"
                        }
                    );
    }
}
