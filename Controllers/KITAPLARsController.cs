using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BeyazKitaplikV1.Models;

namespace BeyazKitaplikV1.Controllers
{
    public class KITAPLARsController : Controller
    {
        private BeyazKitaplikEntities db = new BeyazKitaplikEntities();

        // GET: KITAPLARs
        public ActionResult Index()
        {
            return View(db.KITAPLAR.ToList());
        }

        // GET: KITAPLARs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KITAPLAR kITAPLAR = db.KITAPLAR.Find(id);
            if (kITAPLAR == null)
            {
                return HttpNotFound();
            }
            return View(kITAPLAR);
        }

        // GET: KITAPLARs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: KITAPLARs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "KitapID,Kitap_Adi,YazarID,YayinID,Baski_no,Baski_Tarihi,Sayfa,Alinma_Tarihi,Alindigi_Yer")] KITAPLAR kITAPLAR)
        {
            if (ModelState.IsValid)
            {
                db.KITAPLAR.Add(kITAPLAR);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(kITAPLAR);
        }

        // GET: KITAPLARs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KITAPLAR kITAPLAR = db.KITAPLAR.Find(id);
            if (kITAPLAR == null)
            {
                return HttpNotFound();
            }
            return View(kITAPLAR);
        }

        // POST: KITAPLARs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "KitapID,Kitap_Adi,YazarID,YayinID,Baski_no,Baski_Tarihi,Sayfa,Alinma_Tarihi,Alindigi_Yer")] KITAPLAR kITAPLAR)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kITAPLAR).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(kITAPLAR);
        }

        // GET: KITAPLARs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KITAPLAR kITAPLAR = db.KITAPLAR.Find(id);
            if (kITAPLAR == null)
            {
                return HttpNotFound();
            }
            return View(kITAPLAR);
        }

        // POST: KITAPLARs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            KITAPLAR kITAPLAR = db.KITAPLAR.Find(id);
            db.KITAPLAR.Remove(kITAPLAR);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
