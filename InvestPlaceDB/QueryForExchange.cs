using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InvestPlaceDB
{
    public class QueryForExchange
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime Date { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ModerateDate { get; set; }

        public bool? Moderate { get; set; }

        /// <summary>
        /// Должен быть заполнен или LotId или PazzleId
        /// </summary>
        public int? LotId { get; set; }

        /// <summary>
        /// Должен быть заполнен или LotId или PazzleId
        /// </summary>
        public int? PazzleId { get; set; }

        [ForeignKey(nameof(PazzleId))]
        [InverseProperty(nameof(InvestPlaceDB.Pazzle.QueryForExchange))]
        public virtual Pazzle Pazzle { get; set; }


        [ForeignKey(nameof(LotId))]
        [InverseProperty(nameof(InvestPlaceDB.Lot.QueryForExchange))]
        public virtual Lot Lot { get; set; }


        public int? ExchangeModeratorId { get; set; }

        [ForeignKey(nameof(ExchangeModeratorId))]
        [InverseProperty(nameof(ExtendedUser.ExchangeModerator))]
        public virtual ExtendedUser ExchangeModerator { get; set; }
    }
}
