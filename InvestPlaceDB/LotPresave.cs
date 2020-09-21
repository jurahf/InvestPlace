using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InvestPlaceDB
{
    public class LotPresave
    {
        public LotPresave()
        {
            LotPresaveCategory = new HashSet<LotCategory>();
        }

        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Price { get; set; }

        [StringLength(2000)]
        public string ImageLink { get; set; }

        [StringLength(2000)]
        public string SourceLink { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }

        public int? SellerId { get; set; }


        [ForeignKey(nameof(SellerId))]
        [InverseProperty(nameof(ExtendedUser.LotPresaveSeller))]
        public virtual ExtendedUser Seller { get; set; }


        [InverseProperty("LotPresave")]
        public virtual ICollection<LotCategory> LotPresaveCategory { get; set; }

    }
}
