using HomeIMS.SharedContracts.Domain.Inventories;
using HomeIMS.SharedContracts.Domain.Inventories.Commands;
using HomeIMS.SharedContracts.Domain.Inventories.Events;

namespace HomeIMS.Server.Tests.CommandHandling;

public class CreateInventoryTests
{
    private CreateInventoryHandler subject;

    [SetUp]
    public void Setup()
    {
        subject = new CreateInventoryHandler();
    }

    [Test]
    public void MissingCommandCausesException()
    {
        var testState = default(Inventory);
        var testCommand = default(CreateInventory);

        Assert.Throws<ArgumentNullException>(() =>
        {
            subject.Decide(testCommand!, testState);
        });
    }

    [Test]
    public void BasicCreation()
    {
        // arrange
        var testState = default(Inventory);
        var testCommand = new CreateInventory
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
        result.Value.First().Should().BeOfType<InventoryCreated>();
        var resultEvent = (InventoryCreated)result.Value.First();
        resultEvent.ArticleGroupId.Should().BeNull();
        resultEvent.Name.Should().BeNull();
        resultEvent.Description.Should().BeNull();
    }

    [Test]
    public void BasicCreationWithData()
    {
        // arrange
        var testState = default(Inventory);
        var testCommand = new CreateInventory
        {
            ArticleGroupId = Guid.NewGuid(),
            Name = "Test Inventory",
            Description = "Test Description"
        };

        // act
        var result = subject.Decide(testCommand, testState);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value.First().Should().BeOfType<InventoryCreated>();
        var resultEvent = (InventoryCreated)result.Value.First();
        resultEvent.ArticleGroupId.Should().Be(testCommand.ArticleGroupId);
        resultEvent.Name.Should().Be(testCommand.Name);
        resultEvent.Description.Should().Be(testCommand.Description);
    }
}