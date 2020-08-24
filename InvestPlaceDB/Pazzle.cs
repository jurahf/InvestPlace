using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvestPlaceDB
{
    public partial class Pazzle
    {
        public Pazzle()
        {
            QueryForExchange = new HashSet<QueryForExchange>();
        }

        [Key]
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool? Winner { get; set; }

        public int? BasketId { get; set; }

        /// <summary>
        /// Дата и время покупки пазла
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? BuyDate { get; set; }

        /// <summary>
        /// Покупатель
        /// </summary>
        public int? BuyerId { get; set; }

        public int? LotId { get; set; }

        [ForeignKey(nameof(BasketId))]
        [InverseProperty("Pazzle")]
        public virtual Basket Basket { get; set; }


        [ForeignKey(nameof(BuyerId))]
        [InverseProperty(nameof(ExtendedUser.PazzleBuyer))]
        public virtual ExtendedUser Buyer { get; set; }


        [ForeignKey(nameof(LotId))]
        [InverseProperty("Pazzle")]
        public virtual Lot Lot { get; set; }


        [InverseProperty(nameof(InvestPlaceDB.QueryForExchange.Pazzle))]
        public virtual ICollection<QueryForExchange> QueryForExchange { get; set; }
    }
}
