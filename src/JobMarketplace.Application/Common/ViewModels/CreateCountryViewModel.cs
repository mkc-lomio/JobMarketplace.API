using JobMarketplace.Application.Features.Countries.Commands.CreateCountry;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Common.ViewModels
{
    public record CreateCountryViewModel
    {
        public string FlagUrl { get; set; } = string.Empty;
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
