using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvestPlaceDB
{
    public partial class ExtendedUser : IdentityUser<int>
    {
        public ExtendedUser()
        {
            LotCompleteModerator = new HashSet<Lot>();
            LotCreateModerator = new HashSet<Lot>();
            PazzleBuyer = new HashSet<Pazzle>();
            PazzleChangeModerator = new HashSet<Pazzle>();
            SellerNavigation = new HashSet<Seller>();
            CashQueryModerator = new HashSet<QueryForOperation>();
        }


        public int? SellerId { get; set; }

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

        [ForeignKey(nameof(SellerId))]
        [InverseProperty("ExtendedUser")]
        public virtual Seller Seller { get; set; }

        [InverseProperty(nameof(Lot.CompleteModerator))]
        public virtual ICollection<Lot> LotCompleteModerator { get; set; }

        [InverseProperty(nameof(Lot.CreateModerator))]
        public virtual ICollection<Lot> LotCreateModerator { get; set; }

        [InverseProperty(nameof(Pazzle.Buyer))]
        public virtual ICollection<Pazzle> PazzleBuyer { get; set; }

        [InverseProperty(nameof(Pazzle.ChangeModerator))]
        public virtual ICollection<Pazzle> PazzleChangeModerator { get; set; }

        [InverseProperty("ContractModerator")]
        public virtual ICollection<Seller> SellerNavigation { get; set; }

        [InverseProperty(nameof(QueryForOperation.CashQueryModerator))]
        public virtual ICollection<QueryForOperation> CashQueryModerator { get; set; }
    }
}
