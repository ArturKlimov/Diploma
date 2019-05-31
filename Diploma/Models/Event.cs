using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Diploma.Models
{
    public class Event
    {
        //ID мероприятия
        public int ID { get; set; }

        //Заголовок мероприятия
        [Required(ErrorMessage = "Введите заголовок")]
        public string Title { get; set; }

        //Описание мероприятия
        [Required(ErrorMessage = "Введите описание")]
        public string Description { get; set; }

        //Дата и время начала мероприятия
        [DataType(DataType.DateTime, ErrorMessage = "Неправильная дата")]
        public DateTime StartDate { get; set; }

        //Дата и время конца мероприяти
        [DataType(DataType.DateTime, ErrorMessage = "Неправильная дата")]
        public DateTime EndDate { get; set; }

    }
}