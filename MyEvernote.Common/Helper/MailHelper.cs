using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.Common.Helper
{
    public class MailHelper
    {
        //nesneyi yeniden olusturmadan gönderme işlemi yapabilmek için static olusturduk.
        public static bool SendMail(string body, string to, string subject, bool isHtml = true)
        {
            //tek adrese gönderim yapıyor fakat tek mailide listeye ekleyip alttaki methoda gönderiyor bi espirisi yok.
            return SendMail(body, new List<string> { to }, subject, isHtml);
        }
        //mailin toplu atılma durumu olduğu için to stringi list olarak tanımlandı--isHtml gönderilen veri html mi normla mesaj mı
        private static bool SendMail(string body, List<string> to, string subject, bool isHtml)
        {
            bool result = false;
            try
            {
                var message = new MailMessage();
                //kimden kime
                message.From = new MailAddress(ConfigHelper.Get<string>("MailUser"));
                to.ForEach(x =>
                {
                    message.To.Add(new MailAddress(x));
                });
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = isHtml;
                //hangi sunucu ve port
                using (var smtp=new SmtpClient(ConfigHelper.Get<string>("MailHost"),ConfigHelper.Get<int>("MailPort")))
                {
                    smtp.EnableSsl = true;
                    //kimin yetkileriyle gönderilecek.
                    smtp.Credentials = new NetworkCredential(ConfigHelper.Get<string>("MailUser"), ConfigHelper.Get<string>("MailPass"));
                    
                    smtp.Send(message);
                    result = true;
                }
            }
            catch (Exception)
            {
                
            }
            return result;
        }
    }
}
