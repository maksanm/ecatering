﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECaterer.WebApi.Models
{
    public class RegisterUserModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
