using MyEvernote.Common;
using MyEvernote.DataAccessLayer.Abstract;
using MyEvernote.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.DataAccessLayer.EntityFramework
{
    public class Repository<T> : RepositoryBase, IRepository<T> where T : class//Repository yalnızca class türünden oluşabilir.
    {
        private DbSet<T> _objectSet;
        public Repository()
        {
            _objectSet = db.Set<T>();//Genericte gelen classa ait database bilgileri Constructor aşamasında alınıyor.
        }
        //Constructor aşamasında databaseden gelen table üzerinden, sorgulamalar ve işlemler aşağıdaki methodlarda gerçekleşiyor.
        //Aynı methodların tekrar tekrar tanımlamanmasına önlem olarak oluşturulan RepositoryPattern yapısı kullanıldı.
        public List<T> List()
        {
            return _objectSet.ToList();
        }
        public IQueryable<T> ListQueryable()
        {
            return _objectSet;
        }
        public List<T> List(Expression<Func<T, bool>> where)//expression<func<t,bool>> name---linq yazmamızı sağlıyor.
        {
            return _objectSet.Where(where).ToList();
        }
        public int Insert(T obj)
        {
            _objectSet.Add(obj);
            if (obj is MyEntityBase)//her kullanıcı için standart belirtilen şeyler olduğu için buraya taşıdık
            {
                MyEntityBase o = obj as MyEntityBase;
                DateTime now = DateTime.Now; ;

                o.CreatedOn = now;
                o.ModifiedOn = now;
                o.ModifiedUsername = App.Common.GetCurrentUsername() ;// TODO: işlem yapan kullanıcıyı belirt
            }
            return Save();
        }
        public int Update(T obj)
        {
            if (obj is MyEntityBase)
            {
                MyEntityBase o = obj as MyEntityBase;
                o.ModifiedOn = DateTime.Now;
                o.ModifiedUsername = App.Common.GetCurrentUsername();// TODO: işlem yapan kullanıcıyı belirt
            }
            return Save();
        }
        public int Delete(T obj)
        {
            _objectSet.Remove(obj);
            return Save();
        }
        public int Save()
        {
            return db.SaveChanges();
        }
        public T Find(Expression<Func<T, bool>> where)
        {
            return _objectSet.FirstOrDefault(where);
        }
    }
}
