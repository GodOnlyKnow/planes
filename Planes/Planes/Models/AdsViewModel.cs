﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Planes.Models
{
    public class CreateAdsModel
    {
        [Required]
        [Display(Name = "广告标题")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "广告链接")]
        public string Link { get; set; }
        [Display(Name = "广告描述")]
        public string Desc { get; set; }
        [Display(Name = "展示图片")]
        public HttpPostedFileBase Img { get; set; }
        [Display(Name = "显示位置")]
        public string Position { get; set; }
    }

    public class EditAdsModel : CreateAdsModel
    {
        public int Id { get; set; }
        public string ImgUrl { get; set; }
    }
}