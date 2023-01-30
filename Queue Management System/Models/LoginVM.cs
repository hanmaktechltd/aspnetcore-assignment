﻿using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Queue_Management_System.Models
{
    public class LoginVM
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int Password { get; set; }
    }
}
