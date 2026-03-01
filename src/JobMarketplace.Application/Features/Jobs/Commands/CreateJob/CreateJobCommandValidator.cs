using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Jobs.Commands.CreateJob
{
    public class CreateJobCommandValidator : AbstractValidator<CreateJobCommand>
    {
        public CreateJobCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().MaximumLength(200);

            RuleFor(x => x.Description)
                .NotEmpty();

            RuleFor(x => x.Location)
                .NotEmpty().MaximumLength(200);

            RuleFor(x => x.CompanyPublicGuid)
                .NotEmpty().WithMessage("Company is required.");

            RuleFor(x => x.SalaryMin)
                .LessThanOrEqualTo(x => x.SalaryMax)
                .When(x => x.SalaryMin.HasValue && x.SalaryMax.HasValue);
        }
    }
}
