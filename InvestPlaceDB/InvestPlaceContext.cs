using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace InvestPlaceDB
{

    public class InvestPlaceContext : IdentityDbContext<ExtendedUser, ExtendedRole, int>
    {
        public virtual DbSet<Basket> Basket { get; set; }
        public virtual DbSet<Cash> Cash { get; set; }
        public virtual DbSet<CashOperation> CashOperation { get; set; }
        public virtual DbSet<QueryForOperation> QueryForOperation { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        //public virtual DbSet<ExtendedUser> ExtendedUser { get; set; }
        public virtual DbSet<Lot> Lot { get; set; }
        public virtual DbSet<LotCategory> LotCategory { get; set; }
        public virtual DbSet<Pazzle> Pazzle { get; set; }

        public InvestPlaceContext(DbContextOptions options)
            : base(options)
        {
            //Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

    }
}
