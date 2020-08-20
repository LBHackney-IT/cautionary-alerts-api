using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CautionaryAlertsApi.V1.Infrastructure
{
    [Table("W2CPickS", Schema = "dbo")]
    public class AlertDescriptionLookup
    {
        [Column("code")]
        [MaxLength(40)]
        public string AlertCode { get; set; }

        [Column("descx")]
        [MaxLength(100)]
        public string Description { get; set; }

        [Column("PickType")]
        [MaxLength(10)]
        public string PickType { get; set; }

        [Column("MODDATE")]
        public DateTime DateModified { get; set; }

        [Column("MODUSER")]
        [MaxLength(20)]
        public string ModifiedBy { get; set; }

        [Column("MODTYPE")]
        [DefaultValue('I')]
        [Required]
        public char ModifyType { get; set; }
    }
}
