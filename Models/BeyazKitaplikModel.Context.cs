﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BeyazKitaplikV1.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class BeyazKitaplikEntities : DbContext
    {
        public BeyazKitaplikEntities()
            : base("name=BeyazKitaplikEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ALTTURLER> ALTTURLER { get; set; }
        public virtual DbSet<EMANETLER> EMANETLER { get; set; }
        public virtual DbSet<KITAP_TUR_ILISKISI> KITAP_TUR_ILISKISI { get; set; }
        public virtual DbSet<KITAPLAR> KITAPLAR { get; set; }
        public virtual DbSet<OKUNAN_KITAPLAR> OKUNAN_KITAPLAR { get; set; }
        public virtual DbSet<YAYINLAR> YAYINLAR { get; set; }
        public virtual DbSet<YAZARLAR> YAZARLAR { get; set; }
        public virtual DbSet<TURLER> TURLER { get; set; }
    }
}
