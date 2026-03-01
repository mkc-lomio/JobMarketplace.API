using FluentAssertions;
using JobMarketplace.Application.Features.Companies.Commands.CreateCompany;

namespace JobMarketplace.Tests.Unit.Validators;

public class CreateCompanyCommandValidatorTests
{
    private readonly CreateCompanyCommandValidator _validator = new();

    private static CreateCompanyCommand ValidCommand() => new()
    {
        Name = "Test Company",
        Description = "A test company",
        Industry = "Technology",
        Location = "Manila, PH",
        ContactEmail = "test@company.com",
        FoundedYear = 2020
    };

    [Fact]
    public async Task ValidCommand_ShouldPassValidation()
    {
        var result = await _validator.ValidateAsync(ValidCommand());
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task EmptyName_ShouldFail(string? name)
    {
        var command = ValidCommand() with { Name = name! };
        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task NameTooLong_ShouldFail()
    {
        var command = ValidCommand() with { Name = new string('A', 201) };
        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    [InlineData("missing@")]
    public async Task InvalidEmail_ShouldFail(string email)
    {
        var command = ValidCommand() with { ContactEmail = email };
        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ContactEmail");
    }

    [Theory]
    [InlineData(1799)]
    [InlineData(2099)]
    public async Task InvalidFoundedYear_ShouldFail(int year)
    {
        var command = ValidCommand() with { FoundedYear = year };
        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FoundedYear");
    }

    [Fact]
    public async Task InvalidWebsite_ShouldFail()
    {
        var command = ValidCommand() with { Website = "not-a-url" };
        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Website");
    }

    [Fact]
    public async Task ValidWebsite_ShouldPass()
    {
        var command = ValidCommand() with { Website = "https://company.com" };
        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task NullWebsite_ShouldPass()
    {
        var command = ValidCommand() with { Website = null };
        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeTrue();
    }
}
