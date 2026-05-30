using JobMarketplace.Application.Common.Models;
using JobMarketplace.Application.Features.Jobs.Commands.UpdateJob;
using JobMarketplace.Domain.Entities;
using JobMarketplace.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Countries.Commands.UpdateCountry
{
    public class UpdateCountryCommandHandler : IRequestHandler<UpdateCountryCommand, Result<bool>>
    {
        private readonly ICountryRepository _countryRepository;

        public UpdateCountryCommandHandler(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public async Task<Result<bool>> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
        {
            var country = await _countryRepository.GetByPublicGuidAsync(request.PublicGuid, cancellationToken);
            if (country is null)
                return Result<bool>.Failure($"Country with Id '{request.PublicGuid}' not found.");

            country.FlagUrl = request.FlagUrl;
            country.Name = request.Name;
            country.Description = request.Description;

            _countryRepository.Update(country);
            await _countryRepository.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}
