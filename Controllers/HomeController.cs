using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BeyazKitaplikV1.DTO;
using BeyazKitaplikV1.Models;
using BeyazKitaplikV1.Services;


namespace BeyazKitaplikV1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            /*KitaplarClass kitapClassQbj = new KitaplarClass();
            OkunanlarClass okunanKitapClassObj = new OkunanlarClass();
            EmanetlerClass emanetClassObj = new EmanetlerClass();

            List<OKUNAN_KITAPLAR> okunanlarList = new List<OKUNAN_KITAPLAR>();
            List<KITAPLAR> kitaplarList = new List<KITAPLAR>();
            List<EMANETLER> emanetlerList = new List<EMANETLER>();

            // okunanlarList = okunanKitapClassObj.GetLastFive();
            //okunanlarList =okunanKitapClassObj.GetUnfinishedFive();

            DateTime d1 = new DateTime(01 / 01 / 2018);
             KitaplarDTO kitaplarDTO = new KitaplarDTO()
              {
                  KitapID=17,
                  AltTurIDs = new List<int> { 4, 2,1},
                  TurID = 5,
                  KitapAdi ="DEneme yeni update"
              };
              kitapClassQbj.updateBook(kitaplarDTO);

            List<KitaplarDTO> gelenKitaplar = kitapClassQbj.getKitaplar();*/


            return View();
            
        }



        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}