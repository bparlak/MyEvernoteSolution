using MyEvernote.BusinessLayer.Abstract;
using MyEvernote.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.BusinessLayer
{
    public class CategoryManager : ManagerBase<Category>
    {
        //public override int Delete(Category category)
        //{
        //    NoteManager noteManager = new NoteManager();
        //    LikedManager likedManager = new LikedManager();
        //    CommentManager commentManager = new CommentManager(); 
        //    //Kategori ile ilişkili notların silinmesi için ezildi
        //    foreach (Note note in category.Notes.ToList())
        //    {
        //        //note ile ilişkili likeler silinmeli
        //        foreach (Liked like in note.Likes.ToList())
        //        {
        //            likedManager.Delete(like);
        //        }
        //        // note ile ilikili commentler silindi
        //        foreach(Comment comment in note.Comments.ToList())
        //        {
        //            commentManager.Delete(comment);
        //        }
        //        noteManager.Delete(note);
        //    }

        //    return base.Delete(category);
        //}
    }
}
