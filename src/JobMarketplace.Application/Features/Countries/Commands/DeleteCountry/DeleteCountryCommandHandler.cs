using JobMarketplace.Application.Common.Models;
using JobMarketplace.Application.Features.Jobs.Commands.DeleteJob;
using JobMarketplace.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Features.Countries.Commands.DeleteCountry
{
    public class DeleteCountryCommandHandler : IRequestHandler<DeleteCountryCommand, Result<bool>>
    {
        private readonly ICountryRepository _countryRepository;

        public DeleteCountryCommandHandler(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public async Task<Result<bool>> Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
        {
            var country = await _countryRepository.GetByPublicGuidAsync(request.PublicGuid, cancellationToken);

            if (country is null)
                return Result<bool>.Failure($"Country with Id '{request.PublicGuid}' not found.");

            _countryRepository.Remove(country);
            await _countryRepository.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
