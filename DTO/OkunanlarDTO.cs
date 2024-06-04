using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeyazKitaplikV1.DTO
{
    public class OkunanlarDTO
    {
        public int OkumaID { get; set; }
        public Nullable<int> KitapID { get; set; }
        public Nullable<int> DegerlendirmePuani { get; set; }
        public string Yorum { get; set; }
        public Nullable<System.DateTime> BaslamaTarihi { get; set; }
        public Nullable<System.DateTime> BitirmeTarihi { get; set; }
        public string KitapAdi {  get; set; }
        public string YazarAdi { get; set; }

    }
}