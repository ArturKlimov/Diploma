using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Diploma.Models
{
    public class ApplicationUser : IdentityUser
    {
        //ФИО пользователя
        public string Name { get; set; }

        public ApplicationUser()
        {
        }
    }
}