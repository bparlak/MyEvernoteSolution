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
                int dbResult = Insert(new EvernoteUser()
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


                    //SmtpClient smtp = new SmtpClient();//gideceği yer server
                    //smtp.Host = "smtp.gmail.com";
                    //smtp.Port = 587;
                    //smtp.EnableSsl = true;

                    //NetworkCredential auth = new NetworkCredential();//nasıl gidecek
                    //auth.UserName = "infoprojects01@gmail.com";
                    //auth.Password = "Aa123456*";
                    //smtp.Credentials = auth;
                    ////buraya kadar ayarlar yapıldı alt yapı hazırlandı
                    //MailMessage msg = new MailMessage();
                    //msg.Subject = "Uyelik Dogrulama";
                    //msg.Body = @"<strong> Email: [email] </strong><br/>".Replace("[email]", res.Result.Email);
                    //msg.Body += @"<strong> Password: [password] </strong><br/>".Replace("[password]", res.Result.Password);
                    //msg.Body += @"<strong> Username and password created successfully. </strong><br/>";
                    //msg.IsBodyHtml = true;//body içerisinde yazılan html tagları aktif hale getiriyor
                    //msg.To.Add(res.Result.Email);//mailin gideceği adres
                    //msg.From = new MailAddress("infoprojects01@gmail.com", "MyEvernote");

                    //smtp.Send(msg);




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

            if (Update(res.Result) == 0)
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
    }
}
