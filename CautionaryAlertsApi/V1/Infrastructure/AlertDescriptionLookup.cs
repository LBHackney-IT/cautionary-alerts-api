using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CautionaryAlertsApi.V1.Infrastructure
{
    [Table("W2CPICKSDCH", Schema = "dbo")]
    public class AlertDescriptionLookup
    {
        [Column("CODE")]
        [MaxLength(40)]
        [Required]
        public string AlertCode { get; set; }

        [Column("DESCX")]
        [MaxLength(100)]
        public string Description { get; set; }

        [Column("PICKTYPE")]
        [MaxLength(10)]
        [Required]
        public string PickType { get; set; }

        [Column("MODDATE")]
        [Required]
        public DateTime DateModified { get; set; }

        [Column("MODUSER")]
        [MaxLength(20)]
        [Required]
        public string ModifiedBy { get; set; }

        [Column("MODTYPE")]
        [DefaultValue('I')]
        public char ModifyType { get; set; }
    }
}
