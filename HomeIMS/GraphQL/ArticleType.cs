using GraphQL.Types;
using HomeIMS.Contracts;

namespace HomeIMS.GraphQL;

public class ArticleType : ObjectGraphType<Article>
{
    public ArticleType()
    {
        Field(x => x.Id);
        Field(nameof(Article.EAN).ToLower(), x => x.EAN);
        Field(x => x.Description);
    }
}