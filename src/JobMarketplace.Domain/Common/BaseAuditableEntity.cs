using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Domain.Common
{
    public abstract class BaseAuditableEntity
    {
        public long Id { get; set; }               // BIGINT IDENTITY(1,1) — internal PK, clustered
        public Guid PublicGuid { get; set; }          // UNIQUEIDENTIFIER — exposed in APIs
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public string? LastModifiedBy { get; set; }
    }
}
