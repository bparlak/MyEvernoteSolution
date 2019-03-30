using MyEvernote.BusinessLayer.Abstract;
using MyEvernote.BusinessLayer.Results;
using MyEvernote.Common.Helper;
using MyEvernote.DataAccessLayer.EntityFramework;
using MyEvernote.Entities;
using MyEvernote.Entities.Messages;
using MyEvernote.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.BusinessLayer
{
    public class EvernoteUserManager : ManagerBase<EvernoteUser>
    {
        public BusinessLayerResult<EvernoteUser> RegisterUser(RegisterViewModel data)
        {
            EvernoteUser user = Find(x => x.Username == data.Username || x.Email == data.EMail);
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();//model ile birlikte birden fazla hata dönebilmek için olusturduk
            if (user != null)
            {
                if (user.Username == data.Username)//aynı isimde email adresi yada kullanıcı adı varsa dönen modele eklenecek
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExist, "Kullanıcı adı kayıtlı.");
                }
                if (user.Email == data.EMail)
                {
                    res.AddError(ErrorMessageCode.EmailAlreadyExist, "E-Posta adresi kayıtlı.");
                }
            }
            else
            {//insert işlemi
                int dbResult = base.Insert(new EvernoteUser()
                {
                    Name = data.Name,
                    Surname = data.Surname,
                    Username = data.Username,
                    Email = data.EMail,
                    Password = data.Password,
                    ProfileImageFilename = "profilepic.png",
                    ActivateGuid = Guid.NewGuid(),
                    IsActive = false,
                    IsAdmin = false,
                });
                if (dbResult > 0)
                {
                    res.Result = Find(x => x.Username == data.Username && x.Email == data.EMail);
                    string siteUri = ConfigHelper.Get<string>("SiteRootUri");
                    string activateUri = $"{siteUri}/Home/UserActivate/{res.Result.ActivateGuid}";
                    string body = $"Merhaba { res.Result.Username}, <br><br>Hesabınızı aktifleştirmek için <a href='{activateUri}' target='_blank'>tıklayınız</a>.";
                    MailHelper.SendMail(body, res.Result.Email, "MyEvernote Hesap Aktifleştirme");
                }
            }
            return res;
        }
        public BusinessLayerResult<EvernoteUser> LoginUser(LoginViewModel data)
        {
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();
            res.Result = Find(x => x.Username == data.Username && x.Password == data.Password);
            if (res.Result != null)
            {
                if (!res.Result.IsActive)
                {
                    res.AddError(ErrorMessageCode.UserIsNotActive, "Kullanıcı aktifleştirilmemiştir.");
                    res.AddError(ErrorMessageCode.CheckYourEmail, "Lütfen e-posta adresinizi kontrol ediniz.");
                }

            }
            else
            {
                res.AddError(ErrorMessageCode.UsernameOrPassWrong, "Kullanıcı adı yada şifre uyuşmuyor.");
            }
            return res;
        }
        public BusinessLayerResult<EvernoteUser> ActivateUser(Guid activateId)
        {
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();
            res.Result = Find(x => x.ActivateGuid == activateId);
            if (res.Result != null)
            {
                if (res.Result.IsActive)
                {
                    res.AddError(ErrorMessageCode.UserAlreadyActive, "Kullanıcı zaten aktif edilmiştir.");
                    return res;
                }
                res.Result.IsActive = true;
                Update(res.Result);
            }
            else
            {
                res.AddError(ErrorMessageCode.ActivateIdDoesNotExist, "Böyle bir kullanıcı bulunamadı.");
                return res;
            }
            return res;
        }

        public BusinessLayerResult<EvernoteUser> GetUserById(int id)
        {
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();
            res.Result = Find(x => x.Id == id);
            if (res.Result == null)
            {
                res.AddError(ErrorMessageCode.UserNotFound, "Kullanıcı bulunamadı.");
            }
            return res;
        }

        public BusinessLayerResult<EvernoteUser> UpdateProfile(EvernoteUser model)
        {
            //email ve username ayrı karsılastırılmalı cunku maili değiştirmeyip username i var olan bir tane seçtiğimde ilk e-maili bulup geleceği için başkasının usernameine kayıt yapıyor.
            EvernoteUser db_user = Find(x => x.Id != model.Id && (x.Username == model.Username || x.Email == model.Email));
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();
            //baskabirisinin kullanıcı adını veya emailini yazmış mı kontrolü
            if (db_user != null && db_user.Id != model.Id)
            {
                if (db_user.Username == model.Username)//aynı isimde email adresi yada kullanıcı adı varsa dönen modele eklenecek
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExist, "Kullanıcı adı kayıtlı.");
                }
                if (db_user.Email == model.Email)
                {
                    res.AddError(ErrorMessageCode.EmailAlreadyExist, "E-Posta adresi kayıtlı.");
                }
                return res;
            }
            res.Result = Find(x => x.Id == model.Id);
            res.Result.Email = model.Email;
            res.Result.Name = model.Name;
            res.Result.Surname = model.Surname;
            res.Result.Password = model.Password;
            res.Result.Username = model.Username;
            if (string.IsNullOrEmpty(model.ProfileImageFilename) == false)
            {
                res.Result.ProfileImageFilename = model.ProfileImageFilename;
            }

            if (base.Update(res.Result) == 0)
            {
                res.AddError(ErrorMessageCode.ProfileCouldNotUpdated, "Profil güncellenemedi.");
            }
            return res;
        }

        public BusinessLayerResult<EvernoteUser> RemoveUserById(int id)
        {
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();
            EvernoteUser user = res.Result = Find(x => x.Id == id);
            if (user != null)
            {
                if (Delete(user) == 0)
                {
                    res.AddError(ErrorMessageCode.UserCouldNotRemove, "Kullanıcı silinemedi.");
                    return res;
                }
            }
            else
            {
                res.AddError(ErrorMessageCode.UserCouldNotFind, "Kullanıcı bulunamadı.");
            }
            return res;
        }

        public new BusinessLayerResult<EvernoteUser> Insert(EvernoteUser data)
        {
            EvernoteUser user = Find(x => x.Username == data.Username || x.Email == data.Email);
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();//model ile birlikte birden fazla hata dönebilmek için olusturduk
            res.Result = data;
            if (user != null)
            {
                if (user.Username == data.Username)//aynı isimde email adresi yada kullanıcı adı varsa dönen modele eklenecek
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExist, "Kullanıcı adı kayıtlı.");
                }
                if (user.Email == data.Email)
                {
                    res.AddError(ErrorMessageCode.EmailAlreadyExist, "E-Posta adresi kayıtlı.");
                }
            }
            else
            {

                res.Result.ProfileImageFilename = "profilepic.png";
                res.Result.ActivateGuid = Guid.NewGuid();

                int dbResult = base.Insert(res.Result);
                if (dbResult == 0)
                {
                    res.AddError(ErrorMessageCode.UserCouldNotInserted, "Kullanıcı eklenemedi.");
                }else if (dbResult > 0)
                {
                    if (res.Result.IsActive==false)
                    {
                        res.Result = Find(x => x.Username == data.Username && x.Email == data.Email);
                        string siteUri = ConfigHelper.Get<string>("SiteRootUri");
                        string activateUri = $"{siteUri}/Home/UserActivate/{res.Result.ActivateGuid}";
                        string body = $"Merhaba { res.Result.Username}, <br><br>Hesabınızı aktifleştirmek için <a href='{activateUri}' target='_blank'>tıklayınız</a>.";
                        MailHelper.SendMail(body, res.Result.Email, "MyEvernote Hesap Aktifleştirme");
                    }

                }
            }
            return res;
        }

        public new BusinessLayerResult<EvernoteUser> Update(EvernoteUser model)
        {
            EvernoteUser db_user = Find(x => x.Id != model.Id && (x.Username == model.Username || x.Email == model.Email));
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();
            res.Result = model;
            if (db_user != null && db_user.Id != model.Id)
            {
                if (db_user.Username == model.Username)//aynı isimde email adresi yada kullanıcı adı varsa dönen modele eklenecek
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExist, "Kullanıcı adı kayıtlı.");
                }
                if (db_user.Email == model.Email)
                {
                    res.AddError(ErrorMessageCode.EmailAlreadyExist, "E-Posta adresi kayıtlı.");
                }
                return res;
            }
            res.Result = Find(x => x.Id == model.Id);
            res.Result.Email = model.Email;
            res.Result.Name = model.Name;
            res.Result.Surname = model.Surname;
            res.Result.Password = model.Password;
            res.Result.Username = model.Username;
            res.Result.IsActive = model.IsActive;
            res.Result.IsAdmin = model.IsAdmin;


            if (base.Update(res.Result) == 0)
            {
                res.AddError(ErrorMessageCode.UserCouldNotUpdated, "Kullanıcı güncellenemedi.");
            }
            return res;
        }
    }
}
