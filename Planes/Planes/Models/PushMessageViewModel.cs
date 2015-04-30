using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Planes.Models
{
    public class PushMessageCreate
    {
        [Required]
        [Display(Name = "标题")]
        public string Title { get; set; }
        [Required]
        [Display(Name = "内容")]
        public string Body { get; set; }
        [Required]
        [Display(Name = "推送平台")]
        public int Type { get; set; }
    }
}