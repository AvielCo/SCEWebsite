using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SCE_Website.Models;

namespace SCE_Website.ViewModel
{
    public class UserViewModel
    {
            public User User { get; set; }
            public List<User> Users { get; set; }

    }
}