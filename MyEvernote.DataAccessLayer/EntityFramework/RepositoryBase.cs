using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.DataAccessLayer.EntityFramework
{
    //singletonPattern her defasında yeniden db nesnesi oluşmaması için tanımlandı
    public class RepositoryBase
    {
        protected static DatabaseContext db;
        private static object _lockSync = new object();//multi thread calısmalarında kullanılıyor lock object turu istiyor.
        protected RepositoryBase()//protected şeklinde sadece kalıtım alan classlar instance olusturabilir.
        {
            CreateContext();
        }
        private static void CreateContext()
        {
            if (db == null)//Constructor aşamasında ilk defa oluşurken null ise girip instance olusturacak diger durumlarda bu alana girişi mümkün olmayacak
            {
                lock (_lockSync)//multi thread durumlarında aynı anda 2 threadın girmesini önlemek için yapıldı.
                {
                    if (db == null)
                    {
                        db = new DatabaseContext();//private şekilde tek bir tane obje oluşması sağlandı.
                    }
                }
            }
        }
    }
}
