using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Domain.Enums
{
    public enum ApplicationStatus
    {
        Submitted = 0,
        UnderReview = 1,
        Shortlisted = 2,
        InterviewScheduled = 3,
        Offered = 4,
        Accepted = 5,
        Rejected = 6,
        Withdrawn = 7
    }
}
