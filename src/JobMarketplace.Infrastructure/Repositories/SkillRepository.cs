using JobMarketplace.Domain.Entities;
using JobMarketplace.Domain.Interfaces;
using JobMarketplace.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Infrastructure.Repositories
{
    public class SkillRepository : GenericRepository<Skill>, ISkillRepository
    {
        public SkillRepository(ApplicationDbContext context) : base(context) { }
    }
}
