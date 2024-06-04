using BeyazKitaplikV1.DTO;
using BeyazKitaplikV1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Web;
using System.Web.Mvc;

namespace BeyazKitaplikV1.ViewModels
{
    public class CreateBooksViewModel
    {
        public KitaplarDTO Books { get; set; }
        public TURLER Types { get; set; }
        public ALTTURLER SubTypes { get; set; }
        public YAYINLAR Publication { get; set; }
        public YAZARLAR Writer { get; set; }
        public string SubTypesIDs { get; set; }
    }
}