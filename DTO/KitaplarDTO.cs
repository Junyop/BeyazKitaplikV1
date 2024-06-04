using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeyazKitaplikV1.DTO
{
    public class KitaplarDTO
    {
        public int KitapID { get; set; }
        public string KitapAdi { get; set; }
        public Nullable<int> YazarID { get; set; }
        public Nullable<int> YayinID { get; set; }
        public Nullable<int> BaskiNo { get; set; }
        public Nullable<System.DateTime> BaskiTarihi { get; set; }
        public string BaskiTarihiText { get; set; }
        public Nullable<int> Sayfa { get; set; }
        public Nullable<System.DateTime> AlinmaTarihi { get; set; }
        public string AlinmaTarihiText { get; set; }
        public string AlindigiYer { get; set; }
        public List<int> AltTurIDs { get; set; }
        public int TurID { get; set; }
        public List<string> AltTurAdis {  get; set; }
        public string TurAdi { get; set; }
        public string YazarAdi { get; set; }
        public string YayinAdi { get; set; }
        public string SubTypes { get; set; }
        public bool OkunduMu { get; set; }
        public bool EmanetteMi { get; set; }

    }
}
