using FluentValidation;
using JobMarketplace.Application.Features.Companies.Commands.CreateCompany;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Countries.Commands.CreateCountry
{
    public class CreateCountryCommandValidator : AbstractValidator<CreateCountryCommand>
    {
        public CreateCountryCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Country name is required.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Country description is required.");

            RuleFor(x => x.FlagUrl)
                .NotEmpty()
                .WithMessage("Country flag URL is required.");
        }
    }
}
