using AutoMapper;
using FluentAssertions;
using JobMarketplace.Application.Common.Mappings;
using JobMarketplace.Application.Features.Companies.Commands.CreateCompany;
using JobMarketplace.Domain.Entities;
using JobMarketplace.Domain.Interfaces;
using Moq;

namespace JobMarketplace.Tests.Unit.Handlers;

public class CreateCompanyCommandHandlerTests
{
    private readonly Mock<ICompanyRepository> _companyRepoMock;
    private readonly IMapper _mapper;
    private readonly CreateCompanyCommandHandler _handler;

    public CreateCompanyCommandHandlerTests()
    {
        _companyRepoMock = new Mock<ICompanyRepository>();
        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();
        _handler = new CreateCompanyCommandHandler(_companyRepoMock.Object, _mapper);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldReturnSuccessWithGuid()
    {
        // Arrange
        _companyRepoMock.Setup(r => r.AddAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _companyRepoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new CreateCompanyCommand
        {
            Name = "Test Corp",
            Description = "A test company",
            Industry = "Tech",
            Location = "Manila",
            ContactEmail = "test@corp.com",
            FoundedYear = 2020
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldCallAddAsyncAndSaveChanges()
    {
        // Arrange
        _companyRepoMock.Setup(r => r.AddAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _companyRepoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new CreateCompanyCommand
        {
            Name = "Test Corp",
            Description = "Desc",
            Industry = "Tech",
            Location = "Manila",
            ContactEmail = "test@corp.com",
            FoundedYear = 2020
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert — verify repository methods were called exactly once
        _companyRepoMock.Verify(r => r.AddAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()), Times.Once);
        _companyRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldMapCommandToEntity()
    {
        // Arrange
        Company? capturedCompany = null;
        _companyRepoMock.Setup(r => r.AddAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()))
            .Callback<Company, CancellationToken>((c, _) => capturedCompany = c)
            .Returns(Task.CompletedTask);
        _companyRepoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new CreateCompanyCommand
        {
            Name = "Mapped Corp",
            Description = "Testing mapper",
            Industry = "Finance",
            Location = "Makati",
            ContactEmail = "map@corp.com",
            FoundedYear = 2015
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert — AutoMapper correctly mapped command fields to entity
        capturedCompany.Should().NotBeNull();
        capturedCompany!.Name.Should().Be("Mapped Corp");
        capturedCompany.Industry.Should().Be("Finance");
        capturedCompany.ContactEmail.Should().Be("map@corp.com");
    }
}
