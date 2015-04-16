using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Planes.Models
{
    public class UserViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [Display(Name = "会员等级")]
        public int LevelId { get; set; }
    }
}