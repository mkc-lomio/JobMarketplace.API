using FluentAssertions;
using JobMarketplace.Application.Common.DTOs;
using JobMarketplace.Application.Common.Interfaces;
using JobMarketplace.Application.Features.Jobs.Queries.GetAllJobs;
using Moq;

namespace JobMarketplace.Tests.Unit.Handlers;

public class GetAllJobsQueryHandlerTests
{
    private readonly Mock<IDapperQueryService> _queryServiceMock;
    private readonly GetAllJobsQueryHandler _handler;

    public GetAllJobsQueryHandlerTests()
    {
        _queryServiceMock = new Mock<IDapperQueryService>();
        _handler = new GetAllJobsQueryHandler(_queryServiceMock.Object);
    }

    private static List<JobListDto> FakeJobs(int count) =>
        Enumerable.Range(1, count).Select(i => new JobListDto
        {
            Id = i,
            PublicGuid = Guid.NewGuid(),
            Title = $"Job {i}",
            Location = "Remote",
            IsRemote = true,
            JobType = "FullTime",
            ExperienceLevel = "Mid",
            CompanyPublicGuid = Guid.NewGuid(),
            CompanyName = $"Company {i}",
            CreatedAt = DateTime.UtcNow
        }).ToList();

    [Fact]
    public async Task Handle_ShouldCallCorrectStoredProcedure()
    {
        // Arrange
        _queryServiceMock
            .Setup(q => q.QueryAsync<JobListDto>("sp_GetAllJobs", It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeJobs(5));

        // Act
        await _handler.Handle(new GetAllJobsQuery(), CancellationToken.None);

        // Assert
        _queryServiceMock.Verify(
            q => q.QueryAsync<JobListDto>("sp_GetAllJobs", It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithMoreResults_ShouldReturnHasMoreTrue()
    {
        // Arrange — 21 results for pageSize 20 = has more
        _queryServiceMock
            .Setup(q => q.QueryAsync<JobListDto>("sp_GetAllJobs", It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeJobs(21));

        // Act
        var result = await _handler.Handle(new GetAllJobsQuery(PageSize: 20, Cursor: 0), CancellationToken.None);

        // Assert
        result.HasMore.Should().BeTrue();
        result.Data.Should().HaveCount(20);
        result.NextCursor.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithExactResults_ShouldReturnHasMoreFalse()
    {
        // Arrange — 15 results for pageSize 20 = last page
        _queryServiceMock
            .Setup(q => q.QueryAsync<JobListDto>("sp_GetAllJobs", It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeJobs(15));

        // Act
        var result = await _handler.Handle(new GetAllJobsQuery(PageSize: 20, Cursor: 0), CancellationToken.None);

        // Assert
        result.HasMore.Should().BeFalse();
        result.Data.Should().HaveCount(15);
        result.NextCursor.Should().BeNull();
    }
}
