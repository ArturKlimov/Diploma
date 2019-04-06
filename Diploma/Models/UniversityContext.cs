using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Diploma.Models
{
    public class UniversityContext : DbContext
    {
        static UniversityContext()
        {
            Database.SetInitializer(new ContextInitializer());
        }
        public DbSet<New> News { get; set; }
        public DbSet<Category> Categories { get; set; }
    }

    //Инициализация базы данных при изменении модели
    public class ContextInitializer : DropCreateDatabaseIfModelChanges<UniversityContext>
    {
        protected override void Seed(UniversityContext db)
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

            //Сохраняем измения в базе данных
            db.SaveChanges();
        }
    }
}