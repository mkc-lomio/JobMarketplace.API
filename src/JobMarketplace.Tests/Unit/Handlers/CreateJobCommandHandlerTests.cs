using AutoMapper;
using FluentAssertions;
using JobMarketplace.Application.Common.Mappings;
using JobMarketplace.Application.Features.Jobs.Commands.CreateJob;
using JobMarketplace.Domain.Entities;
using JobMarketplace.Domain.Enums;
using JobMarketplace.Domain.Interfaces;
using Moq;

namespace JobMarketplace.Tests.Unit.Handlers;

public class CreateJobCommandHandlerTests
{
    private readonly Mock<IJobRepository> _jobRepoMock;
    private readonly Mock<ICompanyRepository> _companyRepoMock;
    private readonly IMapper _mapper;
    private readonly CreateJobCommandHandler _handler;

    public CreateJobCommandHandlerTests()
    {
        _jobRepoMock = new Mock<IJobRepository>();
        _companyRepoMock = new Mock<ICompanyRepository>();
        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();
        _handler = new CreateJobCommandHandler(_jobRepoMock.Object, _companyRepoMock.Object, _mapper);
    }

    private static CreateJobCommand ValidCommand(Guid companyGuid) => new()
    {
        Title = "Senior Developer",
        Description = "Build stuff",
        Location = "Remote",
        CompanyPublicGuid = companyGuid,
        JobType = JobType.FullTime,
        ExperienceLevel = ExperienceLevel.Senior
    };

    [Fact]
    public async Task Handle_ValidCommand_ShouldReturnSuccess()
    {
        // Arrange
        var companyGuid = Guid.NewGuid();
        var company = new Company { Id = 42, PublicGuid = companyGuid, Name = "Test Corp" };

        _companyRepoMock.Setup(r => r.GetByPublicGuidAsync(companyGuid, It.IsAny<CancellationToken>()))
            .ReturnsAsync(company);
        _jobRepoMock.Setup(r => r.AddAsync(It.IsAny<Job>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _jobRepoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(ValidCommand(companyGuid), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_CompanyNotFound_ShouldReturnFailure()
    {
        // Arrange — company doesn't exist
        var fakeGuid = Guid.NewGuid();
        _companyRepoMock.Setup(r => r.GetByPublicGuidAsync(fakeGuid, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Company?)null);

        // Act
        var result = await _handler.Handle(ValidCommand(fakeGuid), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task Handle_ShouldSetCompanyIdFromResolvedCompany()
    {
        // Arrange
        var companyGuid = Guid.NewGuid();
        var company = new Company { Id = 99, PublicGuid = companyGuid, Name = "Test Corp" };

        Job? capturedJob = null;
        _companyRepoMock.Setup(r => r.GetByPublicGuidAsync(companyGuid, It.IsAny<CancellationToken>()))
            .ReturnsAsync(company);
        _jobRepoMock.Setup(r => r.AddAsync(It.IsAny<Job>(), It.IsAny<CancellationToken>()))
            .Callback<Job, CancellationToken>((j, _) => capturedJob = j)
            .Returns(Task.CompletedTask);
        _jobRepoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(ValidCommand(companyGuid), CancellationToken.None);

        // Assert — handler resolved PublicGuid → internal CompanyId
        capturedJob.Should().NotBeNull();
        capturedJob!.CompanyId.Should().Be(99);
        capturedJob.Status.Should().Be(JobStatus.Active);
    }

    [Fact]
    public async Task Handle_CompanyNotFound_ShouldNotCallJobRepository()
    {
        // Arrange
        _companyRepoMock.Setup(r => r.GetByPublicGuidAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Company?)null);

        // Act
        await _handler.Handle(ValidCommand(Guid.NewGuid()), CancellationToken.None);

        // Assert — if company doesn't exist, we should never try to save a job
        _jobRepoMock.Verify(r => r.AddAsync(It.IsAny<Job>(), It.IsAny<CancellationToken>()), Times.Never);
        _jobRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
