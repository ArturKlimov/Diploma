using System;
using System.Collections.Generic;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;

namespace Diploma.Models
{

    public class ApplicationContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationContext() : base("UniversityContext", throwIfV1Schema: false)
        {
            Database.SetInitializer(new ContextInitializer());
        }

        public static ApplicationContext Create()
        {
            return new ApplicationContext();
        }

        public DbSet<New> News { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Email> Emails { get; set; }

        public DbSet<Recipient> Recipients { get; set; }

    }

    //Инициализация базы данных при изменении модели
    public class ContextInitializer : DropCreateDatabaseIfModelChanges<ApplicationContext>
    {
        protected override void Seed(ApplicationContext db)
        {
            //Объявляем и инициализируем новые категории новостей
            Category category1 = new Category { Title = "Достижения" };
            Category category2 = new Category { Title = "Культура" };
            Category category3 = new Category { Title = "Международная деятельность" };
            Category category4 = new Category { Title = "Наука и инновации" };
            Category category5 = new Category { Title = "Образование" };
            Category category6 = new Category { Title = "Партнерство" };
            Category category7 = new Category { Title = "Спорт" };
            Category category8 = new Category { Title = "Университетская жизнь" };

            //Добавляем категории в базу данных
            db.Categories.Add(category1);
            db.Categories.Add(category2);
            db.Categories.Add(category3);
            db.Categories.Add(category4);
            db.Categories.Add(category5);
            db.Categories.Add(category6);
            db.Categories.Add(category7);
            db.Categories.Add(category8);

            //Объявляем и инициализируем группы получателей
            Recipient recipient1 = new Recipient { Title = "Преподаватели" };
            Recipient recipient2 = new Recipient { Title = "Старосты" };

            db.Recipients.Add(recipient1);
            db.Recipients.Add(recipient2);

            //Создаем первого администратора
            ApplicationUserManager userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));

            ApplicationUser admin = new ApplicationUser { UserName = "admin1" };

            IdentityResult result = userManager.Create(admin, "adminfti123");

            //Сохраняем измения в базе данных
            db.SaveChanges();
        }
    }
}