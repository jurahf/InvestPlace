using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvestPlaceDB
{
    public partial class LotCategory
    {
        [Key]
        public int Id { get; set; }

        public int? LotId { get; set; }

        public int? LotPresaveId { get; set; }

        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        [InverseProperty("LotCategory")]
        public virtual Category Category { get; set; }


        [ForeignKey(nameof(LotId))]
        [InverseProperty("LotCategory")]
        public virtual Lot Lot { get; set; }


        [ForeignKey(nameof(LotPresaveId))]
        [InverseProperty("LotPresaveCategory")]
        public virtual LotPresave LotPresave { get; set; }
    }
}
