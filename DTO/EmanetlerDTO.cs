using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeyazKitaplikV1.DTO
{
    public class EmanetlerDTO
    {
        public int EmanetID { get; set; }
        public Nullable<int> KitapID { get; set; }
        public string KimeVerildi { get; set; }
        public Nullable<System.DateTime> VerilmeTarihi { get; set; }
        public Nullable<System.DateTime> GeriAlinmaTarihi { get; set; }
        public string KitapAdi { get; set; }
        public string YazarAdi { get; set; }
    }
}