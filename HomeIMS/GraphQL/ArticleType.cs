using HomeIMS.Contracts;

namespace HomeIMS.GraphQL;

public class ArticleType : ObjectType<Article>
{
    protected override void Configure(IObjectTypeDescriptor<Article> descriptor)
    {
        descriptor.Name("BookAuthor");

        descriptor
            .Field(f => f.EAN)
            .Name(nameof(Article.EAN).ToLower());
    }
}