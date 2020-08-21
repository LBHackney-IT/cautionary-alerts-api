using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CautionaryAlertsApi.V1.Infrastructure
{
    [Table("CCAddress", Schema = "dbo")]
    public class AddressLink
    {
        [Key]
        [Column("UPRN")]
        public int AddressNumber { get; set; }

        [Column("PropertyReference")]
        [MaxLength(12)]
        public string PropertyReference { get; set; }

        [Column("RealUPRN")]
        [MaxLength(12)]
        public string UPRN { get; set; }

        [Column("modDate")]
        public DateTime DateModified { get; set; }

        [Column("modUser")]
        [MaxLength(20)]
        [Required]
        public string ModifiedBy { get; set; }

        [Column("modType")]
        [Required]
        public char ModifyType { get; set; }
    }
}
