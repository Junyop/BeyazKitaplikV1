//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class KITAPLAR
    {
        public int KitapID { get; set; }
        public string Kitap_Adi { get; set; }
        public Nullable<int> YazarID { get; set; }
        public Nullable<int> YayinID { get; set; }
        public Nullable<int> Baski_no { get; set; }
        public Nullable<System.DateTime> Baski_Tarihi { get; set; }
        public Nullable<int> Sayfa { get; set; }
        public Nullable<System.DateTime> Alinma_Tarihi { get; set; }
        public string Alindigi_Yer { get; set; }
    }
}
