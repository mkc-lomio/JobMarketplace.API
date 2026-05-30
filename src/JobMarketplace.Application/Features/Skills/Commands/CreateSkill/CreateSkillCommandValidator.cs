using FluentValidation;
using JobMarketplace.Application.Features.Countries.Commands.CreateCountry;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Skills.Commands.CreateSkill
{
    public class CreateSkillCommandValidator : AbstractValidator<CreateSkillCommand>
    {
        public CreateSkillCommandValidator() { 
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Skill name is required.");
        }
    }
}
