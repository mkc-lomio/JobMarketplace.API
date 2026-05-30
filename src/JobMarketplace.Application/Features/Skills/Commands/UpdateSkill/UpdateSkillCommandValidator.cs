using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Skills.Commands.UpdateSkill
{
    public class UpdateSkillCommandValidator :AbstractValidator<UpdateSkillCommand>
    {
        public UpdateSkillCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Skill name is required.");
            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Skill description is required.");
        }
    }
}
