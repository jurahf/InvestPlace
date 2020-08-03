using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InvestPlaceDB
{
    public class PriceRange
    {
        public PriceRange()
        {
            LotPriceRange = new HashSet<Lot>();
        }

        [Key]
        public int Id { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Minimum { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Maximum { get; set; }

        [InverseProperty(nameof(Lot.PriceRange))]
        public virtual ICollection<Lot> LotPriceRange { get; set; }
    }
}
