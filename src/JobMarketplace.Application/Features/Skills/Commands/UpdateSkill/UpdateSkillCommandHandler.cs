using JobMarketplace.Application.Common.Models;
using JobMarketplace.Application.Features.Countries.Commands.UpdateCountry;
using JobMarketplace.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Skills.Commands.UpdateSkill
{
    public class UpdateSkillCommandHandler : IRequestHandler<UpdateSkillCommand, Result<bool>>
    {
        private readonly ISkillRepository _skillRepository;
        public UpdateSkillCommandHandler(ISkillRepository skillRepository)
        {
            _skillRepository = skillRepository;
        }
        public async Task<Result<bool>> Handle(UpdateSkillCommand request, CancellationToken cancellationToken)
        {
            var skill = await _skillRepository.GetByPublicGuidAsync(request.PublicGuid, cancellationToken);
            if (skill is null)
                return Result<bool>.Failure($"SKill with Id '{request.PublicGuid}' not found.");

            skill.Name = request.Name;
            skill.Description = request.Description;

            _skillRepository.Update(skill);
            await _skillRepository.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}
