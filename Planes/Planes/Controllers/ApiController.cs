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
                rand_id = RandId(6)
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
            user.gender = model.gender;
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
        
        // 获取用户订单
        public JsonResult UserOrders(ApiUserOrderRequestModel model)
        {
            if (!Phone(model.phone)) return E("用户不存在");
            var user = db.Users.Where(x => x.phone == model.phone).First();
            var os = db.Orders
                        .Where(x => x.user_id == user.user_id)
                        .OrderByDescending(x => x.created_at)
                        .ToPagedList(model.page,model.pageSize);
            var list = new List<ApiUserOrderModel>();
            var types = db.OrderType.Select(x => x.name).ToArray();
            foreach (var o in os)
            {
                list.Add(new ApiUserOrderModel() { 
                    orderId = o.orderId,
                    createdAt = o.created_at.Value.ToString("y-M-d H:m:s"),
                    price = o.amount ?? 0,
                    status = types[o.type_id ?? 0 - 1],
                    name = o.Goods.name,
                    img = "http://" + Request.Url.Host + ":" + Request.Url.Port + "/" + o.Goods.img
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
                    img = GetImgUrl(m.img),
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
            foreach (var s in model.imgs.Split(','))
            {
                db.MessageCommentImages.Add(new MessageCommentImages() { 
                    message_id = tmp.id,
                    img = s
                });
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
                var res = m.MessageCommentReplys.OrderByDescending(x => x.created_at).Select(x => { return new ApiSellerMessageCommentReplyModel() { userName = x.Users.username,body = x.body,id = x.parent_id }; });
                var ims = m.MessageCommentImages.Select(x => GetImgUrl(x.img));
                var ls = new List<ApiSellerMessageCommentReplyModel>();
                list.Add(new ApiSellerMessageCommentModel() { 
                    userName = m.Users.username,
                    userImg = m.Users.head_img,
                    createdAt = m.created_at.ToString("y-M-d H:m:s"),
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
                userName = user.username
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
            var tmp = db.Areas.ToList().Select(x => new ApiGetAreasModel()
            {
                id = x.area_id,
                dis = (GetDistance(x.location, model.lat, model.lng).ToString()),
                idString = x.area_id.ToString(),
                name = x.name,
                address = x.address,
                lat = x.location.Split('|')[1],
                lng = x.location.Split('|')[0],
                sellers = x.Airports.Where(xx => xx.Sellers.group_id == 3).Select(xx => new ApiSellerModel()
                {
                    id = xx.seller_id,
                    idString = xx.seller_id.ToString(),
                    name = xx.Sellers.name,
                    img = xx.Sellers.img,
                    planes = x.Goods.Select(xxx => new ApiPlaneAAAModel() 
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
                dis = GetDistance(x.Airports.First().location,model.lat,model.lng).ToString(),
                planes = x.Goods.Select(xxx => new ApiPlaneAAAModel()
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
                dis = GetDistance(x.Areas.location, model.lat, model.lng).ToString(),
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
            var tmp = db.Goods.ToList().Where(x => x.type_id == 7).Select(x => new ApiGetPlaneCCCModel()
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
                col5 = x.col5,
                dis = GetDistance(x.Areas.location, model.lat, model.lng).ToString(),
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
            var tmp = db.Ads.Select(x => new ApiAdModel() 
            { 
                name = x.name,
                link = x.link,
                desc = x.desci,
                img = x.img,
                position = x.position
            });
            return S("获取成功",tmp);
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
                dis = GetDistance(x.Airports.First().location, model.lat, model.lng).ToString(),
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
                dis = GetDistance(x.Airports.First().location, model.lat, model.lng).ToString(),
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
            var tmp = db.Goods.Where(x => x.type_id == 9).Select(x => new ApiSalePlaneModel() { 
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
                visited = x.visited ?? 0
            });

            return S("获取成功",tmp);
        }


        // 航空装备
        public JsonResult GetSaleGoods(ApiGetSaleGoodsRequestModel model)
        {
            var tmp = db.Goods.Where(x => x.type_id == model.type).Select(x => new ApiGetSaleGoodsModel() { 
                name = x.name,
                price = x.price ?? 0,
                unit = x.unit,
                img = x.img,
                imgs = x.GoodImages.Select(xx => xx.img).ToArray(),
                desc = x.desci,
                good = x.priased ?? 0,
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
                var res = m.GoodCommentReplys.OrderByDescending(x => x.created_at).Select(x => { return new ApiSellerMessageCommentReplyModel() { userName = x.Users.username, body = x.body,id = x.parent_id }; });
                var ims = m.GoodCommentImages.Select(x => GetImgUrl(x.img));
                var ls = new List<ApiSellerMessageCommentReplyModel>();
                list.Add(new ApiSellerMessageCommentModel()
                {
                    userName = m.Users.username,
                    userImg = m.Users.head_img,
                    createdAt = m.created_at.ToString("y-M-d H:m:s"),
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
            foreach (var s in model.imgs.Split(','))
            {
                db.GoodCommentImages.Add(new GoodCommentImages()
                {
                    comment_id = tmp.id,
                    img = s
                });
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
                userName = user.username
            });
        }
        public JsonResult AddGoodCommentReplyJson(string json)
        {
            var serializer = new JavaScriptSerializer();
            var model = serializer.Deserialize<ApiSellerMessageCommentReplyRequestModel>(json);
            return AddGoodCommentReply(model);
        }



        // 提交报名表
            
        public JsonResult SendEntry(ApiSendEntryRequestModel model)
        {
            db.Entrys.Add(new Entrys() { 
                name = model.name,
                phone = model.phone,
                id_card = model.idCard,
                address = model.address,
                model = model.model,
                typename = model.typeName,
                start = model.start,
                end = model.end,
                desci = model.desc,
                type = model.type,
                created_at = DateTime.Now,
                user_id = db.Users.Where(x => x.phone == model.phone).First().user_id
            });
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
            var tmp = db.Entrys.Where(x => x.phone == phone).Select(x => new ApiSendEntryRequestModel() { 
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
                createdAt = x.created_at.ToString("y-M-d H:m:s")
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

            return S("提交成功");
        }
        public JsonResult SendFeedbackJson(string json)
        {
            var serializer = new JavaScriptSerializer();
            var model = serializer.Deserialize<ApiSendFeedbackModel>(json);
            return SendFeedback(model);
        }








        private const double EARTH_RADIUS = 6378.137;
        public double GetDistance(string loc,double lat,double lng)
        {
            var lc = loc.Split('|');
            var x = lat - double.Parse(lc[1]);
            var y = lng - double.Parse(lc[0]);
            return Math.Pow(x * x - y * y, 0.5f);
        }
        public double GetDistance(double lat1, double lng1, double lat2, double lng2)
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
                gender = user.gender ?? false,
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