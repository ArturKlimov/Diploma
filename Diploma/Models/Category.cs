using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diploma.Models
{
    public class Category
    {
        //ID категории
        public int ID { get; set; }
        //Название категории
        public string Title { get; set; }
        //Коллекция новостей
        public ICollection<New>  News { get; set; }
        //Конструктор для категории
        public Category()
        {
            News = new List<New>();
        }
    }
}