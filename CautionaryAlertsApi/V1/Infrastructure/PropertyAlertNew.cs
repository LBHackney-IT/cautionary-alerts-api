using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CautionaryAlertsApi.V1.Infrastructure
{
    [Table("PropertyAlertNew", Schema = "dbo")]
    public class PropertyAlertNew
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("door_number")]
        [MaxLength(10)]
        public string DoorNumber { get; set; }

        [Column("address")]
        [MaxLength(50)]
        public string Address { get; set; }

        [Column("neighbourhood")]
        [MaxLength(20)]
        public string Neighbourhood { get; set; }

        [Column("date_of_incident")]
        [MaxLength(12)]
        public string DateOfIncident { get; set; }

        [Column("code")]
        [MaxLength(3)]
        public string Code { get; set; }

        [Column("caution_on_system")]
        [MaxLength(50)]
        public string CautionOnSystem { get; set; }

        [Column("property_reference")]
        [MaxLength(12)]
        public string PropertyReference { get; set; }

        [Column("uprn")]
        [MaxLength(12)]
        public string UPRN { get; set; }

        [Column("person_name")]
        [MaxLength(100)]
        public string PersonName { get; set; }

        [Column("mmh_id")]
        [MaxLength(36)]
        public string MMHID { get; set; }

    }
}
