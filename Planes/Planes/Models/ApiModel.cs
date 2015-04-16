using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Planes.Models
{
    // 注册
    public class ApiRegistRqeustModel
    {
        public string userName { get; set; }
        public string phone { get; set; }
        public string password { get; set; }
        public string headImg { get; set; }
    }

    // 用户信息
    public class ApiModifyUserProfileRquestModel
    {
        public string phone { get; set; }
        public string userName { get; set; }
        public string trueName { get; set; }
        public string idCard { get; set; }
        public bool gender { get; set; }
        public string address { get; set; }
        public string workUnit { get; set; }
    }

    // 用户
    public class ApiUserModel
    {
        public string userName { get; set; }
        public string phone { get; set; }
        public string password { get; set; }
        public string headImg { get; set; }
        public string randId { get; set; }
        public string trueName { get; set; }
        public string idCard { get; set; }
        public bool gender { get; set; }
        public string address { get; set; }
        public string workUnit { get; set; }
    }

    // 登陆
    public class ApiLoginRequestModel
    {
        public string phone { get; set; } 
        public string password { get; set; }
    }

    // 用户订单
    public class ApiUserOrderRequestModel
    {
        public string phone { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }

    }
    public class ApiUserOrderModel
    {
        public string orderId { get; set; }
        public string createdAt { get; set; }
        public decimal price { get; set; }
        public string status { get; set; }
        public string name { get; set; }
        public string img { get; set; }
        
    }

    // 用户修改密码
    public class ApiUserChangePasswordRequestModel
    {
        public string phone { get; set; }
        public string password { get; set; }
    }
    // 动态
    public class ApiSellerMessageRequestModel
    {
        public int page { get; set; }
        public int pageSize { get; set; }
    }

    public class ApiSellerMessageModel
    {
        public int id { get; set; } // 动态id
        public string idString {get;set;}
        public string title { get; set; }
        public string sellerName { get; set; }
        public string summary { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public string detail { get; set; }
        public string img { get; set; }
        public int good { get; set; }
        public int bad { get; set; }
        public int shared { get; set; }
        public ApiSellerMessageCommentModel last { get; set; }
    }

    // 动态点赞等
    public class ApiAddSellerMessagesCountModel
    {
        public int id { get; set; } // 动态id
        public int good { get; set; }
        public int bad { get; set; }
        public int shared { get; set; }
    }

    // 新增动态评论
    public class ApiAddSellerMessageCommentModel
    {
        public int id { get; set; } // 动态id
        public string phone { get; set; } // 用户手机
        public string body { get; set; }
        public string imgs { get; set; }
    }

    // 上传图片
        // base64
    public class ApiUploadImageBase64Model
    {
        public string phone { get; set; } // 用户手机
        public string code { get; set; }
    }
        // http post
    public class ApiUploadImagePostModel
    {
        public string phone { get; set; }
        public HttpPostedFileBase img { get; set; }
    }

    // 获取动态评论
    public class ApiSellerMessageCommentsRequestModel
    {
        public int id { get; set; } // 动态id
        public int page { get; set; }
        public int pageSize { get; set; }
    }

    public class ApiSellerMessageCommentModel
    {
        public int id { get; set; } // 动态评论id
        public string idString { get; set; }
        public string userName { get; set; }
        public string userImg { get; set; }
        public string createdAt { get; set; }
        public string body { get; set; }
        public string[] imgs { get; set; }
        public int good { get; set; }
        public List<ApiSellerMessageCommentReplyModel> replys { get; set; }
    }

        // 动态评论 回复
    public class ApiSellerMessageCommentReplyRequestModel
    {
        public int id { get; set; } // 评论id
        public string phone { get; set; }
        public string body { get; set; }
    }
    public class ApiSellerMessageCommentReplyModel
    {
        public int id { get; set; }
        public string userName { get; set; }
        public string body { get; set; }
    }

    // 动态评论点赞
    public class ApiAddSellerMessageCommentGoodModel
    {
        public int id { get; set; } // 动态评论id
        public string idString { get; set; }
        public int good { get; set; }
    }

    // 租飞机
        // Q-Sort 
    public class ApiSortModel
    {
        public int id { get; set; }
        public double dis { get; set; }

        public string lat { get; set; }
        public string lng { get; set; }
    }
        //预定航程
            // 机场
    public class ApiGetAreasRequestModel
    {
        public double lat { get; set; }
        public double lng { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
    }
       
    public class ApiGetAreasModel : ApiSortModel
    {
        public string idString { get; set; }
        public string name { get; set; }
        public string img { get; set; }
        public string address { get; set; }
        public List<ApiSellerModel> sellers { get; set; }
    }
            // 航空公司
    public class ApiGetSellersRequestModel : ApiGetAreasRequestModel
    {
        
    }
    public class ApiGetSellersModel : ApiSortModel
    {
        public string idString { get; set; }
        public string name { get; set; }
        public string img { get; set; }
        public string address { get; set; }
        public List<ApiPlaneAAAModel> planes { get; set; }
    }

    public class ApiSellerModel
    {
        public int id { get; set; } // 航空公司id
        public string idString { get; set; }
        public string name { get; set; }
        public string img { get; set; }
        public List<ApiPlaneAAAModel> planes { get; set; }
        
    }
    // 飞机
        // 预定航程
    public class ApiPlaneAAAModel
    {
        [Display(Name = "飞机ID")]
        public int id { get; set; }
        public string idString { get; set; }
        [Display(Name = "飞机品牌")]
        public string name { get; set; }
        [Display(Name = "飞机型号")]
        public string model { get; set; }
        [Display(Name = "飞机价值(￥)")]
        public decimal marketPrice { get; set; }
        [Display(Name = "展示图片")]
        public string img { get; set; }
        [Display(Name = "飞机类型")]
        public string col1 { get; set; }
        [Display(Name = "飞机产地")]
        public string col2 { get; set; }
        [Display(Name = "续航里程(km)")]
        public string col3 { get; set; }
        [Display(Name = "座位数(人)")]
        public string col4 { get; set; }
        [Display(Name = "巡航速度(km/h)")]
        public string col6 { get; set; }
        [Display(Name = "每小时飞行价格(￥/h)")]
        public string col7 { get; set; }
        [Display(Name = "展示价格(￥/天)")]
        public string col8 { get; set; }
        [Display(Name = "飞机介绍")]
        public string desci { get; set; }
        [Display(Name = "详情图片")]
        public string[] imgs { get; set; }
        [Display(Name = "销量")]
        public int saled { get; set; }
        [Display(Name = "评论数")]
        public int comments { get; set; }
        [Display(Name = "收藏数")]
        public int collection { get; set; }
        [Display(Name = "访问量")]
        public int visited { get; set; }
    }
    // 获取 特价包机
    public class ApiGetPlaneBBBRequestModel
    {
        public double lat { get; set; }
        public double lng { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
    }
    // 获取 飞的航线
    public class ApiGetPlaneBBBModel : ApiSortModel
    {
        public string idString { get; set; }
        [Display(Name = "出发地")]
        public string col1 { get; set; }
        [Display(Name = "目的地")]
        public string col2 { get; set; }
        [Display(Name = "型号")]
        public string model { get; set; }
        [Display(Name = "人数")]
        public string col4 { get; set; }
        [Display(Name = "出发时间")]
        public string col3 { get; set; }
        [Display(Name = "价格")]
        public decimal price { get; set; }
        [Display(Name = "价格单位")]
        public string unit { get; set; }
        [Display(Name = "图片")]
        public string[] imgs { get; set; }
        [Display(Name = "详情介绍")]
        public string desc { get; set; }
        [Display(Name = "名称")]
        public string name { get; set; }
    }

    // 获取 特价包机
    public class ApiGetPlaneCCCModel : ApiGetPlaneBBBModel
    {
        [Display(Name = "航程")]
        public string col5 { get; set; }
        [Display(Name = "返回时间")]
        public string col6 { get; set; }
    }



    // 广告
    public class ApiAdModel
    {
        public string name { get; set; }
        public string link { get; set; }
        public string desc { get; set; }
        public string img { get; set; }
        public string position { get; set; }
    }

    // 学飞行
        // 国内 & 国外航校
    public class ApiGetSchoolRequestModel
    {
        public double lat { get; set; }
        public double lng { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
    }

    public class ApiGetSchoolModel : ApiSortModel
    {
        public string idString { get; set; }
        public string name { get; set; }
        public string img { get; set; }
        public string desc { get; set; }
        public List<ApiSchoolPlaneModel> planes { get; set; }
    }

    public class ApiSchoolPlaneModel : ApiPlaneAAAModel
    {
        [Display(Name = "价格")]
        public decimal price { get; set; }
        [Display(Name = "价格单位")]
        public string unit { get; set; }
        [Display(Name = "驾照类型")]
        public string type { get; set; }
    }


    // 买飞机
    public class ApiSalePlaneModel : ApiPlaneAAAModel
    {
        // 和 ApiPlaneAAAModel 一样
    }


    // 航空装备
    
    public class ApiGetSaleGoodsRequestModel
    {
        // type 取值说明:
        //  11 最新产品
        //  12 服饰
        //  13 装备
        //  14 航材
        //  15 套材
        public int type { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
    }

    public class ApiGetSaleGoodsModel
    {
        public int id { get; set; }
        public string idString { get; set; }
        public string name { get; set; }
        public decimal price { get; set; }
        public string unit { get; set; }
        public string img { get; set; }
        public string[] imgs { get; set; }
        public string desc { get; set; }
        public int good { get; set; }
        public int shared { get; set; }
        public int comments { get; set; }
        public int visited { get; set; }
    }


    // 商品 就是所有能下订单的，包括租飞机，学飞行，买飞机，航空装备，
    //  它们的唯一标示符是 id 字段

        // 商品 浏览量、收藏数、好评、分享 的增加
    public class ApiAddGoodCountModel
    {
        public int id { get; set; }
        public int good { get; set; }
        public int collection { get; set; }
        public int shared { get; set; }
        public int visited { get; set; }
    }


    // 提交报名表

    public class ApiSendEntryRequestModel
    {
        public string name { get; set; }
        public string phone { get; set; }
        public string idCard { get; set; }
        public string address { get; set; }
        public string model { get; set; } // 机型 
        public string typeName { get; set; } // 型号 or 学校类型
        public string start { get; set; } // 区间 开始
        public string end { get; set; } // 区间 结束
        public string desc { get; set; }
        public byte type { get; set; } // 0 为 买飞机 ，1 为 学飞行
        public string createdAt { get; set; } // 不用传入

    }


    // 提交 反馈意见
    public class ApiSendFeedbackModel
    {
        public string phone { get; set; }
        public string body { get; set; }
    }


    // 用户 获取 互动吧 列表
    public class ApiGetUserCommentsRequest
    {
        public int page { get; set; }
        public int pageSize { get; set; }
    }

    public class ApiGetUserCommentsModel
    {
        public int id { get; set; }
        public string idString { get; set; }
        public string userName { get; set; }
        public string userImg { get; set; }
        public string createdAt { get; set; }
        public string body { get; set; }
        public string[] imgs { get; set; }
        public List<ApiUserCommentReplyModel> replys { get; set; }
    }
        // 学姐学长在这里
    public class ApiGetWTFCommentsModel : ApiGetUserCommentsModel
    {
        public string name { get; set; }
        public byte gender { get; set; }
    }
        // 互动吧 回复
    public class ApiUserCommentReplyModel
    {
        public int id { get; set; }
        public string idString { get; set; }
        public string userFrom { get; set; }
        public string  userFromRandId { get; set; }
        public string userTo { get; set; }
        public string  userToRandId { get; set; }
        public string body { get; set; }
        public string createdAt { get; set; }
    }

    // 互动吧 新增
    public class ApiAddUserCommentRequest
    {
        public string phone { get; set; }
        public string imgs { get; set; } // 多图用 逗号(,) 分割
        public string body { get; set; }
    }
        // 学姐学长在这里
    public class ApiAddWTFCommentRequest : ApiAddUserCommentRequest
    {
        public string name { get; set; }
        public byte gender { get; set; }
    }

    // 互动吧 点赞
    public class ApiUserCommentGoodRequest
    {
        public int id { get; set; } // 那个啥的id
        public int good { get; set; } // 1 or -1
    }

    // 新增 回复
    public class ApiAddUserCommentReplyRequest
    {
        public int id { get; set; } // 上级id
        public string userFromRandId { get; set; }
        public string userToRandId { get; set; }
        public string body { get; set; }
    }


    // 提交 订单
    public class ApiAddOrderRequest
    {
        public string phone { get; set; }
        public int goodId { get; set; }
        public decimal price { get; set; }
        public int type { get; set; }
        // type 取值说明 ，表明 订单 类型
        //  1    学飞行
        //  2    买飞机
        //  3    租飞机
        //  4    航空装备
        public string desc { get; set; }
    }


    // 更新订单状态
    public class ApiChangeOrderStatusRequest
    {
        public int id { get; set; }
        public short status { get; set; }
        // status 
       //    0   待支付
        //   1   已支付
        //   2   订单取消
    }
}