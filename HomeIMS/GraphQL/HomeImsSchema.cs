using GraphQL.Types;

namespace HomeIMS.GraphQL;

public class HomeImsSchema : Schema
{
    public HomeImsSchema(HomeImsQuery query, HomeImsMutation mutation)
    {
        Query = query;
        Mutation = mutation;
    }
}