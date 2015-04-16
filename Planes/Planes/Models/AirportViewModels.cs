using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Planes.Models
{
    public class AirportSearchModel
    {
        public string Name { get; set; }
        public string UserName { get; set; }
    }

    public class AreaListModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public class AirportCreateModel
    {
        [Required]
        [Display(Name = "公司名称")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "公司账号")]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "显示状态")]
        public Int16 IsLock { get; set; }
        [Display(Name = "微信")]
        public string WX { get; set; }
        [Required]
        [Display(Name = "密码")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "地址")]
        public string Address { get; set; }
        [Display(Name = "电话号码")]
        public string Phone { get; set; }
        [Required]
        [Display(Name = "机场")]
        public int AreaId { get; set; }
        [Required]
        [Display(Name = "经度")]
        public string Lng { get; set; }
        [Required]
        [Display(Name = "纬度")]
        public string Lat { get; set; }
        [Display(Name = "公司所在地址")]
        public string AirportAddress { get; set; }
        [Display(Name = "类别")]
        public int GroupId { get; set; }
        [Display(Name = "展示图片")]
        public HttpPostedFileBase Img { get; set; }
        [Display(Name = "介绍图片")]
        public HttpPostedFileBase[] Imgs { get; set; }
        [Display(Name = "简介")]
        public string Desc { get; set; }
    }

    public class AirportEditModel : AirportCreateModel
    {
        
        public int Collection { get; set; }
        public int Planes { get; set; }
        [Required]
        public int Id { get; set; }
        public string ImgUrl { get; set; }
        public string[] ImgUrls { get; set; }
    }

}