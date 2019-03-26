using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using MyEvernote.Entities;

namespace MyEvernote.DataAccessLayer.EntityFramework
{
    public class MyInitializer : CreateDatabaseIfNotExists<DatabaseContext>
    {
        protected override void Seed(DatabaseContext context)
        {
            //adding admin
            EvernoteUser admin = new EvernoteUser()
            {
                Name = "Burak",
                Surname = "Parlak",
                Email = "info@bparlak.com",
                ActivateGuid = Guid.NewGuid(),
                IsActive = true,
                IsAdmin = true,
                Username = "bparlak",
                Password = "123",
                ProfileImageFilename= "profilepic.png",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now.AddMinutes(5),
                ModifiedUsername = "bparlak"
            };
            //adding standart user
            EvernoteUser standartUser = new EvernoteUser()
            {
                Name = "Enes",
                Surname = "Parlak",
                Email = "enes@eparlak.com",
                ActivateGuid = Guid.NewGuid(),
                IsActive = true,
                IsAdmin = false,
                ProfileImageFilename = "profilepic.png",
                Username = "eparlak",
                Password = "123",
                CreatedOn = DateTime.Now.AddHours(1),
                ModifiedOn = DateTime.Now.AddMinutes(61),
                ModifiedUsername = "bparlak"
            };
            context.EvernoteUsers.Add(admin);
            context.EvernoteUsers.Add(standartUser);

            //adding fake user
            for (int i = 0; i < 8; i++)
            {
                EvernoteUser user = new EvernoteUser()
                {
                    Name = FakeData.NameData.GetFirstName(),
                    Surname = FakeData.NameData.GetSurname(),
                    Email = FakeData.NetworkData.GetEmail(),
                    ActivateGuid = Guid.NewGuid(),
                    IsActive = true,
                    IsAdmin = false,
                    ProfileImageFilename = "profilepic.png",
                    Username = $"user{i}",
                    Password = "123",
                    CreatedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                    ModifiedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                    ModifiedUsername = $"user{i}"
                };
                context.EvernoteUsers.Add(user);
            }

            context.SaveChanges();

            List<EvernoteUser> userList = context.EvernoteUsers.ToList();
            //adding fake categories
            for (int i = 0; i < 10; i++)
            {
                Category cat = new Category()
                {
                    Title = FakeData.PlaceData.GetStreetName(),
                    Description = FakeData.PlaceData.GetAddress(),
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    ModifiedUsername = "bparlak"
                };

                context.Categories.Add(cat);
                //adding notes
                cat.Notes = new List<Note>();
                for (int k = 0; k < FakeData.NumberData.GetNumber(5, 9); k++)
                {
                    EvernoteUser owner = userList[FakeData.NumberData.GetNumber(1, userList.Count - 1)];
                    Note note = new Note()
                    {
                        Title = FakeData.TextData.GetAlphabetical(FakeData.NumberData.GetNumber(5, 25)),
                        Text = FakeData.TextData.GetSentences(FakeData.NumberData.GetNumber(1, 3)),
                        IsDraft = false,
                        LikeCount = FakeData.NumberData.GetNumber(1, 9),
                        Owner = owner,
                        CreatedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                        ModifiedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                        ModifiedUsername = owner.Username,

                    };
                    cat.Notes.Add(note);

                    //adding fake comments
                    note.Comments = new List<Comment>();
                    for (int j = 0; j < FakeData.NumberData.GetNumber(3, 5); j++)
                    {
                        EvernoteUser commentowner = userList[FakeData.NumberData.GetNumber(1, userList.Count - 1)];
                        Comment comment = new Comment()
                        {
                            Text = FakeData.TextData.GetSentence(),
                            Owner = commentowner,
                            CreatedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                            ModifiedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                            ModifiedUsername = commentowner.Username,

                        };
                        note.Comments.Add(comment);
                    }
                    //addlikes
                    note.Likes = new List<Liked>();
                    for (int t = 0; t < note.LikeCount; t++)
                    {
                        Liked liked = new Liked()
                        {
                            LikedUser=userList[t]
                        };
                        note.Likes.Add(liked);
                    }
                }
            }
            context.SaveChanges();
        }
    }
}
