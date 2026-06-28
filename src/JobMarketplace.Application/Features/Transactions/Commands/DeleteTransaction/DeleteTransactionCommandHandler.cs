using JobMarketplace.Application.Common.Models;
using JobMarketplace.Application.Common.ViewModels.DeleteTransaction;
using JobMarketplace.Application.Features.Countries.Commands.DeleteCountry;
using JobMarketplace.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Transactions.Commands.DeleteTransaction
{
    public class DeleteTransactionCommandHandler : IRequestHandler<DeleteTransactionCommand, Result<DeleteTransactionViewModel>>
    {
        private readonly ICountryRepository _countryRepository;
        private readonly ISkillRepository _skillRepository;

        public DeleteTransactionCommandHandler(ICountryRepository countryRepository, ISkillRepository skillRepository)
        {
            _countryRepository = countryRepository;
            _skillRepository = skillRepository;
        }

        public async Task<Result<DeleteTransactionViewModel>> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
        {
            var country = await _countryRepository.GetByPublicGuidAsync(request.CountryId, cancellationToken);
            var skill = await _skillRepository.GetByPublicGuidAsync(request.SkillId, cancellationToken);

            if (country is null)
                return Result<DeleteTransactionViewModel>.Failure($"Country with Id '{request.CountryId}' not found.");

            if (skill is null)
                return Result<DeleteTransactionViewModel>.Failure($"Skill with Id '{request.SkillId}' not found.");

            _countryRepository.Remove(country);
            await _countryRepository.SaveChangesAsync(cancellationToken);
            _skillRepository.Remove(skill);
            await _skillRepository.SaveChangesAsync(cancellationToken);

            var deleteTransactionViewModel = new DeleteTransactionViewModel
            {
                CountryId = country.PublicGuid,
                SkillId = skill.PublicGuid
            };

            return Result<DeleteTransactionViewModel>.Success(deleteTransactionViewModel);

        }
    }
}
