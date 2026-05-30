using JobMarketplace.Application.Common.Models;
using JobMarketplace.Application.Features.Countries.Commands.DeleteCountry;
using JobMarketplace.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Skills.Commands.DeleteSkill
{
    public class DeleteSkillCommandHandler : IRequestHandler<DeleteSkillCommand, Result<bool>>
    {
        private readonly ISkillRepository _skillRepository;

        public DeleteSkillCommandHandler(ISkillRepository skillRepository)
        {
            _skillRepository = skillRepository;
        }

        public async Task<Result<bool>> Handle(DeleteSkillCommand request, CancellationToken cancellationToken)
        {
            var skill = await _skillRepository.GetByPublicGuidAsync(request.PublicGuid, cancellationToken);

            if (skill is null)
                return Result<bool>.Failure($"Skill with Id '{request.PublicGuid}' not found.");

            _skillRepository.Remove(skill);
            await _skillRepository.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
