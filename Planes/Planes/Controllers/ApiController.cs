using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Planes.Models;
using Planes.Tools;
using PagedList;
using System.Web.Script.Serialization;
using System.IO;

namespace Planes.Controllers
{
    public class ApiController : Controller
    {
        private planeEntities db = new planeEntities();

        public ApiController() 
        {
        }

        public ActionResult Test()
        {
            return View();
        }
        
        // 注册
        public JsonResult Regist(ApiRegistRqeustModel model)
        {
            if (Phone(model.phone))
                return E("手机号已注册");
            string img = "Users/default.png";
            if (model.headImg != null)
            {
                img = SaveBase64Img(model.headImg,"Users");
            }
            var user = new Users()
            {
                username = model.userName,
                phone = model.phone,
                password = MD5Tool.Encrypt(model.password),
                head_img = img,
                created_at = DateTime.Now,
                is_lock = 1,
                gender = true,
                level_id = 1,
                rand_id = RandId(6),
                reg_id = model.regId
            };
            db.Users.Add(user);
            db.SaveChanges();
            return S("注册成功", ReturnUser(user));

        }
        public JsonResult RegistJson(string json)
        {
            var serializer = new JavaScriptSerializer();
            var model = serializer.Deserialize<ApiRegistRqeustModel>(json);
            return Regist(model);
        }

        // 登陆
        public JsonResult Login(ApiLoginRequestModel model)
        {
            if (!Phone(model.phone)) return E("用户不存在");
            var user = db.Users.Where(x => x.phone == model.phone).First();
            if (user.password != MD5Tool.Encrypt(model.password)) return E("密码错误");
            return S("登陆成功", ReturnUser(user));
        }
        public JsonResult LoginJson(string json)
        {
            var serializer = new JavaScriptSerializer();
            var model = serializer.Deserialize<ApiLoginRequestModel>(json);
            return Login(model);
        }
        
        // 修改用户信息
        public JsonResult ModifyUserProfile(ApiModifyUserProfileRquestModel model)
        {
            if (!Phone(model.phone)) return E("用户不存在");
            var user = db.Users.Where(x => x.phone == model.phone).First();
            user.true_name = model.trueName;
            user.username = model.userName;
            user.id_card = model.idCard;
            user.gender = model.gender == 1;
            user.address = model.address;
            user.work_unit = model.workUnit;
            db.SaveChanges();
            return S("修改成功", ReturnUser(user));
        }
        public JsonResult ModifyUserProfileJson(string json)
        {
            var serializer = new JavaScriptSerializer();
            var model = serializer.Deserialize<ApiModifyUserProfileRquestModel>(json);
            return ModifyUserProfile(model);
        }
        
        // 用户修改头像 
            // 1、图片上传到服务器
                // base64
                // public JsonResult UploadImageBase64(ApiUploadImageBase64Model model)

                // http post 模拟表单
                // public JsonResult UploadImagePost(ApiUploadImagePostModel model)
            // 2、更改头像，将第一步返回的img 传入
        public JsonResult ModifyUserImg(string phone,string img)
        {
            var user = db.Users.Where(x => x.phone == phone).First();
            user.head_img = img;
            db.SaveChanges();
            return S("修改成功",ReturnUser(user));
        }

        // 用户修改密码
        public JsonResult UserChangePassword(ApiUserChangePasswordRequestModel model)
        {
            if (!Phone(model.phone)) return E("用户不存在");
            var user = db.Users.Where(x => x.phone == model.phone).First();
            user.password = MD5Tool.Encrypt(model.password);
            db.SaveChanges();
            return S("修改成功");
        }
        public JsonResult UserChangePasswordJson(string json)
        {
            var serializer = new JavaScriptSerializer();
            var model = serializer.Deserialize<ApiUserChangePasswordRequestModel>(json);
            return UserChangePassword(model);
        }

        // 获取动态列表
        public JsonResult SellerMessages(ApiSellerMessageRequestModel model)
        {
            var ms = db.SellerMessage.OrderByDescending(x => x.created_at).ToPagedList(model.page,model.pageSize);
            var list = new List<ApiSellerMessageModel>();
            foreach (var m in ms)
            {
                var air = db.Airports.Where(x => x.seller_id == m.Goods.seller_id && x.area_id == m.Goods.area_id).First();
                var loc = air.location.Split('|');
                ApiSellerMessageCommentModel ls = null;
                if (m.MessageComments.Count() > 0)
                {
                    var tmp = m.MessageComments.OrderByDescending(x => x.created_at).First();
                    ls = new ApiSellerMessageCommentModel()
                    {
                        id = tmp.id,
                        userName = tmp.Users.username,
                        userImg = tmp.Users.head_img,
                        body = tmp.body
                    };
                }
                list.Add(new ApiSellerMessageModel()
                {
                    title = m.Goods.name,
                    sellerName = m.Sellers.name,
                    summary = m.content,
                    lat = double.Parse(loc[1]),
                    lng = double.Parse(loc[0]),
                    detail = m.Goods.desci,
                    img = (m.img),
                    good = m.good,
                    bad = m.MessageComments.Count(),
                    shared = m.shared,
                    id = m.id,
                    last = ls,
                    idString = m.id.ToString()
                });
            }
            return S("获取成功",list);
        }
        public JsonResult SellerMessagesJson(string json)
        {
            var serializer = new JavaScriptSerializer();
            var model = serializer.Deserialize<ApiSellerMessageRequestModel>(json);
            return SellerMessages(model);
        }

        // 增加动态点赞、差评、分享数，注意是增加！如有用户点赞，则传值:good=1,bad=0,shared=0
        public JsonResult AddSellerMessagesCount(ApiAddSellerMessagesCountModel model)
        {
            var tmp = db.SellerMessage.Where(x => x.id == model.id).First();
            tmp.good += model.good;
            tmp.bad += model.bad;
            tmp.shared += model.shared;
            db.SaveChanges();
            return S("成功");
        }
        public JsonResult AddSellerMessagesCountJson(string json)
        {
            var serializer = new JavaScriptSerializer();
            var model = serializer.Deserialize<ApiAddSellerMessagesCountModel>(json);
            return AddSellerMessagesCount(model);
        }

        // 新增动态评论
            // 第一步：上传图片
                // base64
        public JsonResult UploadImageBase64(ApiUploadImageBase64Model model)
        {
            if (!Phone(model.phone)) return E("用户不存在");
            string path = Server.MapPath("~/Images/Users/" + model.phone);
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
            return S("上传图片成功", new { img = (SaveBase64Img(model.code, "Users/" + model.phone)) });

        }
        public JsonResult UploadImageBase64Json(string json)
        {
            var serializer = new JavaScriptSerializer();
            var model = serializer.Deserialize<ApiUploadImageBase64Model>(json);
            return UploadImageBase64(model);
        }
                // http post 模拟表单
        public JsonResult UploadImagePost(ApiUploadImagePostModel model)
        {
            if (!Phone(model.phone)) return E("用户不存在");
            string path = Server.MapPath("~/Images/Users/" + model.phone);
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
            return S("上传图片成功", new { img = FileTool.Save(model.img, "Images/Users/" + model.phone) });
        }

            // 第二步：上传用户phone，文字内容
        public JsonResult AddSellerMessageComment(ApiAddSellerMessageCommentModel model)
        {
            if (!Phone(model.phone)) return E("用户不存在");
            var tmp = db.MessageComments.Add(new MessageComments() { 
                user_id = db.Users.Where(x => x.phone == model.phone).Select(x => x.user_id).First(),
                body = model.body,
                is_lock = (short)1,
                created_at = DateTime.Now,
                message_id = model.id,
                praised = 0,
                replys = 0,
                title = model.phone 
            });
            if (!string.IsNullOrEmpty(model.imgs))
            {
                foreach (var s in model.imgs.Split(','))
                {
                    db.MessageCommentImages.Add(new MessageCommentImages()
                    {
                        message_id = tmp.id,
                        img = s
                    });
                }
            }
            db.SaveChanges();
            return S("评论成功");
        }
        public JsonResult AddSellerMessageCommentJson(string json)
        {
            var serializer = new JavaScriptSerializer();
            var model = serializer.Deserialize<ApiAddSellerMessageCommentModel>(json);
            return AddSellerMessageComment(model);
        }

        // 获取动态评论
        public JsonResult SellerMessageComments(ApiSellerMessageCommentsRequestModel model)
        {
            var ms = db.MessageComments.Where(x => x.message_id == model.id).OrderByDescending(x => x.created_at);
            var list = new List<ApiSellerMessageCommentModel>();
            foreach (var m in ms)
            {
                var res = m.MessageCommentReplys.OrderByDescending(x => x.created_at).Select(x => 
                {
                    return new ApiSellerMessageCommentReplyModel() 
                    {
                        userName = x.Users.username,
                        body = x.body,
                        id = x.parent_id,
                        time = DateTimeToTimestamp(x.created_at),
                        img = x.Users.head_img
                    }; 
                });
                var ims = m.MessageCommentImages.Select(x => (x.img));
                var ls = new List<ApiSellerMessageCommentReplyModel>();
                list.Add(new ApiSellerMessageCommentModel() { 
                    userName = m.Users.username,
                    userImg = m.Users.head_img,
                    createdAt = m.created_at.ToLocalTime().ToString(),
                    body = m.body,
                    imgs = ims.ToArray(),
                    good = m.praised,
                    replys = res.ToList(),
                    id = m.id,
                    idString = m.id.ToString()
                });
            }
            return S("获取成功",list);
        }
        public JsonResult SellerMessageCommentsJson(string json)
        {
            var serializer = new JavaScriptSerializer();
            var model = serializer.Deserialize<ApiSellerMessageCommentsRequestModel>(json);
            return SellerMessageComments(model);
        }

        // 新增动态评论的回复
        public JsonResult AddSellerMessageCommentReply(ApiSellerMessageCommentReplyRequestModel model)
        {
           var user = db.Users.Where(x => x.phone == model.phone).First();
           var tmp = db.MessageCommentReplys.Add(new MessageCommentReplys() { 
                parent_id = model.id,
                user_id = user.user_id,
                body = model.body,
                is_lock = 1,
                praised = 0,
                created_at = DateTime.Now
            });
            db.SaveChanges();
            return S("回复成功", new ApiSellerMessageCommentReplyModel() { 
                id = model.id,
                body = model.body,
                userName = user.username,
                img = user.head_img,
                time = DateTimeToTimestamp(DateTime.Now)
            });
        }
        public JsonResult AddSellerMessageCommentReplyJson(string json)
        {
            var serializer = new JavaScriptSerializer();
            var model = serializer.Deserialize<ApiSellerMessageCommentReplyRequestModel>(json);
            return AddSellerMessageCommentReply(model);
        }

        // 动态评论点赞
        public JsonResult AddSellerMessageCommentGood(ApiAddSellerMessageCommentGoodModel model)
        {
            var tmp = db.MessageComments.Where(x => x.id == model.id).First();
            tmp.praised += model.good;
            db.SaveChanges();
            return S("点赞成功");
        }
        public JsonResult AddSellerMessageCommentGoodJson(string json)
        {
            var serializer = new JavaScriptSerializer();
            var model = serializer.Deserialize<ApiAddSellerMessageCommentGoodModel>(json);
            return AddSellerMessageCommentGood(model);
        }



        // 租飞机

            // 获取 预定航程 机场
        public JsonResult GetAreas(ApiGetAreasRequestModel model)
        {
            var tmp = db.Areas.Where(x => x.Goods.Count > 0).ToList().Select(x => new ApiGetAreasModel()
            {
                id = x.area_id,
                dis = (GetDistance(x.location, model.lat, model.lng)),
                idString = x.area_id.ToString(),
                name = x.name,
                address = x.address,
                lat = x.location.Split('|')[1],
                lng = x.location.Split('|')[0],
                img = x.img,
                priceMax = x.Goods.Where(sx => sx.type_id == 8 && CanToDecimal(sx.col7)).Count() > 0 ? x.Goods.Where(sx => sx.type_id == 8 && CanToDecimal(sx.col7)).Max(sx => decimal.Parse(sx.col7)) : 0,
                priceMin = x.Goods.Where(sx => sx.type_id == 8 && CanToDecimal(sx.col7)).Count() > 0 ? x.Goods.Where(sx => sx.type_id == 8 && CanToDecimal(sx.col7)).Min(sx => decimal.Parse(sx.col7)) : 0,
                //priceMax = x.Goods.Where(sx => sx.type_id == 8 && CanToDecimal(sx.col7)).Max(sx => decimal.Parse(sx.col7)),
                //priceMin = x.Goods.Where(sx => sx.type_id == 8 && CanToDecimal(sx.col7)).Min(sx => decimal.Parse(sx.col7)),
                sellers = x.Airports.Where(xx => xx.Sellers.group_id == 3).Select(xx => new ApiSellerModel()
                {
                    id = xx.seller_id,
                    idString = xx.seller_id.ToString(),
                    name = xx.Sellers.name,
                    img = xx.Sellers.img,
                    address = xx.address,
                    desc = xx.Sellers.desci,
                    planes = x.Goods.Where(s => s.type_id == 8 && s.seller_id == xx.seller_id).Select(xxx => new ApiPlaneAAAModel() 
                    { 
                        id = xxx.good_id,
                        idString = xxx.good_id.ToString(),
                        name = xxx.name,
                        model = xxx.model,
                        marketPrice = xxx.market_price ?? 0,
                        img = xxx.img,
                        col1 = xxx.col1,
                        col2 = xxx.col2,
                        col3 = xxx.col3,
                        col4 = xxx.col4,
                        col6 = xxx.col6,
                        col7 = xxx.col7,
                        col8 = xxx.col8,
                        desci = xxx.desci,
                        saled = xxx.saled ?? 0,
                        comments = xxx.comments ?? 0,
                        collection = xxx.collected ?? 0,
                        visited = xxx.visited ?? 0,
                        imgs = xxx.GoodImages.Select(xxxx => xxxx.img).ToArray()
                    }).ToList()
                }).ToList()
            }).OrderBy(x => x.dis).ToPagedList(model.page, model.pageSize);

            return S("获取成功", tmp);
        }
        public JsonResult GetAreasJson(string json)
        {
            var serializer = new JavaScriptSerializer();
            var model = serializer.Deserialize<ApiGetAreasRequestModel>(json);
            return GetAreas(model);
        }


            // 获取 预定航程 航空公司
        public JsonResult GetSellers(ApiGetSellersRequestModel model)
        {
            var tmp = db.Sellers.ToList().Where(x => x.group_id == 3).Select(x => new ApiGetSellersModel() 
            {
                id = x.seller_id,
                idString = x.seller_id.ToString(),
                name = x.name,
                img = x.img,
                lat = x.Airports.First().location.Split('|')[1],
                lng = x.Airports.First().location.Split('|')[0],
                address = x.Airports.First().address,
                desc = x.desci,
                priceMin = GetPlaneAMin(x.Goods.ToList().Where(s => s.area_id == x.Airports.First().area_id)),
                priceMax = GetPlaneAMax(x.Goods.ToList().Where(s => s.area_id == x.Airports.First().area_id)),
                dis = GetDistance(x.Airports.First().location,model.lat,model.lng),
                planes = x.Goods.Where(s => s.type_id == 8 && s.area_id == x.Airports.First().area_id).Select(xxx => new ApiPlaneAAAModel()
                {
                    id = xxx.good_id,
                    idString = xxx.good_id.ToString(),
                    name = xxx.name,
                    model = xxx.model,
                    marketPrice = xxx.market_price ?? 0,
                    img = xxx.img,
                    col1 = xxx.col1,
                    col2 = xxx.col2,
                    col3 = xxx.col3,
                    col4 = xxx.col4,
                    col6 = xxx.col6,
                    col7 = xxx.col7,
                    col8 = xxx.col8,
                    desci = xxx.desci,
                    saled = xxx.saled ?? 0,
                    comments = xxx.comments ?? 0,
                    collection = xxx.collected ?? 0,
                    visited = xxx.visited ?? 0,
                    imgs = xxx.GoodImages.Select(xxxx => xxxx.img).ToArray()
                }).ToList()
            }).OrderBy(x => x.dis).ToPagedList(model.page,model.pageSize);

            return S("获取成功",tmp);
        }
        public JsonResult GetSellersJson(string json)
        {
            var serializer = new JavaScriptSerializer();
            var model = serializer.Deserialize<ApiGetSellersRequestModel>(json);
            return GetSellers(model);
        }

            // 获取 特价包机
        public JsonResult GetPlaneBBB(ApiGetPlaneBBBRequestModel model)
        {
            var tmp = db.Goods.ToList().Where(x => x.type_id == 7).Select(x => new ApiGetPlaneCCCModel()
            { 
                id = x.good_id,
                idString = x.good_id.ToString(),
                col1 = x.col1,
                col2 = x.col2,
                model = x.model,
                col4 = x.col4,
                col3 = x.col3,
                col5 = x.col5,
                col6 = x.col6,
                price = x.price ?? 0,
                unit = x.unit,
                desc = x.desci,
                name = x.name,
                imgs = x.GoodImages.Select(xx => xx.img).ToArray(),
                dis = GetDistance(x.Areas.location, model.lat, model.lng),
                lat = x.Areas.location.Split('|')[1],
                lng = x.Areas.location.Split('|')[0]
            }).OrderBy(x => x.dis).ToPagedList(model.page,model.pageSize);

            return S("获取成功",tmp);
        }
        public JsonResult GetPlaneBBBJson(string json)
        {
            var serializer = new JavaScriptSerializer();
            var model = serializer.Deserialize<ApiGetPlaneBBBRequestModel>(json);
            return GetPlaneBBB(model);
        }

            // 获取 飞的航线
        public JsonResult GetPlaneCCC(ApiGetPlaneBBBRequestModel model)
        {
            var tmp = db.Goods.ToList().Where(x => x.type_id == 6).Select(x => new ApiGetPlaneBBBModel()
            {
                id = x.good_id,
                idString = x.good_id.ToString(),
                col1 = x.col1,
                col2 = x.col2,
                model = x.model,
                col4 = x.col4,
                col3 = x.col3,
                price = x.price ?? 0,
                unit = x.unit,
                desc = x.desci,
                name = x.name,
                imgs = x.GoodImages.Select(xx => xx.img).ToArray(),
                
                dis = GetDistance(x.Areas.location, model.lat, model.lng),
                lat = x.Areas.location.Split('|')[1],
                lng = x.Areas.location.Split('|')[0]
            }).OrderBy(x => x.dis).ToPagedList(model.page, model.pageSize);

            return S("获取成功", tmp);
        }
        public JsonResult GetPlaneCCCJson(string json)
        {
            var serializer = new JavaScriptSerializer();
            var model = serializer.Deserialize<ApiGetPlaneBBBRequestModel>(json);
            return GetPlaneCCC(model);
        }



        // 获取 广告
        public JsonResult GetAds()
        {
            var tmp = db.Ads.ToList().Select(x => new ApiAdModel()
            {
                name = x.name,
                link = x.link,
                desc = x.desci,
                img = x.img,
                position = x.position,
                type = GetAdsType(x),
                good = GetAdsGood(x.good_id)
            });
            return S("获取成功",tmp);
        }
        private int GetAdsType(Ads x)
        {
            if (x.type == 0) return 0;
            var sql = db.Goods.Where(s => s.good_id == x.good_id);
            if (sql.Count() == 0) return 0;
            var good = sql.First();
            if (good.type_id == 9) return 1;
            return 2;
        }
        private object GetAdsGood(int? id)
        {
            if (id == null) return null;
            var sql = db.Goods.Where(x => x.good_id == id.Value);
            if (sql.Count() == 0) return null;
            var good = sql.First();
            if (good.type_id == 9) return new ApiSalePlaneModel()
            {
                id = good.good_id,
                idString = good.good_id.ToString(),
                name = good.name,
                model = good.model,
                marketPrice = good.market_price ?? 0,
                img = good.img,
                col1 = good.col1,
                col2 = good.col2,
                col3 = good.col3,
                col4 = good.col4,
                col6 = good.col6,
                col7 = good.col7,
                col8 = good.col8,
                desci = good.desci,
                imgs = good.GoodImages.Select(xx => xx.img).ToArray(),
                saled = good.saled ?? 0,
                comments = good.comments ?? 0,
                collection = good.collected ?? 0,
                visited = good.visited ?? 0,
                shared = good.shared ?? 0
            };
            return new ApiGetSaleGoodsModel()
            {
                name = good.name,
                price = good.price ?? 0,
                unit = good.unit,
                img = good.img,
                imgs = good.GoodImages.Select(xx => xx.img).ToArray(),
                desc = good.desci,
                good = good.collected ?? 0,
                shared = good.shared ?? 0,
                comments = good.comments ?? 0,
                visited = good.visited ?? 0,
                id = good.good_id,
                idString = good.good_id.ToString()
            };
        }

        // 学飞行
            // 国内航校
        public JsonResult GetSchoolA(ApiGetSchoolRequestModel model)
        {
            var tmp = db.Sellers.ToList().Where(x => x.group_id == 5).Select(x => new ApiGetSchoolModel()
            {
                id = x.seller_id,
                idString = x.seller_id.ToString(),
                name = x.name,
                img = x.img,
                dis = GetDistance(x.Airports.First().location, model.lat, model.lng),
                lat = x.Airports.First().location.Split('|')[1],
                lng = x.Airports.First().location.Split('|')[0],
                address = x.Airports.First().address,
                desc = x.desci,
                priceMax = x.Goods.Max(sx => sx.price).Value,
                priceMin = x.Goods.Min(sx => sx.price).Value,
                planes = x.Goods.Select(xxx => new ApiSchoolPlaneModel()
                {
                    id = xxx.good_id,
                    idString = xxx.good_id.ToString(),
                    name = xxx.name,
                    model = xxx.model,
                    marketPrice = xxx.market_price ?? 0,
                    img = xxx.img,
                    col1 = xxx.col1,
                    col2 = xxx.col2,
                    col3 = xxx.col3,
                    col4 = xxx.col4,
                    col6 = xxx.col6,
                    col7 = xxx.col7,
                    col8 = xxx.col8,
                    desci = xxx.desci,
                    saled = xxx.saled ?? 0,
                    comments = xxx.comments ?? 0,
                    collection = xxx.collected ?? 0,
                    visited = xxx.visited ?? 0,
                    imgs = xxx.GoodImages.Select(xxxx => xxxx.img).ToArray(),
                    price = xxx.price ?? 0,
                    unit = xxx.unit,
                    type = xxx.GoodTypes.name
                }).ToList()
            }).OrderBy(x => x.dis).ToPagedList(model.page, model.pageSize);

            return S("获取成功", tmp);
        }
        public JsonResult GetSchoolAJson(string json)
        {
            var serializer = new JavaScriptSerializer();
            var model = serializer.Deserialize<ApiGetSchoolRequestModel>(json);
            return GetSchoolA(model);
        }

            // 国外航校
        public JsonResult GetSchoolB(ApiGetSchoolRequestModel model)
        {
            var tmp = db.Sellers.ToList().Where(x => x.group_id == 4).Select(x => new ApiGetSchoolModel()
            {
                id = x.seller_id,
                idString = x.seller_id.ToString(),
                name = x.name,
                img = x.img,
                dis = GetDistance(x.Airports.First().location, model.lat, model.lng),
                lat = x.Airports.First().location.Split('|')[1],
                lng = x.Airports.First().location.Split('|')[0],
                address = x.Airports.First().address,
                desc = x.desci,
                priceMax = x.Goods.Max(sx => sx.price).Value,
                priceMin = x.Goods.Min(sx => sx.price).Value,
                planes = x.Goods.Select(xxx => new ApiSchoolPlaneModel()
                {
                    id = xxx.good_id,
                    idString = xxx.good_id.ToString(),
                    name = xxx.name,
                    model = xxx.model,
                    marketPrice = xxx.market_price ?? 0,
                    img = xxx.img,
                    col1 = xxx.col1,
                    col2 = xxx.col2,
                    col3 = xxx.col3,
                    col4 = xxx.col4,
                    col6 = xxx.col6,
                    col7 = xxx.col7,
                    col8 = xxx.col8,
                    desci = xxx.desci,
                    saled = xxx.saled ?? 0,
                    comments = xxx.comments ?? 0,
                    collection = xxx.collected ?? 0,
                    visited = xxx.visited ?? 0,
                    imgs = xxx.GoodImages.Select(xxxx => xxxx.img).ToArray(),
                    price = xxx.price ?? 0,
                    unit = xxx.unit,
                    type = xxx.GoodTypes.name
                }).ToList()
            }).OrderBy(x => x.dis).ToPagedList(model.page, model.pageSize);

            return S("获取成功", tmp);
        }
        public JsonResult GetSchoolBJson(string json)
        {
            var serializer = new JavaScriptSerializer();
            var model = serializer.Deserialize<ApiGetSchoolRequestModel>(json);
            return GetSchoolB(model);
        }



        // 买飞机
        public JsonResult GetSalePlanes()
        {
            var tmp = db.Goods.ToList().Where(x => x.type_id == 9).Select(x => new ApiSalePlaneModel() { 
                id = x.good_id,
                idString = x.good_id.ToString(),
                name = x.name,
                model = x.model,
                marketPrice = x.market_price ?? 0,
                img = x.img,
                col1 = x.col1,
                col2 = x.col2,
                col3 = x.col3,
                col4 = x.col4,
                col6 = x.col6,
                col7 = x.col7,
                col8 = x.col8,
                desci = x.desci,
                imgs = x.GoodImages.Select(xx => xx.img).ToArray(),
                saled = x.saled ?? 0,
                comments = x.comments ?? 0,
                collection = x.collected ?? 0,
                visited = x.visited ?? 0,
                shared = x.shared ?? 0
            });

            return S("获取成功",tmp);
        }


        // 航空装备
        public JsonResult GetSaleGoods(ApiGetSaleGoodsRequestModel model)
        {
            var tmp = db.Goods.ToList().Where(x => x.type_id == model.type).Select(x => new ApiGetSaleGoodsModel() { 
                name = x.name,
                price = x.price ?? 0,
                unit = x.unit,
                img = x.img,
                imgs = x.GoodImages.Select(xx => xx.img).ToArray(),
                desc = x.desci,
                good = x.collected ?? 0,
                shared = x.shared ?? 0,
                comments = x.comments ?? 0,
                visited = x.visited ?? 0,
                id = x.good_id,
                idString = x.good_id.ToString()
            }).OrderBy(x => x.id).ToPagedList(model.page,model.pageSize);

            return S("获取成功",tmp);
        }
        public JsonResult GetSaleGoodsJson(string json)
        {
            var serializer = new JavaScriptSerializer();
            var model = serializer.Deserialize<ApiGetSaleGoodsRequestModel>(json);
            return GetSaleGoods(model);
        }


        // 商品 就是所有能下订单的，包括租飞机，学飞行，买飞机，航空装备，
        //  它们的唯一标示符是 id 字段
            
            // 商品 浏览量、收藏数、好评、分享 的增加
        public JsonResult AddGoodCount(ApiAddGoodCountModel model)
        {
            var tmp = db.Goods.Where(x => x.good_id == model.id).First();
            tmp.priased += model.good;
            tmp.collected += model.collection;
            tmp.shared += model.shared;
            tmp.visited += model.visited;
            db.SaveChanges();
            return S("提交成功");
        }
        public JsonResult AddGoodCountJson(string json)
        {
            var serializer = new JavaScriptSerializer();
            var model = serializer.Deserialize<ApiAddGoodCountModel>(json);
            return AddGoodCount(model);

        }

            // 获取商品评论
                // 所有字段与 动态评论 的一样
        public JsonResult GetGoodComments(ApiSellerMessageCommentsRequestModel model)
        {
            var ms = db.GoodComments.Where(x => x.good_id == model.id).OrderByDescending(x => x.created_at);
            var list = new List<ApiSellerMessageCommentModel>();
            foreach (var m in ms)
            {
                var res = m.GoodCommentReplys.OrderByDescending(x => x.created_at).Select(x => 
                { 
                    return new ApiSellerMessageCommentReplyModel() 
                    { 
                        userName = x.Users.username, 
                        body = x.body,
                        id = x.parent_id,
                        time = DateTimeToTimestamp(x.created_at),
                        img = x.Users.head_img
                    }; 
                });
                var ims = m.GoodCommentImages.Select(x => (x.img));
                var ls = new List<ApiSellerMessageCommentReplyModel>();
                list.Add(new ApiSellerMessageCommentModel()
                {
                    userName = m.Users.username,
                    userImg = m.Users.head_img,
                    createdAt = m.created_at.ToLocalTime().ToString(),
                    body = m.body,
                    imgs = ims.ToArray(),
                    good = m.praised,
                    replys = res.ToList(),
                    id = m.id,
                    idString = m.id.ToString()
                });
            }
            return S("获取成功", list);
        }
        public JsonResult GetGoodCommentsJson(string json)
        {
            var serializer = new JavaScriptSerializer();
            var model = serializer.Deserialize<ApiSellerMessageCommentsRequestModel>(json);
            return GetGoodComments(model);
        }


            // 新增商品评论
                // 第一步：上传图片
                // base64
       // public JsonResult UploadImageBase64(ApiUploadImageBase64Model model)

                // http post 模拟表单
       // public JsonResult UploadImagePost(ApiUploadImagePostModel model)
                
                // 第二步：上传用户phone，文字内容
        public JsonResult AddGoodComment(ApiAddSellerMessageCommentModel model)
        {
            if (!Phone(model.phone)) return E("用户不存在");
            var good = db.Goods.Where(x => x.good_id == model.id).First();
            good.comments++;
            var tmp = db.GoodComments.Add(new GoodComments()
            {
                user_id = db.Users.Where(x => x.phone == model.phone).Select(x => x.user_id).First(),
                body = model.body,
                is_lock = 1,
                created_at = DateTime.Now,
                good_id = model.id,
                praised = 0,
                replys = 0,
                title = model.phone
            });
            if (!string.IsNullOrEmpty(model.imgs))
            {
                foreach (var s in model.imgs.Split(','))
                {
                    db.GoodCommentImages.Add(new GoodCommentImages()
                    {
                        comment_id = tmp.id,
                        img = s
                    });
                }
            }
            db.SaveChanges();
            return S("评论成功");
        }
        public JsonResult AddGoodCommentJson(string json)
        {
            var serializer = new JavaScriptSerializer();
            var model = serializer.Deserialize<ApiAddSellerMessageCommentModel>(json);
            return AddGoodComment(model);
        }

            // 新增 商品评论 回复
        public JsonResult AddGoodCommentReply(ApiSellerMessageCommentReplyRequestModel model)
        {
            var user = db.Users.Where(x => x.phone == model.phone).First();
            var tmp = db.GoodCommentReplys.Add(new GoodCommentReplys()
            {
                parent_id = model.id,
                user_id = user.user_id,
                body = model.body,
                is_lock = 1,
                praised = 0,
                created_at = DateTime.Now
            });
            db.SaveChanges();
            return S("回复成功", new ApiSellerMessageCommentReplyModel()
            {
                id = model.id,
                body = model.body,
                userName = user.username,
                img = user.head_img,
                time = DateTimeToTimestamp(DateTime.Now)
            });
        }
        public JsonResult AddGoodCommentReplyJson(string json)
        {
            var serializer = new JavaScriptSerializer();
            var model = serializer.Deserialize<ApiSellerMessageCommentReplyRequestModel>(json);
            return AddGoodCommentReply(model);
        }

        // 商品评论 点赞
        public JsonResult AddGoodCommentCount(ApiAddCommentCountRequest model)
        {
            var tmp = db.GoodComments.Where(x => x.id == model.id).First();
            tmp.praised += model.good;
            db.SaveChanges();
            return S("点赞成功");
        }

        // 提交报名表
            
        public JsonResult SendEntry(ApiSendEntryRequestModel ms)
        {
            db.Entrys.Add(new Entrys() { 
                name = ms.name,
                phone = ms.phone,
                id_card = ms.idCard,
                address = ms.address,
                model = ms.model,
                typename = ms.typeName,
                start = ms.start,
                end = ms.end,
                desci = ms.desc,
                type = ms.type,
                created_at = DateTime.Now,
                user_id = db.Users.Where(x => x.phone == ms.phone).First().user_id
            });
            db.SaveChanges();
            return S("提交成功");
        }
         public JsonResult SendEntryJson(string json)
        {
            var serializer = new JavaScriptSerializer();
            var model = serializer.Deserialize<ApiSendEntryRequestModel>(json);
            return SendEntry(model);
        }
        
        // 获取 报名表
        public JsonResult GetEntrys(string phone)
        {
            var tmp = db.Entrys.ToList().Where(x => x.phone == phone).Select(x => new ApiSendEntryRequestModel() { 
                name = x.name,
                phone = x.phone,
                idCard = x.id_card,
                address = x.address,
                model = x.model,
                typeName = x.typename,
                type = x.type,
                start = x.start,
                end = x.end,
                desc = x.desci,
                createdAt = x.created_at.ToLocalTime().ToString()
            });

            return S("获取成功",tmp);
        }


        // 提交 反馈意见

        public JsonResult SendFeedback(ApiSendFeedbackModel model)
        {
            db.Feedback.Add(new Feedback() { 
                user_id = db.Users.Where(x => x.phone == model.phone).First().user_id,
                body = model.body
            });
            db.SaveChanges();
            return S("提交成功");
        }
        public JsonResult SendFeedbackJson(string json)
        {
            var serializer = new JavaScriptSerializer();
            var model = serializer.Deserialize<ApiSendFeedbackModel>(json);
            return SendFeedback(model);
        }


        // 用户 获取 互动吧
        public JsonResult GetUserComments(ApiGetUserCommentsRequest model)
        {
            var tmp = db.UserComments.ToList().Where(x => x.type == 0).OrderByDescending(x => x.created_at).Select(x => new ApiGetUserCommentsModel()
            {
                id = x.id,
                idString = x.id.ToString(),
                userName = x.Users.username,
                userImg = x.Users.head_img,
                createdAt = x.created_at.ToLocalTime().ToString(),
                body = x.body,
                good = x.good,
                userRandId = x.Users.rand_id,
                imgs = x.UserCommentImages.Select(xx => xx.img).ToArray(),
                replys = x.UserCommentReplys.Select(xx => new ApiUserCommentReplyModel() { 
                    id = xx.id,
                    idString = xx.id.ToString(),
                    userFrom = xx.Users.username,
                    userFromRandId = xx.Users.rand_id,
                    userTo = xx.Users1.username,
                    userToRandId = xx.Users1.rand_id,
                    body = xx.body,
                    createdAt = xx.created_at.ToString("y-M-d H:m:s"),
                    time = DateTimeToTimestamp(xx.created_at),
                    userFromImg = xx.Users.head_img,
                    userToImg = xx.Users1.head_img
                }).ToList()
            }).ToPagedList(model.page,model.pageSize);

            return S("获取成功",tmp);
        }


        // 互动吧 新增
            // 第一步：上传图片
            // base64
        // public JsonResult UploadImageBase64(ApiUploadImageBase64Model model)

            // http post 模拟表单
        // public JsonResult UploadImagePost(ApiUploadImagePostModel model)
            
        
            // 第二步：上传用户phone，文字内容

        public JsonResult AddUserComment(ApiAddUserCommentRequest model)
        {
            var tmp = db.UserComments.Add(new UserComments() { 
                user_id = db.Users.Where(x => x.phone == model.phone).Select(x => x.user_id).First(),
                body = model.body,
                created_at = DateTime.Now,
                name = "null",
                good = 0,
                type = 0
            });
            if (!string.IsNullOrEmpty(model.imgs))
            {
                foreach (var s in model.imgs.Split(','))
                {
                    db.UserCommentImages.Add(new UserCommentImages()
                    {
                        parent_id = tmp.id,
                        img = s
                    });
                }
            }
            db.SaveChanges();
            return S("发布成功");
        }


        // 互动吧 点赞
        public JsonResult AddUserCommentGood(ApiUserCommentGoodRequest model)
        {
            var tmp = db.UserComments.Where(x => x.id == model.id).First();
            tmp.good += model.good;
            db.SaveChanges();
            return S("点赞成功");
        }

        // 新增 回复
        public JsonResult AddUserCommentReply(ApiAddUserCommentReplyRequest model)
        {
            var tmps = db.UserCommentReplys.Add(new UserCommentReplys() { 
                body = model.body,
                parent_id = model.id,
                user1_id = db.Users.Where(x => x.rand_id == model.userFromRandId).Select(x => x.user_id).First(),
                user2_id = db.Users.Where(x => x.rand_id == model.userToRandId).Select(x => x.user_id).First(),
                created_at = DateTime.Now
            });
            db.SaveChanges();
            var user1 = db.Users.Where(x => x.user_id == tmps.user1_id).First();
            var user2 = db.Users.Where(x => x.user_id == tmps.user2_id).First();
            if (user2.reg_id != null)
                Push.PushToId(user2.reg_id,model.body);
            var tmp = db.UserCommentReplys.ToList().Where(x => x.id == tmps.id).Select(x => new ApiUserCommentReplyModel()
            {
                id = x.parent_id,
                idString = x.parent_id.ToString(),
                userFrom = user1.username,
                userFromImg = user1.head_img,
                userTo = user2.username,
                userToImg = user2.head_img,
                userFromRandId = user1.rand_id,
                userToRandId = user2.rand_id,
                body = x.body,
                createdAt = x.created_at.ToLocalTime().ToString(),
                time = DateTimeToTimestamp(x.created_at)
            }).First();
            return S("回复成功", tmp);
        }



        // 获取 学姐学长在这里
        public JsonResult GetWTFComments(ApiGetUserCommentsRequest model)
        {
            var tmp = db.UserComments.ToList().Where(x => x.type == 1).OrderByDescending(x => x.created_at).Select(x => new ApiGetWTFCommentsModel()
            {
                id = x.id,
                idString = x.id.ToString(),
                userName = x.Users.username,
                userImg = x.Users.head_img,
                createdAt = x.created_at.ToLocalTime().ToString(),
                body = x.body,
                name = x.name,
                gender = x.gender,
                good = x.good,
                userRandId = x.Users.rand_id,
                imgs = x.UserCommentImages.Select(xx => xx.img).ToArray(),
                replys = x.UserCommentReplys.Select(xx => new ApiUserCommentReplyModel()
                {
                    id = xx.id,
                    idString = xx.id.ToString(),
                    userFrom = xx.Users.username,
                    userFromRandId = xx.Users.rand_id,
                    userTo = xx.Users1.username,
                    userToRandId = xx.Users1.rand_id,
                    body = xx.body,
                    createdAt = xx.created_at.ToLocalTime().ToString(),
                    time = DateTimeToTimestamp(xx.created_at),
                    userFromImg = xx.Users.head_img,
                    userToImg = xx.Users1.head_img
                }).ToList()
            }).ToPagedList(model.page, model.pageSize);

            return S("获取成功", tmp);
        }


        // 学姐学长在这里 新增
        // 第一步：上传图片
        // base64
        // public JsonResult UploadImageBase64(ApiUploadImageBase64Model model)

        // http post 模拟表单
        // public JsonResult UploadImagePost(ApiUploadImagePostModel model)


        // 第二步：上传用户phone，文字内容

        public JsonResult AddWTFComment(ApiAddWTFCommentRequest model)
        {
            var user = db.Users.Where(x => x.phone == model.phone).First();
            var tmp = db.UserComments.Add(new UserComments()
            {
                user_id = user.user_id,
                body = model.body,
                created_at = DateTime.Now,
                name = user.true_name,
                good = 0,
                type = 1,
                gender = user.gender.Value ? (byte)1 : (byte)0
            });
            if (!string.IsNullOrEmpty(model.imgs))
            {
                foreach (var s in model.imgs.Split(','))
                {
                    db.UserCommentImages.Add(new UserCommentImages()
                    {
                        parent_id = tmp.id,
                        img = s
                    });
                }
            }
            db.SaveChanges();
            return S("发布成功");
        }


        // 学姐学长在这里 点赞
        public JsonResult AddWTFCommentGood(ApiUserCommentGoodRequest model)
        {
            var tmp = db.UserComments.Where(x => x.id == model.id).First();
            tmp.good += model.good;
            db.SaveChanges();
            return S("点赞成功");
        }

        // 新增 回复
        public JsonResult AddWTFCommentReply(ApiAddUserCommentReplyRequest model)
        {
            var tmps = db.UserCommentReplys.Add(new UserCommentReplys()
            {
                body = model.body,
                parent_id = model.id,
                user1_id = db.Users.Where(x => x.rand_id == model.userFromRandId).Select(x => x.user_id).First(),
                user2_id = db.Users.Where(x => x.rand_id == model.userToRandId).Select(x => x.user_id).First(),
                created_at = DateTime.Now
            });
            db.SaveChanges();
            var user1 = db.Users.Where(x => x.user_id == tmps.user1_id).First();
            var user2 = db.Users.Where(x => x.user_id == tmps.user2_id).First();
            if (user2.reg_id != null)
                Push.PushToId(user2.reg_id, model.body);
            var tmp = db.UserCommentReplys.ToList().Where(x => x.id == tmps.id).Select(x => new ApiUserCommentReplyModel()
            {
                id = x.parent_id,
                idString = x.parent_id.ToString(),
                userFrom = user1.username,
                userTo = user2.username,
                userFromRandId = user1.rand_id,
                userToRandId = user2.rand_id,
                body = x.body,
                userFromImg = user1.head_img,
                userToImg = user2.head_img,
                time = DateTimeToTimestamp(x.created_at),
                createdAt = x.created_at.ToLocalTime().ToString()
            }).First();
            return S("回复成功", tmp);
        }

        //  提交 订单
        public JsonResult AddOrder(ApiAddOrderRequest model)
        {
            if (!Phone(model.phone)) return E("用户不存在");
            var good = db.Goods.Where(x => x.good_id == model.goodId).First();
            var userId = db.Users.Where(x => x.phone == model.phone).Select(x => x.user_id).First();
            var order = new Orders();
            order.amount = model.price;
            order.created_at = DateTime.Now;
            order.good_id = good.good_id;
            order.status = 0;
            order.type_id = model.type;
            order.user_id = userId;
            order.price = model.amount;
            order.count = model.num;
            order.payType = 0;

            do
            {
                Random rand = new Random();
                order.orderId = DateTime.Now.ToString("yMdHmsfff") + userId;
            } while (db.Orders.Where(x => x.orderId == order.orderId).Count() > 0);
            
            switch (model.type)
            {
                case 1:
                    order.col2 = model.desc;
                    order.col1 = good.col11;
                    break;
                case 2:
                    order.col1 = model.desc;
                    break;
                case 3:
                    if (good.type_id == 8)
                    {
                        order.col1 = model.goTime;
                        order.col2 = model.backTime;
                        order.col3 = model.fromAdd;
                        order.col4 = model.count;
                        order.col5 = model.desc;
                        order.col6 = model.toAdd;
                        order.col7 = model.stayTime;
                        order.col8 = model.flyTime;
                    }
                    else
                    {
                        order.col1 = good.col3;
                        order.col2 = good.col11;
                        order.col3 = good.col1 + " - " + good.col2;
                        order.col4 = good.col4;
                        order.col5 = model.desc;
                    }
                    break;
                case 4:
                    order.col1 = model.desc;
                    break;
            }

            db.Orders.Add(order);
            db.SaveChanges();
            return S("提交成功", new { 
                orderId = order.orderId
            });
        }
        public JsonResult AddOrderJson(string json)
        {
            var serializer = new JavaScriptSerializer();
            var model = serializer.Deserialize<ApiAddOrderRequest>(json);
            return AddOrder(model);
        }

        // 更新订单状态
        public JsonResult ChangeOrderStatus(ApiChangeOrderStatusRequest model)
        {
            var tmp = db.Orders.Where(x => x.order_id == model.id).First();
            tmp.status = model.status;
            db.SaveChanges();
            return S("更改成功");
        }

        // 更改订单支付类型
        public JsonResult ChangeOrderPayType(ApiChangeOrderPayTypeRequest model)
        {
            var order = db.Orders.Where(x => x.orderId == model.orderId).First();
            order.payType = model.payType;
            db.SaveChanges();
            return S("更新成功");
        }

        // 获取用户订单
        public JsonResult UserOrders(ApiUserOrderRequestModel model)
        {
            if (!Phone(model.phone)) return E("用户不存在");
            var user = db.Users.Where(x => x.phone == model.phone).First();
            var os = db.Orders
                        .Where(x => x.user_id == user.user_id)
                        .OrderByDescending(x => x.created_at)
                        .ToPagedList(model.page, model.pageSize);
            var list = new List<ApiUserOrderModel>();
            string[] sss = { "待支付", "已支付", "订单取消" };
            foreach (var o in os)
            {
                list.Add(new ApiUserOrderModel()
                {
                    orderId = o.orderId,
                    createdAt = o.created_at.Value.ToLocalTime().ToString(),
                    price = o.amount ?? 0,
                    status = sss[o.status ?? 0],
                    name = o.Goods.name,
                    img = o.Goods.img,
                    payType = o.payType ?? 0,
                    type = o.type_id ?? 1
                });
            }
            return S("获取成功", list);
        }
        public JsonResult UserOrdersJson(string json)
        {
            var serializer = new JavaScriptSerializer();
            var model = serializer.Deserialize<ApiUserOrderRequestModel>(json);
            return UserOrders(model);
        }


        // 获取用户的评论
        public JsonResult GetUserCommentList(ApiGetUserCommentListRequest model)
        {
            var user = db.Users.Where(x => x.phone == model.phone).First();
            int cnt = model.page * model.pageSize;
            var cmt = db.MessageComments.ToList().Where(x => x.user_id == user.user_id).Select(x => new ApiGetUserCommentList() { 
                type = 1,
                commentId = x.id,
                body = x.body,
                username = user.username,
                img = user.head_img,
                imgs = db.MessageCommentImages.Where(xx => xx.message_id == x.id).Select(xx => xx.img).ToArray(),
                time = DateTimeToTimestamp(x.created_at)
            });
            if (cmt.Count() < cnt)
            {
                cmt = cmt.Concat(db.GoodComments.ToList().Where(x => x.user_id == user.user_id).Select(x => new ApiGetUserCommentList() { 
                    type = 2,
                    commentId = x.id,
                    body = x.body,
                    username = user.username,
                    img = user.head_img,
                    imgs = db.GoodCommentImages.Where(xx => xx.comment_id == x.id).Select(xx => xx.img).ToArray(),
                    time = DateTimeToTimestamp(x.created_at)
                }));
                if (cmt.Count() < cnt)
                {
                    cmt = cmt.Concat(db.UserComments.ToList().Where(x => x.user_id == user.user_id && x.type == 0).Select(x => new ApiGetUserCommentList() { 
                        type = 3,
                        commentId = x.id,
                        body = x.body,
                        username = user.username,
                        img = user.head_img,
                        imgs = db.UserCommentImages.Where(xx => xx.parent_id == x.id).Select(xx => xx.img).ToArray(),
                        time = DateTimeToTimestamp(x.created_at)
                    }));
                    if (cmt.Count() < cnt)
                    {
                        cmt = cmt.Concat(db.UserComments.ToList().Where(x => x.user_id == user.user_id && x.type == 1).Select(x => new ApiGetUserCommentList()
                        {
                            type = 4,
                            commentId = x.id,
                            body = x.body,
                            username = user.username,
                            img = user.head_img,
                            imgs = db.UserCommentImages.Where(xx => xx.parent_id == x.id).Select(xx => xx.img).ToArray(),
                            time = DateTimeToTimestamp(x.created_at)
                        }));
                    }
                }
            }

            return S("获取成功",cmt.ToPagedList(model.page,model.pageSize));
        }


        // 获取 用户评论 详情
        public JsonResult GetUserCommentDetail(ApiGetUserCommentDetailRequest model)
        {
            List<ApiGetUserCommentDetail> tmp = new List<ApiGetUserCommentDetail>();
            switch (model.type)
            {
                case 1:
                    tmp = db.MessageCommentReplys.ToList().Where(x => x.parent_id == model.commentId).Select(x => new ApiGetUserCommentDetail()
                    {
                        id = x.id,
                        idString = x.id.ToString(),
                        userFrom = x.Users.username,
                        userFromRandId = x.Users.rand_id,
                        userFromImg = x.Users.head_img,
                        body = x.body,
                        time = DateTimeToTimestamp(x.created_at),
                        createdAt = x.created_at.ToLocalTime().ToString()
                    }).ToList();
                    break;
                case 2:
                    tmp = db.GoodCommentReplys.ToList().Where(x => x.parent_id == model.commentId).Select(x => new ApiGetUserCommentDetail()
                    {
                        id = x.id,
                        idString = x.id.ToString(),
                        userFrom = x.Users.username,
                        userFromRandId = x.Users.rand_id,
                        userFromImg = x.Users.head_img,
                        body = x.body,
                        time = DateTimeToTimestamp(x.created_at),
                        createdAt = x.created_at.ToLocalTime().ToString()
                    }).ToList();
                    break;
                case 3:
                case 4:
                    tmp = db.UserCommentReplys.ToList().Where(x => x.parent_id == model.commentId).Select(x => new ApiGetUserCommentDetail()
                    {
                        id = x.id,
                        idString = x.id.ToString(),
                        userFrom = x.Users.username,
                        userFromRandId = x.Users.rand_id,
                        userFromImg = x.Users.head_img,
                        body = x.body,
                        time = DateTimeToTimestamp(x.created_at),
                        createdAt = x.created_at.ToLocalTime().ToString()
                    }).ToList();
                    break;

            }
            return S("获取成功",tmp);
        }

        // 获取 消息中心 
        public JsonResult GetPushMessages(ApiGetPushMessagesRequest model)
        {
            var tmp = db.PushMessage.ToList().OrderByDescending(x => x.created_at).Select(x => new ApiGetPushMessages()
            {
                title = x.title,
                body = x.body,
                createdAt = x.created_at.Value.ToLocalTime().ToString()
            }).ToPagedList(model.page,model.pageSize);

            return S("获取成功",tmp);
        }


        // 租飞机 搜索
        public JsonResult SearchPlaneA(ApiSearchPlaneARequest model)
        {
            if (model.type == 1)
            {
                var tmp = db.Areas.Where(x => x.name.Contains(model.key)).ToList().Select(x => new ApiGetAreasModel()
                {
                    id = x.area_id,
                    dis = 0,
                    idString = x.area_id.ToString(),
                    name = x.name,
                    address = x.address,
                    lat = x.location.Split('|')[1],
                    lng = x.location.Split('|')[0],
                    img = x.img,
                    sellers = x.Airports.ToList().Where(xx => xx.Sellers.group_id == 3).Select(xx => new ApiSellerModel()
                    {
                        id = xx.seller_id,
                        idString = xx.seller_id.ToString(),
                        name = xx.Sellers.name,
                        img = xx.Sellers.img,
                        address = xx.address,
                        planes = x.Goods.ToList().Where(s => s.type_id == 8 && s.seller_id == xx.seller_id && s.area_id == x.area_id).Select(xxx => new ApiPlaneAAAModel()
                        {
                            id = xxx.good_id,
                            idString = xxx.good_id.ToString(),
                            name = xxx.name,
                            model = xxx.model,
                            marketPrice = xxx.market_price ?? 0,
                            img = xxx.img,
                            col1 = xxx.col1,
                            col2 = xxx.col2,
                            col3 = xxx.col3,
                            col4 = xxx.col4,
                            col6 = xxx.col6,
                            col7 = xxx.col7,
                            col8 = xxx.col8,
                            desci = xxx.desci,
                            saled = xxx.saled ?? 0,
                            comments = xxx.comments ?? 0,
                            collection = xxx.collected ?? 0,
                            visited = xxx.visited ?? 0,
                            imgs = xxx.GoodImages.Select(xxxx => xxxx.img).ToArray()
                        }).ToList()
                    }).ToList()
                });

                return S("搜索成功",tmp);
            }
            else
            {
                var tmp = db.Sellers.ToList().Where(x => x.group_id == 3 && x.name.Contains(model.key)).Select(x => new ApiGetSellersModel()
                {
                    id = x.seller_id,
                    idString = x.seller_id.ToString(),
                    name = x.name,
                    img = x.img,
                    lat = x.Airports.First().location.Split('|')[1],
                    lng = x.Airports.First().location.Split('|')[0],
                    address = x.Airports.First().address,
                    dis = 0,
                    planes = x.Goods.ToList().Where(s => s.type_id == 8).Select(xxx => new ApiPlaneAAAModel()
                    {
                        id = xxx.good_id,
                        idString = xxx.good_id.ToString(),
                        name = xxx.name,
                        model = xxx.model,
                        marketPrice = xxx.market_price ?? 0,
                        img = xxx.img,
                        col1 = xxx.col1,
                        col2 = xxx.col2,
                        col3 = xxx.col3,
                        col4 = xxx.col4,
                        col6 = xxx.col6,
                        col7 = xxx.col7,
                        col8 = xxx.col8,
                        desci = xxx.desci,
                        saled = xxx.saled ?? 0,
                        comments = xxx.comments ?? 0,
                        collection = xxx.collected ?? 0,
                        visited = xxx.visited ?? 0,
                        imgs = xxx.GoodImages.Select(xxxx => xxxx.img).ToArray()
                    }).ToList()
                });

                return S("搜索成功",tmp);
            }
        }

        // 驾校 搜索
        public JsonResult SearchPlaneB(ApiSearchPlaneBRequest model)
        {
            if (model.type == 1)
            {
                var tmp = db.Sellers.ToList().Where(x => x.group_id == 5 && x.name.Contains(model.key)).Select(x => new ApiGetSchoolModel()
                {
                    id = x.seller_id,
                    idString = x.seller_id.ToString(),
                    name = x.name,
                    img = x.img,
                    dis = 0,
                    lat = x.Airports.First().location.Split('|')[1],
                    lng = x.Airports.First().location.Split('|')[0],
                    planes = x.Goods.Select(xxx => new ApiSchoolPlaneModel()
                    {
                        id = xxx.good_id,
                        idString = xxx.good_id.ToString(),
                        name = xxx.name,
                        model = xxx.model,
                        marketPrice = xxx.market_price ?? 0,
                        img = xxx.img,
                        col1 = xxx.col1,
                        col2 = xxx.col2,
                        col3 = xxx.col3,
                        col4 = xxx.col4,
                        col6 = xxx.col6,
                        col7 = xxx.col7,
                        col8 = xxx.col8,
                        desci = xxx.desci,
                        saled = xxx.saled ?? 0,
                        comments = xxx.comments ?? 0,
                        collection = xxx.collected ?? 0,
                        visited = xxx.visited ?? 0,
                        imgs = xxx.GoodImages.Select(xxxx => xxxx.img).ToArray(),
                        price = xxx.price ?? 0,
                        unit = xxx.unit,
                        type = xxx.GoodTypes.name
                    }).ToList()
                });

                return S("获取成功", tmp);
            }
            else
            {
                var tmp = db.Sellers.ToList().Where(x => x.group_id == 4 && x.name.Contains(model.key)).Select(x => new ApiGetSchoolModel()
                {
                    id = x.seller_id,
                    idString = x.seller_id.ToString(),
                    name = x.name,
                    img = x.img,
                    dis = 0,
                    lat = x.Airports.First().location.Split('|')[1],
                    lng = x.Airports.First().location.Split('|')[0],
                    planes = x.Goods.Select(xxx => new ApiSchoolPlaneModel()
                    {
                        id = xxx.good_id,
                        idString = xxx.good_id.ToString(),
                        name = xxx.name,
                        model = xxx.model,
                        marketPrice = xxx.market_price ?? 0,
                        img = xxx.img,
                        col1 = xxx.col1,
                        col2 = xxx.col2,
                        col3 = xxx.col3,
                        col4 = xxx.col4,
                        col6 = xxx.col6,
                        col7 = xxx.col7,
                        col8 = xxx.col8,
                        desci = xxx.desci,
                        saled = xxx.saled ?? 0,
                        comments = xxx.comments ?? 0,
                        collection = xxx.collected ?? 0,
                        visited = xxx.visited ?? 0,
                        imgs = xxx.GoodImages.Select(xxxx => xxxx.img).ToArray(),
                        price = xxx.price ?? 0,
                        unit = xxx.unit,
                        type = xxx.GoodTypes.name
                    }).ToList()
                });

                return S("获取成功", tmp);
            }
        }

        // 地图模式
            // 租飞机
        public JsonResult MapPlaneA(ApiMapPlaneRequest model)
        {
            var tmp = db.Sellers.ToList().Where(x => x.group_id == 3).Select(x => new ApiMapPlaneA()
            {
                lat = x.Airports.First().location.Split('|')[1],
                lng = x.Airports.First().location.Split('|')[0],
                dis = GetDistance(x.Airports.First().location, model.lat, model.lng),
                name = x.name,
                priceMin = GetPlaneAMin(x.Goods.ToList().Where(s => s.area_id == x.Airports.First().area_id)),
                priceMax = GetPlaneAMax(x.Goods.ToList().Where(s => s.area_id == x.Airports.First().area_id)),
                type = GetPlaneType(x.Goods),
                address = x.Airports.First().address.Substring(0, 3),
                seller = new ApiGetSellersModel() 
                {
                    id = x.seller_id,
                    idString = x.seller_id.ToString(),
                    name = x.name,
                    img = x.img,
                    lat = x.Airports.First().location.Split('|')[1],
                    lng = x.Airports.First().location.Split('|')[0],
                    address = x.Airports.First().address,
                    dis = GetDistance(x.Airports.First().location, model.lat, model.lng),
                    planes = x.Goods.Where(s => s.type_id == 8 && s.area_id == x.Airports.First().area_id).Select(xxx => new ApiPlaneAAAModel()
                    {
                        id = xxx.good_id,
                        idString = xxx.good_id.ToString(),
                        name = xxx.name,
                        model = xxx.model,
                        marketPrice = xxx.market_price ?? 0,
                        img = xxx.img,
                        col1 = xxx.col1,
                        col2 = xxx.col2,
                        col3 = xxx.col3,
                        col4 = xxx.col4,
                        col6 = xxx.col6,
                        col7 = xxx.col7,
                        col8 = xxx.col8,
                        desci = xxx.desci,
                        saled = xxx.saled ?? 0,
                        comments = xxx.comments ?? 0,
                        collection = xxx.collected ?? 0,
                        visited = xxx.visited ?? 0,
                        imgs = xxx.GoodImages.Select(xxxx => xxxx.img).ToArray()
                    }).ToList()
                }
            }).OrderBy(x => x.dis);

            return S("获取成功",tmp);
        }
        public decimal GetPlaneAMax(IEnumerable<Goods> goods)
        {
            var sql = goods.Where(x => x.price != null && x.price != 0);
            decimal t = 0;
            if (sql.Count() > 0)
                 t = sql.Max(x => x.price).Value;
            decimal outs;
            var sql1 = goods.Where(x => decimal.TryParse(x.col7, out outs));
            if (sql1.Count() > 0)
                t = Math.Max(t, sql1.Max(x => decimal.Parse(x.col7)));
            return t;
        }
        public decimal GetPlaneAMin(IEnumerable<Goods> goods)
        {
            var sql = goods.Where(x => x.price != null && x.price != 0);
            decimal t = 0;
            if (sql.Count() > 0)
                t = sql.Min(x => x.price).Value;
            decimal outs;
            var sql1 = goods.Where(x => decimal.TryParse(x.col7, out outs) && decimal.Parse(x.col7) != 0);
            if (sql1.Count() > 0)
            {
                if (t != 0)
                    t = Math.Min(t, sql1.Min(x => decimal.Parse(x.col7)));
                else
                    t = sql1.Min(x => decimal.Parse(x.col7));
            }
            return t;
        }
            // 学飞行
        public JsonResult MapPlaneB(ApiMapPlaneRequest model)
        {
            var tmp = db.Sellers.ToList().Where(x => x.group_id == 5).Select(x => new ApiMapPlaneB()
            {
                lat = x.Airports.First().location.Split('|')[1],
                lng = x.Airports.First().location.Split('|')[0],
                dis = GetDistance(x.Airports.First().location, model.lat, model.lng),
                name = x.name,
                priceMin = x.Goods.Where(xx => xx.price.HasValue && xx.price != 0).Count() > 0 ? x.Goods.Where(xx => xx.price.HasValue && xx.price != 0).Min(xx => xx.price).Value : 0,
                priceMax = x.Goods.Where(xx => xx.price.HasValue).Max(xx => xx.price).Value,
                type = GetPlaneType(x.Goods),
                address = x.Airports.First().address.Substring(0,3),
                school = new ApiGetSchoolModel()
                {
                    id = x.seller_id,
                    idString = x.seller_id.ToString(),
                    name = x.name,
                    img = x.img,
                    dis = GetDistance(x.Airports.First().location, model.lat, model.lng),
                    lat = x.Airports.First().location.Split('|')[1],
                    lng = x.Airports.First().location.Split('|')[0],
                    address = x.address,
                    desc = x.desci,
                    priceMax = x.Goods.Max(sx => sx.price).Value,
                    priceMin = x.Goods.Min(sx => sx.price).Value,
                    planes = x.Goods.Select(xxx => new ApiSchoolPlaneModel()
                    {
                        id = xxx.good_id,
                        idString = xxx.good_id.ToString(),
                        name = xxx.name,
                        model = xxx.model,
                        marketPrice = xxx.market_price ?? 0,
                        img = xxx.img,
                        col1 = xxx.col1,
                        col2 = xxx.col2,
                        col3 = xxx.col3,
                        col4 = xxx.col4,
                        col6 = xxx.col6,
                        col7 = xxx.col7,
                        col8 = xxx.col8,
                        desci = xxx.desci,
                        saled = xxx.saled ?? 0,
                        comments = xxx.comments ?? 0,
                        collection = xxx.collected ?? 0,
                        visited = xxx.visited ?? 0,
                        imgs = xxx.GoodImages.Select(xxxx => xxxx.img).ToArray(),
                        price = xxx.price ?? 0,
                        unit = xxx.unit,
                        type = xxx.GoodTypes.name
                    }).ToList()
                }
            }).OrderBy(x => x.dis);

            return S("获取成功", tmp);
        }


        // 用户收藏
            // 新增
        public JsonResult AddUserCollect(ApiAddUserCollectRequest model)
        {
            var user = db.Users.Where(x => x.phone == model.phone).First();
            if (db.UserCollect.Where(x => x.user_id == user.user_id && x.good_id == model.goodId).Count() > 0)
                return E("已经收藏过了~~");
            db.UserCollect.Add(new UserCollect() { 
                created_at = DateTime.Now,
                good_id = model.goodId,
                user_id = user.user_id
            });
            db.Goods.Where(x => x.good_id == model.goodId).First().collected++;
            db.SaveChanges();
            db.SaveChanges();

            return S("收藏成功");
        }
        
             // 删除
        public JsonResult DelUserCollect(ApiDelUserCollectRequest model)
        {
            var col = db.UserCollect.Where(x => x.id == model.id).First();
            db.UserCollect.Remove(col);
            db.Goods.Where(x => x.good_id == col.good_id).First().collected--;
            db.SaveChanges();

            return S("删除成功");
        }

            // 获取
                // 买飞机
        public JsonResult GetUserCollectA(ApiGetUserCollectRequest model)
        {
            var user = db.Users.Where(x => x.phone == model.phone).First();
            var tmp = db.UserCollect.ToList().Where(x => x.user_id == user.user_id && CheckIsA(x.good_id)).Select(x => GetWTFA(x.id,x.good_id));
            return S("获取成功", tmp);
        }

            // 航空装备
        public JsonResult GetUserCollectB(ApiGetUserCollectRequest model)
        {
            var user = db.Users.Where(x => x.phone == model.phone).First();
            var tmp = db.UserCollect.ToList().Where(x => x.user_id == user.user_id && CheckIsB(x.good_id)).Select(x => GetWTFB(x.id,x.good_id));
            return S("获取成功", tmp);
        }

            // 版本更新
        public JsonResult GetVersion()
        {
            var str = System.IO.File.ReadAllText(Server.MapPath("~/Tools/version"));
            var sp = str.Split('|');
            return S("获取成功", new { 
                androidVersion = sp[0],
                androidLink = sp[1],
                iosVersion = sp[2],
                iosLink = sp[3]
            });
        }

        private bool CanToDecimal(string d)
        {
            decimal s;
            return decimal.TryParse(d,out s);
        }
        private ApiGetUserCollectA GetWTFA(int del,int id)
        {
            var good = db.Goods.Where(x => x.good_id == id).First();
            return new ApiGetUserCollectA()
            { 
                delId = del,
                    id = good.good_id,
                    idString = good.good_id.ToString(),
                    name = good.name,
                    model = good.model,
                    marketPrice = good.market_price ?? 0,
                    img = good.img,
                    col1 = good.col1,
                    col2 = good.col2,
                    col3 = good.col3,
                    col4 = good.col4,
                    col6 = good.col6,
                    col7 = good.col7,
                    col8 = good.col8,
                    desci = good.desci,
                    imgs = good.GoodImages.Select(s => s.img).ToArray(),
                    saled = good.saled ?? 0,
                    comments = good.comments ?? 0,
                    collection = good.collected ?? 0,
                    visited = good.visited ?? 0,
                    shared = good.shared ?? 0
                };
        }
        private bool CheckIsA(int id)
        {
            return db.Goods.Where(x => x.good_id == id).First().type_id == 9;
        }
        private bool CheckIsB(int id)
        {
            return db.Goods.Where(x => x.good_id == id).First().GoodTypes.parent == 10;
        }
        private ApiGetUserCollectB GetWTFB(int del,int id)
        {
            var good = db.Goods.Where(x => x.good_id == id).First();
            return new ApiGetUserCollectB()
            {
                delId = del,
                id = good.good_id,
                idString = good.good_id.ToString(),
                name = good.name,
                price = good.price ?? 0,
                unit = good.unit,
                img = good.img,
                imgs = good.GoodImages.Select(x => x.img).ToArray(),
                desc = good.desci,
                good = good.collected ?? 0,
                shared = good.shared ?? 0,
                comments = good.comments ?? 0,
                visited = good.visited ?? 0
            };
        }
        private int GetCollectType(int goodId)
        {
            var good = db.Goods.Where(x => x.good_id == goodId).First();
            if (good.type_id == 9)
                return 0;
            return 1;
        }
        private string GetPlaneType(ICollection<Goods> goods)
        {
            var str = string.Empty;
            if (goods.Where(x => x.col1 == "公务机").Count() > 0) str += "1";
            else str += "0";
            if (goods.Where(x => x.col1 == "旋翼机").Count() > 0) str += "1";
            else str += "0";
            if (goods.Where(x => x.col1 == "直升机").Count() > 0) str += "1";
            else str += "0";
            if (goods.Where(x => x.col1 == "固定翼").Count() > 0) str += "1";
            else str += "0";
            return str;
            
        }
        private const double EARTH_RADIUS = 6378.137;
        private double GetDistance(string loc,double lat,double lng)
        {
            var lc = loc.Split('|');
            //var x = lat - double.Parse(lc[1]);
            //var y = lng - double.Parse(lc[0]);
            //return Math.Pow(x * x - y * y, 0.5f);
            return GetDistance(double.Parse(lc[1]),double.Parse(lc[0]),lat,lng);
        }
        private double GetDistance(double lat1, double lng1, double lat2, double lng2)
        {
            double radLat1 = rad(lat1);
            double radLat2 = rad(lat2);
            double a = radLat1 - radLat2;
            double b = rad(lng1) - rad(lng2);
            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
             Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            s = Math.Round(s * 10000) / 10000;
            return s;
        }

        private int DateTimeToTimestamp(DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }

        private double rad(double d)
        {
            return d * Math.PI / 180.0;
        }
        private string GetImgUrl(string p)
        {
            return "http://" + Request.Url.Host + ":" + Request.Url.Port + "/" + p;
        }
        private string SaveBase64Img(string code, string path)
        {
            var sp = code.Split(',');
            var buffer = Convert.FromBase64String(sp[1]);
            var sp1 = sp[0].Split(';');
            var ext = sp1[0].Split('/');
            Random rand = new Random();
            string fileName = MD5Tool.Encrypt(DateTime.Now.ToString("y-M-d H-m-s.fff") + rand.Next()) + "." + ext[1];
            System.IO.File.WriteAllBytes(Server.MapPath("~/Images/" + path ) + "/" + fileName, buffer);
            return "Images/" + path + "/" + fileName;
        }

        private string RandId(int length)
        {
            string str = "888888";
            do
            {
                Random rand = new Random();
                str = rand.Next(1, 9).ToString();
                for (int i = 0; i < length - 1; i++)
                    str += rand.Next(9).ToString();
            } while (CheckRandId(str));
            return str;
        }

        private bool CheckRandId(string id)
        {
            if (db.Users.Where(x => x.rand_id == id).Count() > 0)
                return true;
            return false;
        }

        private ApiUserModel ReturnUser(Users user)
        {
            return new ApiUserModel() 
            { 
                userName = user.username,
                phone = user.phone,
                headImg = "http://" + Request.Url.Host + ":" + Request.Url.Port + "/" + user.head_img,
                randId = user.rand_id,
                trueName = user.true_name,
                idCard = user.id_card,
                gender = user.gender.HasValue ? user.gender.Value : true,
                address = user.address,
                workUnit = user.work_unit
            };
        }

        private bool UserName(string username)
        {
            if (db.Users.Where(x => x.username == username).Count() > 0)
                return true;
            return false;
        }

        private bool Phone(string phone)
        {
            if (db.Users.Where(x => x.phone == phone).Count() > 0)
                return true;
            return false;
        }

        public JsonResult S(string info,object data = null)
        {
            return Pack(1,info,data);
        }

        public JsonResult E(string info, object data = null)
        {
            return Pack(0, info, data);
        }

        public JsonResult Pack(int code,string info,object data)
        {
            return Json(new { code = code,data = data,info = info },JsonRequestBehavior.AllowGet);
        }
    }
}