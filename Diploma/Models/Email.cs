using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Diploma.Models
{
    public class Email
    {
        //ID email
        public int ID { get; set; }
        
        //Email
        [Required(ErrorMessage = "Вы не ввели почту")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный адрес")]
        public string Mail { get; set; }

        //Владелец почты для справочной информации
        [StringLength(50, ErrorMessage = "Максимум 50 символов")]
        public string Name { get; set; }

        //ID группы получателей
        public int? RecipientID { get; set; }

        //Навигационное свойство для группы получателей
        public Recipient Recipient { get; set; }

    }
}