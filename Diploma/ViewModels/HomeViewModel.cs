using Diploma.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Diploma.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Notification> Notifications { get; set; }

        public IEnumerable<New> News { get; set; }

        public IEnumerable<Video> Videos { get; set; }

        public IEnumerable<Event> Events { get; set; }
    }
}