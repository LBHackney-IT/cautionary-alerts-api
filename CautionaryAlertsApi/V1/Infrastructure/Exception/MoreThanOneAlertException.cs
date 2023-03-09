using System;

namespace CautionaryAlertsApi.Tests.V1.Infrastructure
{
    public class MoreThanOneAlertException : Exception
    {
        public int AlertCount { get; private set; }

        public MoreThanOneAlertException(int alertCount)
            : base(string.Format("There is currnetly {0} related alerts in the database. There should only be one", alertCount))
        {
            AlertCount = alertCount;
        }
    }
}
