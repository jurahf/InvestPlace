using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvestPlaceDB
{
    public partial class Seller
    {
        public Seller()
        {
            ExtendedUser = new HashSet<ExtendedUser>();
            Lot = new HashSet<Lot>();
        }

        [Key]
        public int Id { get; set; }
        public string Contract { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ContractDate { get; set; }
        public bool? ContrectModerate { get; set; }
        public int? ContractModeratorId { get; set; }

        [ForeignKey(nameof(ContractModeratorId))]
        [InverseProperty("SellerNavigation")]
        public virtual ExtendedUser ContractModerator { get; set; }
        [InverseProperty("Seller")]
        public virtual ICollection<ExtendedUser> ExtendedUser { get; set; }
        [InverseProperty("Seller")]
        public virtual ICollection<Lot> Lot { get; set; }
    }
}
