using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Diploma.Models
{
    public class New
    {
        //ID
        public int ID { get; set; }

        //Заголовок новости
        [Required(ErrorMessage = "Введите заголовок новости")]
        public string Title { get; set; }

        //Описание новости
        [Required(ErrorMessage = "Введите описание новости")]
        public string Description { get; set; }

        //Дата публикации новости
        public DateTime Date { get; set; }

        //Путь к изображению
        public string ImagePath { get; set; }

        //ID категории новости
        public int? CategoryID { get; set; }

        //Навигационное свойство для категорий
        public Category Category { get; set; }
    }
}