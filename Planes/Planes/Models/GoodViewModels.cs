using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Planes.Models
{
    public class CreateOfPlaneModel
    {
        public int AirportId { get; set; }
        [Required]
        [Display(Name = "飞机品牌")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "飞机型号")]
        public string Modelss { get; set; }
        [Display(Name = "价格(￥)")]
        public decimal Price { get; set; }
        [Display(Name = "单位")]
        public string Unit { get; set; }
        [Required]
        [Display(Name = "飞机价值(￥)")]
        public decimal MarketPrice { get; set; }
        [Display(Name = "展示图片")]
        public HttpPostedFileBase Img { get; set; }
        [Display(Name = "飞机类型")]
        public string Col1 { get; set; }
        [Display(Name = "飞机产地")]
        public string Col2 { get; set; }
        [Display(Name = "续航里程(km)")]
        public string Col3 { get; set; }
        [Required]
        [Display(Name = "座位数(人)")]
        public string Col4 { get; set; }
        [Display(Name = "生产年代")]
        public string ProductionTime { get; set; }
        [Display(Name = "飞机颜色")]
        public string Col5 { get; set; }
        [Display(Name = "巡航速度(km/h)")]
        public string Col6 { get; set; }
        [Display(Name = "每小时飞行价格(￥/h)")]
        public string Col7 { get; set; }
        [Display(Name = "展示价格(￥/天)")]
        public string Col8 { get; set; }
        [Display(Name = "飞机介绍")]
        public string Desci { get; set; }
        [Display(Name = "私照价格(￥)")]
        public string Col9 { get;set; }
        [Display(Name = "商照价格(￥)")]
        public string Col10 { get; set; }
        [Display(Name = "销售类型")]
        public int GoodType { get; set; }
        public HttpPostedFileBase[] Imgs { get; set; }
    }

    public class EditOfPlaneModel : CreateOfPlaneModel
    {
        public string ImgUrl { get; set; }
        public string[] ImgUrls { get; set; }
        public int Id { get; set; }
        public int Collection { get; set; }
        public int Saled { get; set; }
    }

    public class CreateGoodModel
    {
        [Required]
        [Display(Name = "名称")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "价格")]
        public decimal Price { get; set; }
        [Display(Name = "单位")]
        public string Unit { get; set; }
        [Display(Name = "展示图片")]
        public HttpPostedFileBase Img { get; set; }
        [Required]
        [Display(Name = "所属类型")]
        public int TypeId { get; set; }
        [Display(Name = "商品描述")] 
        public string Desc { get; set; }
        [Display(Name = "介绍图片")]
        public HttpPostedFileBase[] Imgs { get; set; }
    }

    public class EditGoodModel : CreateGoodModel
    {
        [Required]
        public int Id { get; set; }
        public string ImgUrl { get; set; }
        public string[] ImgUrls { get; set; }
    }

}