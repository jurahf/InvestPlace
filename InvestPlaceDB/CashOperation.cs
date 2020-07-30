using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvestPlaceDB
{
    public partial class CashOperation
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Date { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Summ { get; set; }
        public string Comment { get; set; }
        public int CashId { get; set; }

        [ForeignKey(nameof(CashId))]
        [InverseProperty("CashOperation")]
        public virtual Cash Cash { get; set; }
    }
}
