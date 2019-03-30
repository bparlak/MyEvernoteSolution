using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyEvernote.BusinessLayer;
using MyEvernote.Entities;

namespace MyEvernote.Web.Controllers
{
    public class EvernoteUserController : Controller
    {
        private EvernoteUserManager evernoteUserManager = new EvernoteUserManager();

        // GET: EvernoteUser
        public ActionResult Index()
        {
            return View(evernoteUserManager.List().ToList());
        }

        // GET: EvernoteUser/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvernoteUser evernoteUser = evernoteUserManager.Find(x => x.Id == id.Value);
            if (evernoteUser == null)
            {
                return HttpNotFound();
            }
            return View(evernoteUser);
        }

        // GET: EvernoteUser/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EvernoteUser evernoteUser)
        {
            if (ModelState.IsValid)
            {
                //TODO: Düzeltilecek
                //evernoteUserManager.Insert(evernoteUser);
                evernoteUserManager.Save();
                return RedirectToAction("Index");
            }

            return View(evernoteUser);
        }

        // GET: EvernoteUser/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvernoteUser evernoteUser = evernoteUserManager.Find(x => x.Id == id.Value);
            if (evernoteUser == null)
            {
                return HttpNotFound();
            }
            return View(evernoteUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EvernoteUser evernoteUser)
        {
            if (ModelState.IsValid)
            {
                //TODO düzenlenecek.
                //evernoteUserManager.Update(evernoteUser);
                evernoteUserManager.Save();
                return RedirectToAction("Index");
            }
            return View(evernoteUser);
        }

        // GET: EvernoteUser/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvernoteUser evernoteUser = evernoteUserManager.Find(x => x.Id == id.Value);
            if (evernoteUser == null)
            {
                return HttpNotFound();
            }
            return View(evernoteUser);
        }
        

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EvernoteUser evernoteUser = evernoteUserManager.Find(x => x.Id == id);
            evernoteUserManager.Delete(evernoteUser);
            evernoteUserManager.Save();
            return RedirectToAction("Index");
        }
    }
}
