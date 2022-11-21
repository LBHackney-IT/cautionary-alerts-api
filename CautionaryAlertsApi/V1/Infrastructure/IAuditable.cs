using System;

namespace CautionaryAlertsApi.V1.Infrastructure
{
    public interface IAuditable
    {
        DateTime DateModified { get; set; }

        string ModifiedBy { get; set; }
    }
}
