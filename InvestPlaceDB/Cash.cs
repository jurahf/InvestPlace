using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvestPlaceDB
{
    public partial class Cash
    {
        public Cash()
        {
            CashOperation = new HashSet<CashOperation>();
            ExtendedUser = new HashSet<ExtendedUser>();
        }

        [Key]
        public int Id { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Summ { get; set; }

        [InverseProperty("Cash")]
        public virtual ICollection<CashOperation> CashOperation { get; set; }
        [InverseProperty("Cash")]
        public virtual ICollection<ExtendedUser> ExtendedUser { get; set; }
    }
}
