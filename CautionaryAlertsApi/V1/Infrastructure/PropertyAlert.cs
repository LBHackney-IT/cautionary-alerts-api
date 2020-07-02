using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CautionaryAlertsApi.V1.Infrastructure
{
    public class PropertyAlert
    {
        [Column("addressAlertId")]
        [Key]
        public int Id { get; set; }

        [Column("addressNo")]
        public int AddressId { get; set; }

        [ForeignKey("AddressId")]
        public Address address { get; set; }

        [Column("alertCode")]
        [MaxLength(40)]
        [Required]
        public string AlertCode { get; set; }

        [Column("modDate")]
        [Required]
        public DateTime DateModified { get; set; }

        [Column("startDate")]
        [Required]
        public DateTime StartDate { get; set; }

        [Column("endDate")]
        public DateTime? EndDate { get; set; }

        [Column("modUser")]
        [MaxLength(20)]
        [Required]
        public string ModifiedBy { get; set; }

        [Column("modType")]
        [DefaultValue('I')]
        [Required]
        public char ModifyType { get; set; }
    }
}
