using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvestPlaceDB
{
    public partial class Pazzle
    {
        [Key]
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool? Winner { get; set; }
        public bool? Changed { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ChangeDate { get; set; }
        public int? ChangeModeratorId { get; set; }
        public int? BasketId { get; set; }
        public int? BuyerId { get; set; }
        public int? LotId { get; set; }

        [ForeignKey(nameof(BasketId))]
        [InverseProperty("Pazzle")]
        public virtual Basket Basket { get; set; }
        [ForeignKey(nameof(BuyerId))]
        [InverseProperty(nameof(ExtendedUser.PazzleBuyer))]
        public virtual ExtendedUser Buyer { get; set; }
        [ForeignKey(nameof(ChangeModeratorId))]
        [InverseProperty(nameof(ExtendedUser.PazzleChangeModerator))]
        public virtual ExtendedUser ChangeModerator { get; set; }
        [ForeignKey(nameof(LotId))]
        [InverseProperty("Pazzle")]
        public virtual Lot Lot { get; set; }
    }
}
