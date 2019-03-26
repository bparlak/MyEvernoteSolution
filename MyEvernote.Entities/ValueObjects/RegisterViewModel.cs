using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyEvernote.Entities.ValueObjects
{
    public class RegisterViewModel
    {
        [DisplayName("Adınız"),
            StringLength(30, ErrorMessage = "{0} {1} karakteri aşamaz.")]
        public string Name { get; set; }

        [DisplayName("Soy Adınız"),
            StringLength(30, ErrorMessage = "{0} {1} karakteri aşamaz.")]
        public string Surname { get; set; }
        [DisplayName("Kullanıcı Adı"),
            Required(ErrorMessage = "{0} alanı boş geçilemez."),
            StringLength(25, ErrorMessage = "{0} {1} karakteri aşamaz.")]
        public string Username { get; set; }

        [DisplayName("E-Posta"),
            Required(ErrorMessage = "{0} alanı boş geçilemez."),
            StringLength(70, ErrorMessage = "{0} {1} karakteri aşamaz."),
            EmailAddress(ErrorMessage = "Lütfen geçerli bir {0} giriniz.")]
        public string EMail { get; set; }

        [DisplayName("Şifre"),
            Required(ErrorMessage = "{0} alanı boş geçilemez."),
            DataType(DataType.Password),
            StringLength(25, ErrorMessage = "{0} {1} karakteri aşamaz.")]
        public string Password { get; set; }

        [DisplayName("Şifre (Tekrar)"),
            Required(ErrorMessage = "{0} alanı boş geçilemez."),
            DataType(DataType.Password),
            Compare("Password", ErrorMessage = "{0} {1} uyuşmuyor."),
            StringLength(25, ErrorMessage = "{0} {1} karakteri aşamaz.")]
        public string RePassword { get; set; }
    }
}