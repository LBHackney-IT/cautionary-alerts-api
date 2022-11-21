using System;

namespace CautionaryAlertsApi.V1.Boundary.Request
{
    public class AssetDetails
    {
        public Guid Id { get; set; }
        public string PropertyReference { get; set; }
        public string UPRN { get; set; }
        public string FullAddress { get; set; }
    }
}
