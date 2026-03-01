using FluentAssertions;
using JobMarketplace.Application.Features.Jobs.Commands.CreateJob;
using JobMarketplace.Domain.Enums;

namespace JobMarketplace.Tests.Unit.Validators;

public class CreateJobCommandValidatorTests
{
    private readonly CreateJobCommandValidator _validator = new();

    private static CreateJobCommand ValidCommand() => new()
    {
        Title = "Senior Developer",
        Description = "Build cool stuff",
        Location = "Remote",
        CompanyPublicGuid = Guid.NewGuid(),
        JobType = JobType.FullTime,
        ExperienceLevel = ExperienceLevel.Senior
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
    public async Task EmptyTitle_ShouldFail(string? title)
    {
        var command = ValidCommand() with { Title = title! };
        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Fact]
    public async Task TitleTooLong_ShouldFail()
    {
        var command = ValidCommand() with { Title = new string('A', 201) };
        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task EmptyDescription_ShouldFail()
    {
        var command = ValidCommand() with { Description = "" };
        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }

    [Fact]
    public async Task EmptyCompanyGuid_ShouldFail()
    {
        var command = ValidCommand() with { CompanyPublicGuid = Guid.Empty };
        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CompanyPublicGuid");
    }

    [Fact]
    public async Task SalaryMinGreaterThanMax_ShouldFail()
    {
        var command = ValidCommand() with { SalaryMin = 100000, SalaryMax = 50000 };
        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "SalaryMin");
    }

    [Fact]
    public async Task ValidSalaryRange_ShouldPass()
    {
        var command = ValidCommand() with { SalaryMin = 50000, SalaryMax = 100000 };
        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task NullSalary_ShouldPass()
    {
        var command = ValidCommand() with { SalaryMin = null, SalaryMax = null };
        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeTrue();
    }
}
