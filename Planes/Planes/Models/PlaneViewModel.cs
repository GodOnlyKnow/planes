using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Planes.Models
{
    public class PlaneCreateBModel
    {
        [Required]
        public int AirportId { get; set; }
        [Required]
        [Display(Name = "出发地")]
        public string Col1 { get; set; }
        [Required]
        [Display(Name = "目的地")]
        public string Col2 { get; set; }
        [Required]
        [Display(Name = "型号")]
        public string Modelss { get; set; }
        [Required]
        [Display(Name = "人数")]
        public string Col4 { get; set; }
        [Required]
        [Display(Name = "出发时间")]
        public string  Col3 { get; set; }
        [Required]
        [Display(Name = "价格")]
        public decimal Price { get; set; }
        [Display(Name = "单位")]
        public string Unit { get; set; }
        [Required]
        [Display(Name = "图片")]
        public HttpPostedFileBase[] Imgs { get; set; }
        [Display(Name = "详情介绍")]
        public string Desc { get; set; }
        [Display(Name = "名称")]
        public string Name { get; set; }
    }

    public class PlaneCreateCModel : PlaneCreateBModel
    {
        [Required]
        [Display(Name = "航程")]
        public string Col5 { get; set; }
        [Display(Name = "返程时间")]
        public string Col6 { get; set; }
    }

    public class PlaneEditBModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [Display(Name = "出发地")]
        public string Col1 { get; set; }
        [Required]
        [Display(Name = "目的地")]
        public string Col2 { get; set; }
        [Required]
        [Display(Name = "型号")]
        public string Modelss { get; set; }
        [Required]
        [Display(Name = "人数")]
        public string Col4 { get; set; }
        [Required]
        [Display(Name = "出发时间")]
        public string Col3 { get; set; }
        [Required]
        [Display(Name = "价格")]
        public decimal Price { get; set; }
        [Display(Name = "单位")]
        public string Unit { get; set; }
        [Display(Name = "图片")]
        public HttpPostedFileBase[] Imgs { get; set; }
        [Display(Name = "详情介绍")]
        public string Desc { get; set; }
        [Display(Name = "名称")]
        public string Name { get; set; }
        public string[] ImgUrls { get; set; }
    }

    public class PlaneEditCModel : PlaneEditBModel
    {
        [Required]
        [Display(Name = "航程")]
        public string Col5 { get; set; }
        [Display(Name = "返程时间")]
        public string Col6 { get; set; }
    }
}