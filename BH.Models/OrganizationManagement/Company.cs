﻿using System.ComponentModel.DataAnnotations;

namespace BH.Models.OrganizationManagement
{
    public class Company
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? PhoneNumber { get; set; }
        public string? WebSiteAddress { get; set; }
    }
}
