using AutoMapper;
using JobMarketplace.Application.Common.Models;
using JobMarketplace.Application.Common.ViewModels;
using JobMarketplace.Application.Features.Countries.Commands.CreateCountry;
using JobMarketplace.Application.Features.Skills.Commands.CreateSkill;
using JobMarketplace.Domain.Entities;
using JobMarketplace.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace JobMarketplace.Application.Features.Transactions.Commands
{
    public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, Result<CreateTransactionViewModel>>
    {
        private readonly ICountryRepository _countryRepository;
        private readonly ISkillRepository _skillRepository;
        private readonly IMapper _mapper;

        public CreateTransactionCommandHandler(ICountryRepository countryRepository, ISkillRepository skillRepository, IMapper mapper)
        {
            _countryRepository = countryRepository;
            _skillRepository = skillRepository;
            _mapper = mapper;
        }

        public async Task<Result<CreateTransactionViewModel>> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            
            var country = _mapper.Map<Country>(request.CountryViewModel);
            var skill = _mapper.Map<Skill>(request.SkillViewModel);

            await _countryRepository.AddAsync(country, cancellationToken);
            await _countryRepository.SaveChangesAsync(cancellationToken);
            await _skillRepository.AddAsync(skill, cancellationToken);
            await _skillRepository.SaveChangesAsync(cancellationToken);

            var transactionCreateViewModel = new CreateTransactionViewModel
            {
                CountryId = country.PublicGuid,
                SkillId = skill.PublicGuid
            };

            return Result<CreateTransactionViewModel>.Success(transactionCreateViewModel);
        }

    }
}
