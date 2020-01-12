using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCE_Website.Models
{
    [Table("tblUsers")]
    public class User
    {

        [Required]
        [Key, Column (Order = 0)]
        [StringLength(9, MinimumLength = 2, ErrorMessage = "Username must be between 2 and 9 characters")]
        public string ID { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string PermissionType { get; set; }
        [Required]
        public string Name { get; set; }
    }
}