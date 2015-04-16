using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Planes.Models
{
    public class LUser
    {
        public static string UserName 
        {
            get
            {
                return LoginUser == null ? string.Empty : LoginUser.username;
            }
        }

        public static int Id
        {
            get
            {
                return LoginUser == null ? 0 : LoginUser.seller_id;
            }
        }
        public static Sellers LoginUser 
        {  
            get
            {
                return HttpContext.Current.Session["LoginUser"] as Sellers;
            }
        }
    }
}