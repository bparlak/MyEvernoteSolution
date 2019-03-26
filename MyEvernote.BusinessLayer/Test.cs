using MyEvernote.DataAccessLayer.EntityFramework;
using MyEvernote.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.BusinessLayer
{
    public class Test
    {
        private Repository<EvernoteUser> repo_user = new Repository<EvernoteUser>();
        private Repository<Category> repo_category = new Repository<Category>();
        private Repository<Comment> repo_comment = new Repository<Comment>();
        private Repository<Note> repo_note = new Repository<Note>();
        public Test()
        {
            List<Category> cat= repo_category.List();
        }
        public void InsertTest()
        {
            int result = repo_user.Insert(new EvernoteUser()
            {
                Name = "Alican",
                Surname = "Yılmaz",
                Email = "blue@dark.com",
                ActivateGuid = Guid.NewGuid(),
                IsActive = true,
                IsAdmin = true,
                Username = "ayilmaz",
                Password = "123",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now.AddMinutes(5),
                ModifiedUsername = "ayilmaz"
            });
        }
        public void TestUpdate()
        {
            EvernoteUser user = repo_user.Find(x => x.Username == "ayilmaz");
            if (user!=null)
            {
                user.Username = "bluedark";
                int result=repo_user.Save();
            }
        }
        public void TestDelete()
        {
            EvernoteUser user = repo_user.Find(x => x.Username == "ayilmaz");
            if (user != null)
            {
                int result = repo_user.Delete(user);
            }
        }
        public void TestComment()
        {
            EvernoteUser user = repo_user.Find(x => x.Id == 3);
            Note note = repo_note.Find(x => x.Id == 5);
            Comment comment = new Comment()
            {
                Text="bu bir denemedir.",
                CreatedOn=DateTime.Now,
                ModifiedOn=DateTime.Now,
                ModifiedUsername="bparlak",
                Note=note,
                Owner=user
            };
            int result;
            result=repo_comment.Insert(comment);
        }
    }
}
