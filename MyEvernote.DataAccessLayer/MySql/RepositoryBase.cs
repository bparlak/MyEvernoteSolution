using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.DataAccessLayer.MySql
{
    public class RepositoryBase
    {
        protected static object db;
        private static object _lockSync = new object();//multi thread calısmalarında kullanılıyor lock object turu istiyor.
        protected RepositoryBase()
        {
            CreateContext();
        }
        private static void CreateContext()
        {
            if (db == null)
            {
                lock (_lockSync)
                {
                    if (db == null)
                    {
                        db = new object();
                    }
                }
            }
        }
    }
}
