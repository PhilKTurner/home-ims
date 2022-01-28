using Microsoft.EntityFrameworkCore;

namespace HomeIMS.DataAccess;

public class HomeImsContext : DbContext
{
    public HomeImsContext(DbContextOptions<HomeImsContext> options) : base(options)
    {
    }
}
