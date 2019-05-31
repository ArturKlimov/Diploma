using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Diploma.ViewModels
{
    public class AddVideoViewModel
    {
        [Required(ErrorMessage = "Вы не ввели ссылку")]
        public string VideoURL { get; set; }
    }
}