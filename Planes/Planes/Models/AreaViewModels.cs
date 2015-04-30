using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Planes.Models
{
    public class AreaViewModels
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public class AreaSearchModel
    {
        [Display(Name = "机场名称")]
        public string Name { get; set; }
        [Display(Name = "机场地址")]
        public string Address { get; set; }
    }

    public class AreaCreateModel
    {
        [Required]
        [Display(Name = "机场名称")]
        public string Name { get; set; }
        [Display(Name = "省")]
        public int Province { get; set; }
        [Display(Name = "市")]
        public int City { get; set; }
        [Display(Name = "县/区域")]
        public int Area { get; set; }
        [Required]
        [Display(Name = "经度")]
        public string Lng { get; set; }
        [Required]
        [Display(Name = "纬度")]
        public string Lat { get; set; }
        [Required]
        [Display(Name = "详细地址")]
        public string Address { get; set; }
        [Display(Name = "机场图片")]
        public HttpPostedFileBase Img { get; set; }
    }

    public class AreaEditModel : AreaCreateModel
    {
        [Required]
        public int Id { get; set; }
        public string ImgUrl { get; set; }
    }
}