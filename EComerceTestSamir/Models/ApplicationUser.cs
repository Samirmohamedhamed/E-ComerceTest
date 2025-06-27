﻿using Microsoft.AspNetCore.Identity;

namespace EComerceTestSamir.Models
{
    public class ApplicationUser: IdentityUser
    {
        public string?  Address { get; set; }
        public int Age { get; set; }
        public List<Favorite> Favorites { get; set; } =new List<Favorite>();


    }
}
