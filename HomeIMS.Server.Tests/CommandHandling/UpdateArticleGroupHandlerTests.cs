using HomeIMS.SharedContracts.Domain.ArticleGroups;
using HomeIMS.SharedContracts.Domain.ArticleGroups.Commands;
using HomeIMS.SharedContracts.Domain.ArticleGroups.Events;
using HomeIMS.SharedContracts.EventSourcing;


namespace HomeIMS.Server.Tests.CommandHandling;

public class UpdateArticleGroupHandlerTests
{
    private UpdateArticleGroupHandler subject;

    [SetUp]
    public void Setup()
    {
        var eventStoreMock = new Mock<IEventStore>();
        subject = new UpdateArticleGroupHandler(eventStoreMock.Object);
    }

    [Test]
    public void MissingCommandCausesException()
    {
        var testState = default(ArticleGroup);
        var testCommand = default(UpdateArticleGroup);

        Assert.Throws<ArgumentNullException>(() =>
        {
            subject.Decide(testCommand!, testState);
        });
    }

    [Test]
    public void NoChangesNoEvent()
    {
        // arrange
        var testState = new ArticleGroup()
        {
            Id = Guid.NewGuid(),
            Name = "Test Article",
            Description = "Test Description"
        };

        var testCommand = new UpdateArticleGroup(testState); // no changes

        // act
        var result = subject.Decide(testCommand, testState);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(0);
    }

    [TestCase("NewTestName", null)]
    [TestCase(null, "NewTestDescription")]
    public void EventOnlyContainsChangedData(string? expectedName, string? expectedDescription)
    {
        var testState = new ArticleGroup()
        {
            Id = Guid.NewGuid(),
            Name = "TestName",
            Description = "TestDescription"
        };

        var testCommand = new UpdateArticleGroup(testState);
        testCommand.Name = expectedName ?? testState.Name;
        testCommand.Description = expectedDescription ?? testState.Description;

        // act
        var result = subject.Decide(testCommand, testState);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value.First().Should().BeOfType<ArticleGroupModified>();
        var resultEvent = (ArticleGroupModified)result.Value.First();
        resultEvent.Name.Should().Be(expectedName);
        resultEvent.Description.Should().Be(expectedDescription);
    }
}