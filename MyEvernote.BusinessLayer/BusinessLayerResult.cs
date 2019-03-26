using MyEvernote.Entities.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.BusinessLayer
{
    //databasede olusabilecek hataları string listesine atarak dönmemizi sağlıyor.
    public class BusinessLayerResult<T> where T : class
    {
        //keyvaluepairi liste içerisinde 2 tip kullanabilmek için tanımladık. 2 tipden birincisi hata kodu diğeri hata mesajı olacak.(listeye çevirmeye karar verdik)
        public List<ErrorMessageObj> Errors { get; set; }
        public T Result { get; set; }
        public BusinessLayerResult()
        {
            Errors = new List<ErrorMessageObj>();
        }
        public void AddError(ErrorMessageCode code, string message)
        {
            Errors.Add(new ErrorMessageObj() { Code = code, Message = message });
        }
    }
}
