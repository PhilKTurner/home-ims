using HomeIMS.SharedContracts.Domain.Articles;
using HomeIMS.SharedContracts.Domain.Articles.Commands;
using HomeIMS.SharedContracts.Domain.Articles.Events;
using HomeIMS.SharedContracts.EventSourcing;


namespace HomeIMS.Server.Tests.CommandHandling;

public class UpdateArticleHandlerTests
{
    private UpdateArticleHandler subject;

    [SetUp]
    public void Setup()
    {
        var eventStoreMock = new Mock<IEventStore>();
        subject = new UpdateArticleHandler(eventStoreMock.Object);
    }

    [Test]
    public void MissingCommandCausesException()
    {
        var testState = default(Article);
        var testCommand = default(UpdateArticle);

        Assert.Throws<ArgumentNullException>(() =>
        {
            subject.Decide(testCommand!, testState);
        });
    }

    [Test]
    public void NoChangesNoEvent()
    {
        // arrange
        var testState = new Article()
        {
            Id = Guid.NewGuid(),
            ArticleGroupId = Guid.NewGuid(),
            Name = "Test Article",
            Description = "Test Description"
        };

        var testCommand = new UpdateArticle(testState); // no changes

        // act
        var result = subject.Decide(testCommand, testState);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(0);
    }

    [TestCase("e3a1c9f4-8b3e-4d9d-8f3b-1b2e4d9d8f3b", null, null)]
    [TestCase(null, "NewTestName", null)]
    [TestCase(null, null, "NewTestDescription")]
    public void EventOnlyContainsChangedData(string? expectedArticleGroupIdString, string? expectedName, string? expectedDescription)
    {
        // arrange
        Guid? expectedArticleGroupId = expectedArticleGroupIdString is null ? null : Guid.Parse(expectedArticleGroupIdString);

        var testState = new Article()
        {
            Id = Guid.NewGuid(),
            ArticleGroupId = Guid.NewGuid(),
            Name = "TestName",
            Description = "TestDescription"
        };

        var testCommand = new UpdateArticle(testState);
        testCommand.ArticleGroupId = expectedArticleGroupId ?? testState.ArticleGroupId;
        testCommand.Name = expectedName ?? testState.Name;
        testCommand.Description = expectedDescription ?? testState.Description;

        // act
        var result = subject.Decide(testCommand, testState);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value.First().Should().BeOfType<ArticleModified>();
        var resultEvent = (ArticleModified)result.Value.First();
        resultEvent.ArticleGroupId.Should().Be(expectedArticleGroupId);
        resultEvent.Name.Should().Be(expectedName);
        resultEvent.Description.Should().Be(expectedDescription);
    }
}