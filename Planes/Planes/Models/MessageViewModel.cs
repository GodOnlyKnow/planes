using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Planes.Models
{
    public class MessageCreateModel
    {
        [Required]
        public int SellerId { get; set; }
        [Required]
        [Display(Name = "选择一个飞机")]
        public int GoodId { get; set; }
        [Required]
        [Display(Name = "动态内容")]
        public string Content { get; set; }
        [Display(Name = "展示图片")]
        public HttpPostedFileBase Img { get; set; }
        public string ImgUrl { get; set; }
        public int Id { get; set; }
        [Display(Name = "飞机")]
        public string GoodName { get; set; }
    }

    public class MessagePlanesModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Img { get; set; }
    }
}