﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diploma.Models
{
    public class Recipient
    {
        //ID группы получателей
        public int ID { get; set; }

        //Название группы получателей
        public string Title { get; set; }

        //Коллекция групп получателей
        public ICollection<Recipient> Recipients { get; set; }

        //Конструктор для групп получателей
        public Recipient()
        {
            Recipients = new List<Recipient>();
        }
    }
}