using HomeIMS.Contracts;
using Microsoft.EntityFrameworkCore;

namespace HomeIMS.DataAccess;

public class HomeImsContext : DbContext
{
    public HomeImsContext(DbContextOptions<HomeImsContext> options) : base(options)
    {
    }

    public DbSet<Article> Articles { get; set; } = null!;
}
