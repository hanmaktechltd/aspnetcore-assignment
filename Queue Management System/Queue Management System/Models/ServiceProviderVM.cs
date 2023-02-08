﻿using System.ComponentModel.DataAnnotations;

namespace Queue_Management_System.Models
{
    public class ServiceProviderVM
    {
        [Display(Name = "Service Provider Id")]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }
        public string Role { get; set; }

        [Display(Name = "Service Point Id")]        
        public int ServicepointId { get; set; }
    }
}
