using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diploma.Models
{
    public class Video
    {
        //ID видео
        public int ID { get; set; }

        //ID yotube
        public string YoutubeID { get; set; }

        //Заголовок
        public string Title { get; set; }

        //Описание
        public string Description { get; set; }

        //Дата
        public DateTime Date { get; set; }
    }
}