using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvestPlaceDB
{
    public partial class Basket
    {
        public Basket()
        {
            ExtendedUser = new HashSet<ExtendedUser>();
            Pazzle = new HashSet<Pazzle>();
        }

        [Key]
        public int Id { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastOperationDate { get; set; }

        [InverseProperty("Basket")]
        public virtual ICollection<ExtendedUser> ExtendedUser { get; set; }
        [InverseProperty("Basket")]
        public virtual ICollection<Pazzle> Pazzle { get; set; }
    }
}
