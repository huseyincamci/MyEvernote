using MyEvernote.BusinessLayer;
using MyEvernote.BusinessLayer.Results;
using MyEvernote.Entities;
using MyEvernote.Entities.Messages;
using MyEvernote.Entities.ViewModels;
using MyEvernote.WebApp.Filters;
using MyEvernote.WebApp.Models;
using MyEvernote.WebApp.ViewModels;
using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MyEvernote.WebApp.Controllers
{
    [Exc]
    public class HomeController : Controller
    {

        private EvernoteUserManager _userManager = new EvernoteUserManager();
        private NoteManager _noteManager = new NoteManager();
        private CategoryManager _categoryManager = new CategoryManager();

        public ActionResult Index()
        {
            return View(_noteManager.ListQueryable().Where(x => x.IsDraft == false).OrderByDescending(x => x.ModifiedOn).ToList());
        }

        public ActionResult ByCategory(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var category = _categoryManager.Find(x => x.Id == id.Value);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View("Index", category.Notes
                .Where(x => x.IsDraft == false && x.CategoryId == id)
                .OrderByDescending(x => x.ModifiedOn)
                .ToList());
        }

        public ActionResult MostLiked()
        {
            NoteManager noteManager = new NoteManager();
            return View("Index", noteManager.ListQueryable().Where(x => x.IsDraft == false).OrderByDescending(x => x.LikeCount).ToList());
        }

        public ActionResult About()
        {
            return View();
        }

        [Auth]
        public ActionResult ShowProfile()
        {
            BusinessLayerResult<EvernoteUser> res = _userManager.GetUserById(CurrentSession.User.Id);

            if (res.Errors.Count > 0)
            {
                ErrorViewModel errorNotify = new ErrorViewModel()
                {
                    Title = "Hata Oluştu!",
                    Items = res.Errors
                };

                return View("Error", errorNotify);
            }
            return View(res.Result);
        }

        [Auth]
        public ActionResult EditProfile()
        {
            BusinessLayerResult<EvernoteUser> res = _userManager.GetUserById(CurrentSession.User.Id);

            if (res.Errors.Count > 0)
            {
                ErrorViewModel errorNotify = new ErrorViewModel()
                {
                    Title = "Hata Oluştu!",
                    Items = res.Errors
                };

                return View("Error", errorNotify);
            }
            return View(res.Result);
        }

        [Auth]
        [HttpPost]
        public ActionResult EditProfile(EvernoteUser user, HttpPostedFileBase ProfileImage)
        {
            ModelState.Remove("ModifiedUsername");
            if (ModelState.IsValid)
            {
                if (ProfileImage != null &&
               (ProfileImage.ContentType == "image/jpg" ||
               ProfileImage.ContentType == "image/jpeg" ||
               ProfileImage.ContentType == "image/png"))
                {
                    string fileName = $"user_{user.Id}.{ProfileImage.ContentType.Split('/')[1]}";
                    ProfileImage.SaveAs(Server.MapPath($"~/Images/{fileName}"));
                    user.ProfileImageFileName = fileName;
                }

                BusinessLayerResult<EvernoteUser> res = _userManager.UpdateProfile(user);

                if (res.Errors.Count > 0)
                {
                    ErrorViewModel errorNotify = new ErrorViewModel()
                    {
                        Title = "Profil Güncellenemedi.",
                        Items = res.Errors,
                        RedirectingUrl = "/home/editprofile"
                    };

                    return View("Error", errorNotify);
                }

                CurrentSession.Set<EvernoteUser>("login", res.Result);
                return RedirectToAction("ShowProfile");
            }
            return View(user);
        }

        [Auth]
        public ActionResult DeleteProfile()
        {
            BusinessLayerResult<EvernoteUser> res = _userManager.RemoveUserById(CurrentSession.User.Id);

            if (res.Errors.Count > 0)
            {
                ErrorViewModel errorNotify = new ErrorViewModel()
                {
                    Title = "Profil Silinemedi.",
                    Items = res.Errors,
                    RedirectingUrl = "/home/showprofile"
                };

                return View("Error", errorNotify);
            }

            Session.Clear();
            return RedirectToAction("Index");
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                BusinessLayerResult<EvernoteUser> res = _userManager.LoginUser(model);

                if (res.Errors.Count > 0)
                {
                    if (res.Errors.Find(x => x.Code == ErrorMessagesCode.UserIsNotActive) != null)
                    {
                        ViewBag.SetLink = "http://localhost:62881/home/activate/1234-4567-7890";
                    }
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));
                    return View(model);
                }

                CurrentSession.Set<EvernoteUser>("login", res.Result);
                return RedirectToAction("Index");
            }

            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                //if (model.Username == "aaa")
                //{
                //    ModelState.AddModelError("", "Kullanıcı adı kullanılıyor.");
                //}

                //if (model.Email == "aaa@aa.com")
                //{
                //    ModelState.AddModelError("", "E-posta adresi kullanılıyor.");
                //}

                //foreach (var item in ModelState)
                //{
                //    if (item.Value.Errors.Count > 0)
                //    {
                //        return View(model);
                //    }
                //}

                BusinessLayerResult<EvernoteUser> res = _userManager.RegisterUser(model);

                if (res.Errors.Count > 0)
                {
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));
                    return View(model);
                }

                OkViewModel notify = new OkViewModel()
                {
                    Title = "Kayıt Başarılı",
                    RedirectingUrl = "/home/login"
                };
                notify.Items.Add("Lütfen e-posta adresinize gönderdiğimiz aktivasyon link'ine tıklayarak hesabınızı aktif ediniz.");

                return View("Ok", notify);
            }
            return View(model);
        }

        public ActionResult UserActivate(Guid id)
        {
            BusinessLayerResult<EvernoteUser> res = _userManager.ActivateUser(id);

            if (res.Errors.Count > 0)
            {
                ErrorViewModel errorNotify = new ErrorViewModel()
                {
                    Title = "Geçersiz İşlem",
                    Items = res.Errors
                };

                return View("Error", errorNotify);
            }

            OkViewModel okNotify = new OkViewModel()
            {
                Title = "Hesap Aktifleştirildi",
                RedirectingUrl = "/home/login"
            };
            okNotify.Items.Add("Hesabınız aktifleştirildi. Artık not paylaşabilir ve beğenme yapabilirsiniz.");

            return View("Ok", okNotify);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }

        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult HasError()
        {
            return View();
        }
    }
}