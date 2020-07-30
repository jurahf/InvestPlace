using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvestPlaceDB
{
    public partial class Category
    {
        public Category()
        {
            InverseParent = new HashSet<Category>();
            LotCategory = new HashSet<LotCategory>();
        }

        [Key]
        public int Id { get; set; }
        [StringLength(1000)]
        public string Name { get; set; }
        public int? ParentId { get; set; }

        [ForeignKey(nameof(ParentId))]
        [InverseProperty(nameof(Category.InverseParent))]
        public virtual Category Parent { get; set; }
        [InverseProperty(nameof(Category.Parent))]
        public virtual ICollection<Category> InverseParent { get; set; }
        [InverseProperty("Category")]
        public virtual ICollection<LotCategory> LotCategory { get; set; }
    }
}
