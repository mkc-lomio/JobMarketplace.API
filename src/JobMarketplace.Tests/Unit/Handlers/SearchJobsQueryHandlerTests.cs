using FluentAssertions;
using JobMarketplace.Application.Common.DTOs;
using JobMarketplace.Application.Common.Interfaces;
using JobMarketplace.Application.Features.Jobs.Queries.SearchJobs;
using Moq;

namespace JobMarketplace.Tests.Unit.Handlers;

public class SearchJobsQueryHandlerTests
{
    private readonly Mock<IDapperQueryService> _queryServiceMock;
    private readonly SearchJobsQueryHandler _handler;

    public SearchJobsQueryHandlerTests()
    {
        _queryServiceMock = new Mock<IDapperQueryService>();
        _handler = new SearchJobsQueryHandler(_queryServiceMock.Object);
    }

    private static List<JobSearchDto> FakeSearchResults(int count) =>
        Enumerable.Range(1, count).Select(i => new JobSearchDto
        {
            Id = i,
            PublicGuid = Guid.NewGuid(),
            Title = $"Developer {i}",
            Location = "Remote",
            IsRemote = true,
            JobType = "FullTime",
            ExperienceLevel = "Mid",
            CompanyPublicGuid = Guid.NewGuid(),
            CompanyName = $"Company {i}",
            CompanyIndustry = "Tech",
            ApplicationCount = i * 10,
            CreatedAt = DateTime.UtcNow
        }).ToList();

    [Fact]
    public async Task Handle_ShouldCallSearchStoredProcedure()
    {
        // Arrange
        _queryServiceMock
            .Setup(q => q.QueryAsync<JobSearchDto>("sp_SearchJobs", It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeSearchResults(5));

        var query = new SearchJobsQuery { SearchTerm = "developer", PageSize = 20 };

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _queryServiceMock.Verify(
            q => q.QueryAsync<JobSearchDto>("sp_SearchJobs", It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithResults_ShouldReturnPagedResult()
    {
        // Arrange
        _queryServiceMock
            .Setup(q => q.QueryAsync<JobSearchDto>("sp_SearchJobs", It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeSearchResults(10));

        var query = new SearchJobsQuery { SearchTerm = "developer", PageSize = 20 };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Data.Should().HaveCount(10);
        result.HasMore.Should().BeFalse();
        result.Data.Should().AllSatisfy(j => j.ApplicationCount.Should().BeGreaterThan(0));
    }

    [Fact]
    public async Task Handle_NoSearchTerm_ShouldStillWork()
    {
        // Arrange — no search term, just filters
        _queryServiceMock
            .Setup(q => q.QueryAsync<JobSearchDto>("sp_SearchJobs", It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeSearchResults(3));

        var query = new SearchJobsQuery { Location = "Remote", PageSize = 20 };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Data.Should().HaveCount(3);
    }
}
