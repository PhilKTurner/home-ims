using HomeIMS.SharedContracts.Domain.ArticleGroups;
using HomeIMS.SharedContracts.Domain.ArticleGroups.Commands;
using HomeIMS.SharedContracts.Domain.ArticleGroups.Events;

namespace HomeIMS.Server.Tests.CommandHandling;

public class CreateArticleGroupHandlerTests
{
    private CreateArticleGroupHandler subject;

    [SetUp]
    public void Setup()
    {
        subject = new CreateArticleGroupHandler();
    }

    [Test]
    public void MissingCommandCausesException()
    {
        var testState = default(ArticleGroup);
        var testCommand = default(CreateArticleGroup);

        Assert.Throws<ArgumentNullException>(() =>
        {
            subject.Decide(testCommand!, testState);
        });
    }

    [Test]
    public void BasicCreation()
    {
        // arrange
        var testState = default(ArticleGroup);
        var testCommand = new CreateArticleGroup
        {
            Name = null,
            Description = null
        };

        // act
        var result = subject.Decide(testCommand, testState);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value.First().Should().BeOfType<ArticleGroupCreated>();
        var resultEvent = (ArticleGroupCreated)result.Value.First();
        resultEvent.Name.Should().BeNull();
        resultEvent.Description.Should().BeNull();
    }

    [Test]
    public void BasicCreationWithData()
    {
        // arrange
        var testState = default(ArticleGroup);
        var testCommand = new CreateArticleGroup
        {
            Name = "Test Article",
            Description = "Test Description"
        };

        // act
        var result = subject.Decide(testCommand, testState);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value.First().Should().BeOfType<ArticleGroupCreated>();
        var resultEvent = (ArticleGroupCreated)result.Value.First();
        resultEvent.Name.Should().Be(testCommand.Name);
        resultEvent.Description.Should().Be(testCommand.Description);
    }
}