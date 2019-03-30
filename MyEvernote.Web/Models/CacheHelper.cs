using MyEvernote.BusinessLayer;
using MyEvernote.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace MyEvernote.Web.Models
{
    //veriler için önce cache bakar varsa cache den ceker yoksa databaseden getirecek.
    public class CacheHelper
    {
        public static List<Category> GetCategoriesFromCache()
        {
            //cache kontrol et boş mu dolu mu
            var result = WebCache.Get("category-cache");
            if (result == null)//eğer boş ise
            {
                CategoryManager categoryManager = new CategoryManager();
                result = categoryManager.List();//category leri getir
                WebCache.Set("category-cache", result,20,true);//Kategorileri cache at
            }
            return result;//burada her 2 durumdada içerisi doldurularak kategoriler dönecek.
        }
        public static void RemoveCategoriesFrom()
        {
            WebCache.Remove("category-cache");
        }
        public static void RemoveCache(string key)
        {
            WebCache.Remove(key);
        }
    }
}