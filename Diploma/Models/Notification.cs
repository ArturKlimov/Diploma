using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Diploma.Models
{
    public class Notification
    {
        //ID объявления
        public int ID { get; set; }

        //Заголовок объявления
        [Required(ErrorMessage = "Введите заголовок объявления")]
        public string Title { get; set; }

        //Описание объявления
        [Required(ErrorMessage = "Введите описание объявления")]
        public string Description { get; set; }

        //Дата публикации
        public DateTime Date { get; set; }
    }
}