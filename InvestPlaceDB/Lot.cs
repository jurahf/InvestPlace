using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvestPlaceDB
{
    public partial class Lot
    {
        public Lot()
        {
            LotCategory = new HashSet<LotCategory>();
            Pazzle = new HashSet<Pazzle>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
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

        [Column(TypeName = "datetime")]
        public DateTime? CompleteDate { get; set; }

        public bool? CreateModerate { get; set; }

        public bool? CompleteModerate { get; set; }

        public int? CreateModeratorId { get; set; }

        public int? CompleteModeratorId { get; set; }

        public int? PriceRangeId { get; set; }

        public int? SellerId { get; set; }



        [ForeignKey(nameof(PriceRangeId))]
        [InverseProperty(nameof(InvestPlaceDB.PriceRange.LotPriceRange))]
        public virtual PriceRange PriceRange { get; set; }


        [ForeignKey(nameof(CompleteModeratorId))]
        [InverseProperty(nameof(ExtendedUser.LotCompleteModerator))]
        public virtual ExtendedUser CompleteModerator { get; set; }


        [ForeignKey(nameof(CreateModeratorId))]
        [InverseProperty(nameof(ExtendedUser.LotCreateModerator))]
        public virtual ExtendedUser CreateModerator { get; set; }


        [ForeignKey(nameof(SellerId))]
        [InverseProperty(nameof(ExtendedUser.LotSeller))]
        public virtual ExtendedUser Seller { get; set; }




        [InverseProperty("Lot")]
        public virtual ICollection<LotCategory> LotCategory { get; set; }

        [InverseProperty("Lot")]
        public virtual ICollection<Pazzle> Pazzle { get; set; }
    }
}
