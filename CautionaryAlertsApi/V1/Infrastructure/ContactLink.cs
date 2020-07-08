using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CautionaryAlertsApi.V1.Infrastructure
{

    [Table("cccontactlink")]
    public class ContactLink
    {
        [Column("contactno")]
        [Key]
        public int ContactNumber { get; set; }

        [Column("key1")]
        [StringLength(20)]
        //Can be a few types of reference including Tag ref and House Ref
        public string Key { get; set; }

        [Column("key2")]
        [StringLength(10)]
        public string PersonNumber { get; set; }

        [Column("linktype")]
        [MaxLength(40)]
        public string LinkType { get; set; }

        [Column("linkno")]
        public int LinkNumber { get; set; }

        [Column("modtype")]
        [MaxLength(1)]
        public string ModifyType { get; set; }

        [Column("moddate")]
        public DateTime? DateModified { get; set; }
    }
}
