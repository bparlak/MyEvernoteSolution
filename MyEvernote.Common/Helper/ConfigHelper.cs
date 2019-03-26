using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.Common.Helper
{
    //Appsettingse eklediğimiz değerleri okumak için oluşturduk.
    public class ConfigHelper
    {
        //Bu methoda appsettingsden gelen değer stringde olabilir integer da bu yüzden generic tanımlıyoruz
        public static T Get<T>(string key)
        {
            return (T)Convert.ChangeType(ConfigurationManager.AppSettings[key],typeof(T));//appsettingsden gelen parametre değerini al geri istenen yere dön
            //ConfigurationManager.AppSettings[key] bize string türünde değer dönüyor fakat kullanacağımız değer integer da olabileceği için tip dönüşümü yapmamız gerekiyor.Bu konuda Convert.ChangeType sınıfından yardım alıyoruz.Bu sınıf bizden değer ve dönüştürmek istediğimiz tipi istiyor, dönüşümü yapıp obje tipinde bize veriyor enson (T) yazarak fonksiyonun geridönüş tipine uyum sağlamış oluyoruz.
        }
    }
}
