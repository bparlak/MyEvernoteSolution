using MyEvernote.BusinessLayer;
using MyEvernote.BusinessLayer.Results;
using MyEvernote.Entities;
using MyEvernote.Entities.Messages;
using MyEvernote.Entities.ValueObjects;
using MyEvernote.Web.Models;
using MyEvernote.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MyEvernote.Web.Controllers
{
    public class HomeController : Controller
    {
        private NoteManager noteManager = new NoteManager();
        private CategoryManager categoryManager = new CategoryManager();
        private EvernoteUserManager evernoteUserManager = new EvernoteUserManager();
        // GET: Home
        public ActionResult Index()
        {
            //List<Note> notes = nm.GetAllNoteQueryable().OrderByDescending(x => x.ModifiedOn).ToList();
            return View(noteManager.ListQueryable().OrderByDescending(x => x.ModifiedOn).ToList());
        }
        public ActionResult ByCategory(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category cat = categoryManager.Find(x=> x.Id==id.Value);
            if (cat == null)
            {
                return HttpNotFound();
            }
            List<Note> noteList = cat.Notes.OrderByDescending(x => x.ModifiedOn).ToList();
            if (noteList == null)
            {
                noteList.Add(new Note());
            }
            return View("Index", noteList);
        }
        public ActionResult MostLiked()
        {
            return View("Index", noteManager.ListQueryable().OrderByDescending(x => x.LikeCount).ToList());
        }
        public ActionResult About()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)//true dönerse attributes üzerinde tanımladığımız kısıtları,kontrolleri geçmiş demektir.
            {
                BusinessLayerResult<EvernoteUser> res = evernoteUserManager.RegisterUser(model);

                if (res.Errors.Count > 0)
                {
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));//hata varsa hepsini dönüp tektek modelstate e ekleyecek.
                    return View(model);
                }
                OkViewModel okObj = new OkViewModel()
                {
                    Title = "Kayıt Başarılı",
                    RedirectingUrl = "/Home/Login"
                };
                okObj.Items.Add("Lütfen e-posta adresinize gönderdiğimiz aktivasyon Link'ine tıklayarak hesabınızı aktive ediniz.Hesabınızı aktive etmeden not ekleyemez ve beğenme işlemi gerçekleştiremezsiniz.");
                return View("Ok", okObj);
                //EvernoteUser user = null;
                //try
                //{
                //    user=eum.RegisterUser(model);
                //}
                //catch (Exception ex)
                //{
                //    ModelState.AddModelError("", ex.Message);
                //}


                //bool hasError = false;
                //if (model.Username == "aaa")
                //{
                //    ModelState.AddModelError("", "Kullanıcı adı kullanılıyor.");
                //    hasError = true;
                //}
                //if (model.EMail == "aaa@aa.com")
                //{
                //    ModelState.AddModelError("", "E-posta adresi kullanılıyor.");
                //    hasError = true;
                //}
                //if (hasError == true)
                //{
                //    return View(model);
                //}

                //foreach (var item in ModelState)
                //{
                //    if (item.Value.Errors.Count>0)
                //    {
                //        //budurumda yukarıdaki 2 ifden birisinde hata yakalanmıs demektir model viewe return edilir
                //        return View(model);
                //    }
                //}
                //if (user==null)
                //{
                //    return View(model);
                //}
            }
            return View();
        }

        public ActionResult UserActivate(Guid? id)
        {
            BusinessLayerResult<EvernoteUser> res = evernoteUserManager.ActivateUser(id.Value);

            if (res.Errors.Count > 0)
            {
                ErrorViewModel errObj = new ErrorViewModel()
                {
                    Title = "Geçersiz İşlem",
                    Items = res.Errors
                };
                return View("Error", errObj);
            }
            OkViewModel okObj = new OkViewModel()
            {
                Title="Hesap Aktifleştirildi.",
                RedirectingUrl="/Home/Login"
            };
            okObj.Items.Add("Hesabınız aktifleştirilmiştir.Artık not paylaşabilir ve  diğer kullanıcıların notlarını beğenip yorum yapabilirsiniz.");
            return View("Ok",okObj);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)//loginviewmodelde username ve password geliyor.
        {
            if (ModelState.IsValid)
            {
                BusinessLayerResult<EvernoteUser> res = evernoteUserManager.LoginUser(model);//dönüş tipi bussineslayerresult ayarlandı
                if (res.Errors.Count > 0)
                {
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));//tüm hatalar ekrana gönderilmek için modelstate e ekleniyor
                    if (res.Errors.Find(x => x.Code == ErrorMessageCode.UserIsNotActive) != null)//eğer aktif olmayan bir kullanıcı ise aktif linki yönlendir
                    {
                        ViewBag.SetLink = "http://Home/Active/123-4152-123";
                    }
                    return View(model);
                }
                CurrentSession.Set<EvernoteUser>("login", res.Result);
                return RedirectToAction("Index", "Home");
            }
            return View(model);//Modelstate isvalid değilse kısıtlamalara uyulmamıstır. yani hata vardır
        }

        public ActionResult ShowProfile()
        {
            BusinessLayerResult<EvernoteUser> res = evernoteUserManager.GetUserById(CurrentSession.User.Id);

            if (res.Errors.Count > 0)
            {
                ErrorViewModel errObj = new ErrorViewModel()
                {
                    Title = "Hata Oluştu",
                    Items = res.Errors
                };
                return View("Error", errObj);
            }

            return View(res.Result);
        }

        public ActionResult TestNotify()
        {
            ErrorViewModel model = new ErrorViewModel()
            {
                Header = "Yönlendirme..",
                Title = "Ok Test",
                RedirectingTimeout = 10000,
                Items = new List<ErrorMessageObj>()
                {
                    new ErrorMessageObj(){Message="bu bir denemedir1"},
                    new ErrorMessageObj(){Message="bu bir denemedir2"}
                }
            };
            return View("Error", model);
        }

        public ActionResult EditProfile()
        {
            BusinessLayerResult<EvernoteUser> res = evernoteUserManager.GetUserById(CurrentSession.User.Id);

            if (res.Errors.Count > 0)
            {
                ErrorViewModel errObj = new ErrorViewModel()
                {
                    Title = "Hata Oluştu",
                    Items = res.Errors
                };
                return View("Error", errObj);
            }

            return View(res.Result);
        }
        [HttpPost]
        public ActionResult EditProfile(EvernoteUser model,HttpPostedFileBase ProfileImage)
        {
            ModelState.Remove("ModifiedUsername");
            if (ModelState.IsValid)
            {
                if (ProfileImage != null &&
               (ProfileImage.ContentType == "image/jpeg" ||
               ProfileImage.ContentType == "image/jpg" ||
               ProfileImage.ContentType == "image/png")
               )
                {
                    string fileName = $"user_{ model.Id}.{ProfileImage.ContentType.Split('/')[1]}";
                    ProfileImage.SaveAs(Server.MapPath($"~/images/{fileName}"));
                    model.ProfileImageFilename = fileName;
                }

                evernoteUserManager = new EvernoteUserManager();
                BusinessLayerResult<EvernoteUser> res = evernoteUserManager.UpdateProfile(model);

                if (res.Errors.Count > 0)
                {
                    ErrorViewModel errObj = new ErrorViewModel()
                    {
                        Title = "Hata Oluştu",
                        Items = res.Errors,
                        RedirectingUrl = "/Home/EditProfile"
                    };
                    return View("Error", errObj);
                }
                CurrentSession.Set<EvernoteUser>("login", res.Result);

                return RedirectToAction("ShowProfile");
            }
            return View(model);
        }

        public ActionResult DeleteProfile()
        {
            BusinessLayerResult<EvernoteUser> res = evernoteUserManager.RemoveUserById(CurrentSession.User.Id);

            if (res.Errors.Count > 0)
            {
                ErrorViewModel errObj = new ErrorViewModel()
                {
                    Items = res.Errors,
                    Title = "Profil Silinemedi",
                    RedirectingUrl = "/Home/ShowProfile"
                };
                return View("Error", errObj);
            }
            Session.Clear();
            return RedirectToAction("Index");
        }



        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }
    }
}