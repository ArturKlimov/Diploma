using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diploma.Models
{
    public class Notification
    {
        //ID объявления
        public int ID { get; set; }

        //Заголовок объявления
        public string Title { get; set; }

        //Описание объявления
        public string Description { get; set; }

        //Дата публикации
        public DateTime Date { get; set; }
    }
}