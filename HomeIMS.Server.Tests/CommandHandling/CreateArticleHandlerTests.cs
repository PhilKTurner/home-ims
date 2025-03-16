using HomeIMS.SharedContracts.Domain.Articles;
using HomeIMS.SharedContracts.Domain.Articles.Commands;
using HomeIMS.SharedContracts.Domain.Articles.Events;

namespace HomeIMS.Server.Tests.CommandHandling;

public class CreateArticleHandlerTests
{
    private CreateArticleHandler subject;

    [SetUp]
    public void Setup()
    {
        subject = new CreateArticleHandler();
    }

    [Test]
    public void MissingCommandCausesException()
    {
        var testState = default(Article);
        var testCommand = default(CreateArticle);

        Assert.Throws<ArgumentNullException>(() =>
        {
            subject.Decide(testCommand!, testState);
        });
    }

    [Test]
    public void BasicCreation()
    {
        // arrange
        var testState = default(Article);
        var testCommand = new CreateArticle
        {
            ArticleGroupId = null,
            Name = null,
            Description = null
        };

        // act
        var result = subject.Decide(testCommand, testState);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value.First().Should().BeOfType<ArticleCreated>();
        var resultEvent = (ArticleCreated)result.Value.First();
        resultEvent.ArticleGroupId.Should().BeNull();
        resultEvent.Name.Should().BeNull();
        resultEvent.Description.Should().BeNull();
    }

    [Test]
    public void BasicCreationWithData()
    {
        // arrange
        var testState = default(Article);
        var testCommand = new CreateArticle
        {
            ArticleGroupId = Guid.NewGuid(),
            Name = "Test Article",
            Description = "Test Description"
        };

        // act
        var result = subject.Decide(testCommand, testState);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value.First().Should().BeOfType<ArticleCreated>();
        var resultEvent = (ArticleCreated)result.Value.First();
        resultEvent.ArticleGroupId.Should().Be(testCommand.ArticleGroupId);
        resultEvent.Name.Should().Be(testCommand.Name);
        resultEvent.Description.Should().Be(testCommand.Description);
    }
}