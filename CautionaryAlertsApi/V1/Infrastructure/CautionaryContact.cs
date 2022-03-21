using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CautionaryAlertsApi.V1.Infrastructure
{
    [Table("cautionary_contacts", Schema = "dbo")]
    public class CautionaryContact
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("door_number")]
        public string DoorNumber { get; set; }

        [Column("address")]
        public string Address { get; set; }

        [Column("neighbourhood")]
        public string Neighbourhood { get; set; }

        [Column("date_of_incident")]
        public string DateOfIncident { get; set; }

        [Column("code")]
        public string Code { get; set; }

        [Column("caution_on_system")]
        public string CautionOnSystem { get; set; }

        [Column("property_reference")]
        public string PropertyReference { get; set; }

        [Column("uprn")]
        public string UPRN { get; set; }
    }
}
