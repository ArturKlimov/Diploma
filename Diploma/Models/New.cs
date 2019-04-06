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
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Длина заголовка должна быть от 5 до 20 символов")]
        public string Title { get; set; }

        //Описание новости
        [Required(ErrorMessage = "Введите описание новости")]
        [StringLength(200, ErrorMessage = "Длина новости не может быть больше 200 символов")]
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