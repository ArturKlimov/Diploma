using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Diploma.Models
{
    public class LoginModel
    {     
        //Логин пользователя
        [Required(ErrorMessage = "Введите Логин")]
        public string Login { get; set; }

        //Пароль пользователя
        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}