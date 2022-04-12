using GraphQL.Types;
using HomeIMS.Contracts;
using HomeIMS.DataAccess;

namespace HomeIMS.GraphQL;

public class HomeImsQuery : ObjectGraphType
{
    private readonly IServiceProvider serviceProvider;

    public HomeImsQuery(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;

        Field<ListGraphType<ArticleType>>("articles", resolve: context => Articles());
    }

    private List<Article> Articles()
    {
        var himsContext = serviceProvider.GetService<HomeImsContext>() ?? throw new NullReferenceException();

        return himsContext.Articles.ToList();
    }
}
