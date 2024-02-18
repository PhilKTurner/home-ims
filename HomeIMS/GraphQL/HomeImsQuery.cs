using HomeIMS.Contracts;
using HomeIMS.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace HomeIMS.GraphQL;

public class HomeImsQuery
{
    public async Task<IEnumerable<Article>> Articles([Service] IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var himsContext = scope.ServiceProvider.GetService<HomeImsContext>() ?? throw new NullReferenceException();

            return await himsContext.Articles.ToListAsync();
        }
    }
}
