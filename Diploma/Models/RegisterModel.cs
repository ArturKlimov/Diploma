using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diploma.Models
{
    public class RegisterModel
    {
        //Почта пользователя
        [Required(ErrorMessage = "Введите почту")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный адрес")]
        public string Email { get; set; }

        //ФИО пользователя
        [Required(ErrorMessage = "Введите ФИО")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Введите Логин")]
        [StringLength(20, ErrorMessage = "Максимальная длина логина - 20 символов")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}