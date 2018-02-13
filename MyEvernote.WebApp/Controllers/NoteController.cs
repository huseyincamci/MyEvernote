using MyEvernote.BusinessLayer;
using MyEvernote.Entities;
using MyEvernote.WebApp.Filters;
using MyEvernote.WebApp.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MyEvernote.WebApp.Controllers
{
    [Exc]
    public class NoteController : Controller
    {
        private NoteManager _noteManager = new NoteManager();
        private CategoryManager _categoryManager = new CategoryManager();
        private LikedManager _likedManager = new LikedManager();

        [Auth]
        public ActionResult Index()
        {
            var notes = _noteManager.ListQueryable()
                .Include(n => n.Category)
                .Include(n => n.Owner)
                .Where(n => n.Owner.Id == CurrentSession.User.Id)
                .OrderByDescending(x => x.ModifiedOn)
                .ToList();

            return View(notes.ToList());
        }

        [Auth]
        public ActionResult MyLikedNotes()
        {
            var notes = _likedManager.ListQueryable()
                .Include(x => x.LikedUser)
                .Include(x => x.Note)
                .Where(x => x.LikedUser.Id == CurrentSession.User.Id)
                .Select(x => x.Note)
                .Include(x => x.Category)
                .Include(x => x.Owner)
                .OrderByDescending(x => x.ModifiedOn);

            return View(notes.ToList());
        }

        [Auth]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = _noteManager.Find(x => x.Id == id && x.Owner.Id == CurrentSession.User.Id);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }

        [Auth]
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(_categoryManager.List(), "Id", "Title");
            return View();
        }

        [Auth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Note note)
        {
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedUsername");

            if (ModelState.IsValid)
            {
                note.Owner = CurrentSession.User;
                _noteManager.Insert(note);
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(_categoryManager.List(), "Id", "Title", note.CategoryId);
            return View(note);
        }

        [Auth]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = _noteManager.Find(x => x.Id == id && x.Owner.Id == CurrentSession.User.Id);
            if (note == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(_categoryManager.List(), "Id", "Title", note.CategoryId);
            return View(note);
        }

        [Auth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Note note)
        {
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedUsername");

            if (ModelState.IsValid)
            {
                Note dbNote = _noteManager.Find(x => x.Id == note.Id && x.Owner.Id == CurrentSession.User.Id);
                dbNote.IsDraft = note.IsDraft;
                dbNote.CategoryId = note.CategoryId;
                dbNote.Text = note.Text;
                dbNote.Title = note.Title;

                _noteManager.Update(dbNote);
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(_categoryManager.List(), "Id", "Title", note.CategoryId);
            return View(note);
        }

        [Auth]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = _noteManager.Find(x => x.Id == id && x.Owner.Id == CurrentSession.User.Id);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }

        [Auth]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Note note = _noteManager.Find(x => x.Id == id);
            _noteManager.Delete(note);
            return RedirectToAction("Index");
        }

        [Auth]
        [HttpPost]
        public ActionResult GetLiked(int[] ids)
        {
            List<int> likedNoteIds = _likedManager.List(x => x.LikedUser.Id == CurrentSession.User.Id && ids.Contains(x.Note.Id))
                .Select(x => x.Note.Id).ToList();

            return Json(new { result = likedNoteIds });
        }

        [Auth]
        [HttpPost]
        public ActionResult SetLikeState(int noteId, bool liked)
        {
            int res = 0;
            Liked like = _likedManager.Find(x => x.Note.Id == noteId && x.LikedUser.Id == CurrentSession.User.Id);
            Note note = _noteManager.Find(x => x.Id == noteId);

            if (like != null && liked == false)
            {
                res = _likedManager.Delete(like);
            }
            else if (like == null && liked == true)
            {
                res = _likedManager.Insert(new Liked()
                {
                    LikedUser = CurrentSession.User,
                    Note = note
                });
            }

            if (res > 0)
            {
                if (liked)
                {
                    note.LikeCount++;
                }
                else
                {
                    note.LikeCount--;
                }
                res = _noteManager.Update(note);
                return Json(new
                {
                    hasError = false,
                    errorMessage = string.Empty,
                    result = note.LikeCount
                });
            }

            return Json(new
            {
                hasError = true,
                errorMessage = "Beğenme işlemi gerçekleştirilemedi.",
                result = note.LikeCount
            });
        }

        public ActionResult GetNoteText(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = _noteManager.Find(x => x.Id == id);
            if (note == null)
            {
                return HttpNotFound();
            }
            return PartialView("_PartialNoteText", note);
        }
    }
}
