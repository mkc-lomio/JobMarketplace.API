using FluentAssertions;
using JobMarketplace.Application.Common.Models;

namespace JobMarketplace.Tests.Unit.Models;

public class PagedResultTests
{
    private record TestItem(long Id, string Name);

    [Fact]
    public void Create_WithMoreItemsThanPageSize_ShouldSetHasMoreTrue()
    {
        // Arrange — SP returned 21 items (pageSize + 1)
        var items = Enumerable.Range(1, 21)
            .Select(i => new TestItem(i, $"Item {i}"))
            .ToList();

        // Act
        var result = PagedResult<TestItem>.Create(items, pageSize: 20, t => t.Id);

        // Assert
        result.HasMore.Should().BeTrue();
        result.Data.Should().HaveCount(20);          // Extra row trimmed
        result.NextCursor.Should().Be(20);            // Last item's Id
        result.PageSize.Should().Be(20);
    }

    [Fact]
    public void Create_WithExactPageSize_ShouldSetHasMoreFalse()
    {
        // Arrange — SP returned exactly 20 items (no extra row = last page)
        var items = Enumerable.Range(1, 20)
            .Select(i => new TestItem(i, $"Item {i}"))
            .ToList();

        // Act
        var result = PagedResult<TestItem>.Create(items, pageSize: 20, t => t.Id);

        // Assert
        result.HasMore.Should().BeFalse();
        result.Data.Should().HaveCount(20);
        result.NextCursor.Should().BeNull();
    }

    [Fact]
    public void Create_WithFewerItemsThanPageSize_ShouldSetHasMoreFalse()
    {
        // Arrange — SP returned only 5 items
        var items = Enumerable.Range(1, 5)
            .Select(i => new TestItem(i, $"Item {i}"))
            .ToList();

        // Act
        var result = PagedResult<TestItem>.Create(items, pageSize: 20, t => t.Id);

        // Assert
        result.HasMore.Should().BeFalse();
        result.Data.Should().HaveCount(5);
        result.NextCursor.Should().BeNull();
    }

    [Fact]
    public void Create_WithEmptyList_ShouldReturnEmptyResult()
    {
        // Act
        var result = PagedResult<TestItem>.Create([], pageSize: 20, t => t.Id);

        // Assert
        result.HasMore.Should().BeFalse();
        result.Data.Should().BeEmpty();
        result.NextCursor.Should().BeNull();
    }

    [Fact]
    public void Create_ShouldNeverReturnExtraRow()
    {
        // Arrange — 101 items, page size 100
        var items = Enumerable.Range(1, 101)
            .Select(i => new TestItem(i, $"Item {i}"))
            .ToList();

        // Act
        var result = PagedResult<TestItem>.Create(items, pageSize: 100, t => t.Id);

        // Assert — client should never see more than pageSize items
        result.Data.Should().HaveCount(100);
        result.HasMore.Should().BeTrue();
        result.NextCursor.Should().Be(100);
    }
}
