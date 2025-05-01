using HomeIMS.SharedContracts.Domain.Inventories;
using HomeIMS.SharedContracts.Domain.Inventories.Commands;
using HomeIMS.SharedContracts.Domain.Inventories.Events;
using HomeIMS.SharedContracts.EventSourcing;


namespace HomeIMS.Server.Tests.CommandHandling;

public class UpdateInventoryHandlerTests
{
    private UpdateInventoryHandler subject;

    [SetUp]
    public void Setup()
    {
        var eventStoreMock = new Mock<IEventStore>();
        subject = new UpdateInventoryHandler(eventStoreMock.Object);
    }

    [Test]
    public void MissingCommandCausesException()
    {
        var testState = default(Inventory);
        var testCommand = default(UpdateInventory);

        Assert.Throws<ArgumentNullException>(() =>
        {
            subject.Decide(testCommand!, testState);
        });
    }

    [Test]
    public void NoChangesNoEvent()
    {
        // arrange
        var testState = new Inventory()
        {
            Id = Guid.NewGuid(),
            ArticleGroupId = Guid.NewGuid(),
            Name = "Test Inventory",
            Description = "Test Description"
        };

        var testCommand = new UpdateInventory(testState); // no changes

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

        var testState = new Inventory()
        {
            Id = Guid.NewGuid(),
            ArticleGroupId = Guid.NewGuid(),
            Name = "TestName",
            Description = "TestDescription"
        };

        var testCommand = new UpdateInventory(testState);
        testCommand.ArticleGroupId = expectedArticleGroupId ?? testState.ArticleGroupId;
        testCommand.Name = expectedName ?? testState.Name;
        testCommand.Description = expectedDescription ?? testState.Description;

        // act
        var result = subject.Decide(testCommand, testState);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value.First().Should().BeOfType<InventoryModified>();
        var resultEvent = (InventoryModified)result.Value.First();
        resultEvent.ArticleGroupId.Should().Be(expectedArticleGroupId);
        resultEvent.Name.Should().Be(expectedName);
        resultEvent.Description.Should().Be(expectedDescription);
    }
}