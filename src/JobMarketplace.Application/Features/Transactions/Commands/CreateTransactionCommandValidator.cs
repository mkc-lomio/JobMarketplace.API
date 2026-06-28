using FluentValidation;
using JobMarketplace.Application.Features.Skills.Commands.CreateSkill;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Transactions.Commands
{
    public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
    {
        public CreateTransactionCommandValidator() { 
            RuleFor(x => x.CountryViewModel.FlagUrl)
                .NotNull()
                .WithMessage("Flag Url is required.");
            RuleFor(x => x.CountryViewModel.Name)
                .NotNull()
                .WithMessage("Country Name is required");
            RuleFor(x => x.CountryViewModel.Description)
               .NotNull()
               .WithMessage("Country Description is required");
            RuleFor(x => x.SkillViewModel.Name)
                .NotNull()
                .WithMessage("Skill Name is required");
            RuleFor(x => x.SkillViewModel.Description)
                .NotNull()
                .WithMessage("Skill Description is required");
        }
    }
}
