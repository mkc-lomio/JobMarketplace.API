using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Applications.Commands.CreateApplication
{
    public class CreateApplicationCommandValidator : AbstractValidator<CreateApplicationCommand>
    {
        public CreateApplicationCommandValidator()
        {
            RuleFor(x => x.JobPublicGuid)
                .NotEmpty();

            RuleFor(x => x.ApplicantName)
                .NotEmpty().MaximumLength(150);

            RuleFor(x => x.ApplicantEmail)
                .NotEmpty().EmailAddress();
        }
    }
}
