using MyEvernote.BusinessLayer;
using MyEvernote.Entities;
using MyEvernote.Web.Filter;
using MyEvernote.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MyEvernote.Web.Controllers
{
    [Exc]
    public class CommentController : Controller
    {
        private NoteManager noteManager = new NoteManager();
        private CommentManager commentManager = new CommentManager();
        private LikedManager likedManager = new LikedManager();
        public ActionResult ShowNoteComments(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Note note = noteManager.Find(x => x.Id == id);
            Note note = noteManager.ListQueryable().Include("Comments").FirstOrDefault(x => x.Id == id);
            if (note == null)
            {
                return HttpNotFound();
            }
            return PartialView("_PartialComments", note.Comments);
        }

        [Auth]
        [HttpPost]
        public ActionResult Edit(int? id, string text)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Note note = noteManager.Find(x => x.Id == id);
            Comment comment = commentManager.Find(x => x.Id == id);
            if (comment == null)
            {
                return new HttpNotFoundResult();
            }
            comment.Text = text;
            int res = commentManager.Update(comment);
            if (res > 0)
            {
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }

        [Auth]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Note note = noteManager.Find(x => x.Id == id);
            Comment comment = commentManager.Find(x => x.Id == id);
            if (comment == null)
            {
                return new HttpNotFoundResult();
            }
            int res = commentManager.Delete(comment);
            if (res > 0)
            {
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }

        [Auth]
        [HttpPost]
        public ActionResult Create(Comment comment, int? noteid)
        {
            ModelState.Remove("CreateOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsername");
            if (ModelState.IsValid)
            {
                if (noteid == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                //Note note = noteManager.Find(x => x.Id == id);
                Note note = noteManager.Find(x => x.Id == noteid);
                if (comment == null)
                {
                    return new HttpNotFoundResult();
                }
                comment.Note = note;
                comment.Owner = CurrentSession.User;
                int res = commentManager.Insert(comment);
                if (res > 0)
                {
                    return Json(new { result = true }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }

    

    }
}