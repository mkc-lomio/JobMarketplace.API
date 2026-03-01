using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Domain.Common
{
    /// <summary>
    /// Base class for all entities. Provides the Dual-ID pattern + audit fields.
    /// 
    /// Dual-ID: Id (BIGINT) for fast internal joins, PublicGuid (GUID) for secure external APIs.
    /// Why both? BIGINT is fast for indexes/joins. GUID is unguessable (no enumeration attacks).
    /// 
    /// Audit fields: auto-filled by AuditableEntityInterceptor — never set manually.
    /// </summary>
    public abstract class BaseAuditableEntity
    {
        public long Id { get; set; }               // Internal PK — BIGINT IDENTITY, clustered. Never exposed in API.
        public Guid PublicGuid { get; set; }        // External ID — NEWSEQUENTIALID(), unique index. Used in URLs/responses.
        public DateTime CreatedAt { get; set; }     // Auto-set on insert by interceptor
        public string? CreatedBy { get; set; }      // Will hold user ID once auth is added 
        public DateTime? LastModifiedAt { get; set; } // Auto-set on update by interceptor
        public string? LastModifiedBy { get; set; }   // Will hold user ID once auth is added 
    }
}