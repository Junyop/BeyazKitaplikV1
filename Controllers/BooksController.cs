using BeyazKitaplikV1.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data;
using BeyazKitaplikV1.Services;
using BeyazKitaplikV1.DTO;
using BeyazKitaplikV1.ViewModels;
using System.Web.UI.WebControls;
using static System.Data.Entity.Infrastructure.Design.Executor;
using System.Drawing;
using PagedList;
using PagedList.Mvc;


namespace BeyazKitaplikV1.Controllers
{
    public class BooksController : Controller
    {
        List<KitaplarDTO> booksDTOs = new List<KitaplarDTO>();
        KitaplarDTO book = new KitaplarDTO();
        BeyazKitaplikEntities db = new BeyazKitaplikEntities();
        private readonly KitaplarClass _kitaplarClass;

        public BooksController()
        {
            _kitaplarClass = new KitaplarClass();
        }

        // GET: Books
        public ActionResult BookList(int? page)
        {
            int pageSize = 15; // Her sayfada kaç eleman gösterilecek
            int pageNumber = (page ?? 1);

            var booksDTOs = _kitaplarClass.GetBooks().ToPagedList(pageNumber, pageSize);

            return View(booksDTOs);
        }
        public ActionResult CreateBook()
        {
            List<ALTTURLER> subtypes = new List<ALTTURLER>();
            List<TURLER> types = new List<TURLER>();
            List<YAZARLAR> writers = new List<YAZARLAR>();
            List<YAYINLAR> publications = new List<YAYINLAR>();

            writers.Add(new YAZARLAR
            {
                YazarID = 9999,
                Yazar_Adi = "Yeni Yazar Ekle"
            });
            foreach (var item in db.YAZARLAR.ToList())
            {
                writers.Add(new YAZARLAR
                {
                    YazarID = item.YazarID,
                    Yazar_Adi = item.Yazar_Adi
                });
            }

            publications.Add(new YAYINLAR
            {
                YayinID = 9999,
                Yayin_Adi = "Yeni Yayın Ekle"
            });
            foreach (var item in db.YAYINLAR.ToList())
            {
                publications.Add(new YAYINLAR
                {
                    YayinID = item.YayinID,
                    Yayin_Adi = item.Yayin_Adi
                });
            }

            ViewBag.Types = new SelectList(db.TURLER.ToList(), "TurID", "Tur_Adi");
            ViewBag.SubTypes = new SelectList(db.ALTTURLER.ToList(), "Alt_TurID", "Alt_Tur_Adi");
            ViewBag.Publication = new SelectList(publications, "YayinID", "Yayin_Adi");
            ViewBag.Writer = new SelectList(writers, "YazarID", "Yazar_Adi");

            return View();
        }
        [HttpPost]
        public ActionResult CreateBook(CreateBooksViewModel model)
        {

            ViewBag.Types = new SelectList(db.TURLER.ToList(), "TurID", "Tur_Adi");
            ViewBag.SubTypes = new SelectList(db.ALTTURLER.ToList(), "Alt_TurID", "Alt_Tur_Adi");
            ViewBag.Publication = new SelectList(db.YAYINLAR.ToList(), "YayinID", "Yayin_Adi");
            ViewBag.Writer = new SelectList(db.YAZARLAR.ToList(), "YazarID", "Yazar_Adi");

            book.KitapAdi = model.Books.KitapAdi;
            book.Sayfa = model.Books.Sayfa;
            book.AlindigiYer = model.Books.AlindigiYer;
            book.AlinmaTarihi = model.Books.AlinmaTarihi;
            book.BaskiNo = model.Books.BaskiNo;
            book.BaskiTarihi = model.Books.BaskiTarihi;
            book.TurID = model.Types.TurID;
            book.YazarID = model.Writer.YazarID;
            book.YayinID = model.Publication.YayinID;

            List<int> Ids = new List<int>();

            if (model.SubTypesIDs != "" && model.SubTypesIDs != null)
            {
                string gelenModel = model.SubTypesIDs.TrimEnd('-');
                gelenModel = gelenModel.Replace("undefined", "");
                string[] dizi = gelenModel.Split('-');
                foreach (string s in dizi)
                {
                    Ids.Add(Convert.ToInt32(s));
                }
            }

            book.AltTurIDs = Ids;
            _kitaplarClass.CreateBook(book);

            return RedirectToAction("CreateBookSucces");
        }

        public JsonResult GetSubTypes(int id)
        {
            return Json(
                db.ALTTURLER.Where(x => x.TurID == id).ToList(), JsonRequestBehavior.AllowGet
                );
        }
        [HttpPost]
        public JsonResult AddNewAuthor(string yazarAdi)
        {
            try
            {
                var yeniYazar = _kitaplarClass.CreateWriter(yazarAdi);
                return Json(new { success = true, message = "Yazar başarıyla eklendi.", authorId = yeniYazar.YazarID, authorName = yeniYazar.Yazar_Adi });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult AddNewPublicaiton(string yayinAdi)
        {
            try
            {
                var newPublication = _kitaplarClass.CreatePublication(yayinAdi);
                return Json(new { success = true, message = "Yayin başarıyla eklendi.", publicationId = newPublication.YayinID, publicationName = newPublication.Yayin_Adi });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        public ActionResult CreateBookSucces()
        {
            return View("CreateBookSucces");
        }

        public JsonResult DeleteBook(int id)
        {
            try
            {
                _kitaplarClass.DeleteBook(id);
                return Json(new { success = true, message = "Kitap başarıyla kaldırıldı." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });

            }
        }
        public ActionResult BookInfo(int id, bool isAjax = false)
        {
            List<int> ids = new List<int>() { id };
            List<KitaplarDTO> books = _kitaplarClass.GetBooks(ids);
            KitaplarDTO book = books.FirstOrDefault();
            if (book == null)
            {
                return HttpNotFound();
            }
            if (isAjax)
            {
                book.BaskiTarihiText = book.BaskiTarihi.HasValue ? book.BaskiTarihi.Value.ToString("yyyy-MM-dd") : null;
                book.AlinmaTarihiText = book.AlinmaTarihi.HasValue ? book.AlinmaTarihi.Value.ToString("yyyy-MM-dd") : null;
                // Ajax isteği ise JSON formatında kitap bilgilerini döndür
                return Json(book, JsonRequestBehavior.AllowGet);
            }
            else
            {
                // Ajax isteği değilse normal bir görünüm döndür
                return View(book);
            }
        }
        public JsonResult UpdateBook(KitaplarDTO book)
        {
            try
            {
                _kitaplarClass.UpdateBook(book);
                return Json(new { success = true, message = "Kitap başarıyla güncellendi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });

            }
        }
        public JsonResult GetWriters()
        {
            var writers = _kitaplarClass.GetAllWriters()
                .Select(w => new
                {
                    YazarID = w.YazarID,
                    YazarAdi = w.Yazar_Adi
                }).ToList();

            return Json(writers, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPublications()
        {
            var publications = _kitaplarClass.GetAllPublications()
                .Select(p => new
                {
                    YayinID = p.YayinID,
                    YayinAdi = p.Yayin_Adi
                }).ToList();

            return Json(publications, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetGenres()
        {
            var genres = _kitaplarClass.GetAllGenres()
                .Select(g => new
                {
                    TurID = g.TurID,
                    TurAdi = g.Tur_Adi
                }).ToList();
            return Json(genres, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetSubGenres(int id)
        {
            var subGenres = _kitaplarClass.GetAllSubGenres(id)
                .Select(g => new
                {
                    TurID = g.TurID,
                    AltTurID = g.Alt_TurID,
                    AltTurAdi = g.Alt_Tur_Adi
                }).ToList();
            return Json(subGenres, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult FilterBooks(string bookName, int? yazarID, int? yayinID, int? categoryID, int[] subCategoryIDs, string FiterTable)
        {
            try
            {
                var filteredBooks = _kitaplarClass.FilterBooks(bookName, yazarID, yayinID, categoryID, subCategoryIDs).ToList();
                return PartialView("FilterTable", filteredBooks);
            }
            catch (Exception ex)
            {
                // Hata durumunda loglama yapın
                // Örneğin, hatayı bir dosyaya veya veritabanına kaydedebilirsiniz
                System.IO.File.WriteAllText("C:\\path\\to\\your\\log\\file.txt", ex.ToString());
                // Hata mesajını döndürün
                return PartialView("ErrorPartialView", new ErrorViewModel { Message = ex.Message });
            }
        }

        public PartialViewResult SearchBooks(string query)
        {
            var filteredBooks = _kitaplarClass.SearchBooks(query).ToList();

            return PartialView("BookSearch", filteredBooks); // Filtrelenmiş kitapları döndüren partial view
        }
        public class ErrorViewModel
        {
            public string Message { get; set; }
        }
    }
}