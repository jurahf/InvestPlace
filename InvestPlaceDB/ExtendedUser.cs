using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvestPlaceDB
{
    public partial class ExtendedUser : IdentityUser<int>
    {
        public ExtendedUser()
        {
            LotCreateModerator = new HashSet<Lot>();
            PazzleBuyer = new HashSet<Pazzle>();
            CashQueryModerator = new HashSet<QueryForOperation>();
            LotSeller = new HashSet<Lot>();
            ExchangeModerator = new HashSet<QueryForExchange>();
        }

        [DefaultValue(30)]
        public int ExchangeLevel { get; set; }

        public int? BasketId { get; set; }

        public int? CashId { get; set; }

        public int? ExchangeLevelPercent { get; set; }

        [StringLength(200)]
        public string InnerName { get; set; }

        [StringLength(200)]
        public string Surname { get; set; }

        [StringLength(200)]
        public string Patronymic { get; set; }

        [StringLength(200)]
        public string SchetNumber { get; set; }

        [StringLength(200)]
        public string CorrSchet { get; set; }

        [StringLength(1000)]
        public string Bank { get; set; }

        [StringLength(50)]
        public string INN { get; set; }

        [StringLength(50)]
        public string BIK { get; set; }

        [StringLength(50)]
        public string KPP { get; set; }

        [StringLength(1000)]
        public string Address { get; set; }


        [ForeignKey(nameof(BasketId))]
        [InverseProperty("ExtendedUser")]
        public virtual Basket Basket { get; set; }

        [ForeignKey(nameof(CashId))]
        [InverseProperty("ExtendedUser")]
        public virtual Cash Cash { get; set; }


        [InverseProperty(nameof(Lot.Seller))]
        public virtual ICollection<Lot> LotSeller { get; set; }

        [InverseProperty(nameof(Lot.CreateModerator))]
        public virtual ICollection<Lot> LotCreateModerator { get; set; }

        [InverseProperty(nameof(Pazzle.Buyer))]
        public virtual ICollection<Pazzle> PazzleBuyer { get; set; }

        [InverseProperty(nameof(QueryForOperation.CashQueryModerator))]
        public virtual ICollection<QueryForOperation> CashQueryModerator { get; set; }


        [InverseProperty(nameof(QueryForExchange.ExchangeModerator))]
        public virtual ICollection<QueryForExchange> ExchangeModerator { get; set; }
    }
}
