//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Planes.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Users
    {
        public Users()
        {
            this.Orders = new HashSet<Orders>();
            this.GoodCommentReplys = new HashSet<GoodCommentReplys>();
            this.GoodComments = new HashSet<GoodComments>();
            this.MessageCommentReplys = new HashSet<MessageCommentReplys>();
            this.MessageComments = new HashSet<MessageComments>();
            this.Feedback = new HashSet<Feedback>();
            this.Entrys = new HashSet<Entrys>();
        }
    
        public int user_id { get; set; }
        public int level_id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string phone { get; set; }
        public string id_card { get; set; }
        public string work_unit { get; set; }
        public string head_img { get; set; }
        public string true_name { get; set; }
        public Nullable<bool> gender { get; set; }
        public string address { get; set; }
        public Nullable<short> is_lock { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public string rand_id { get; set; }
    
        public virtual Levels Levels { get; set; }
        public virtual ICollection<Orders> Orders { get; set; }
        public virtual ICollection<GoodCommentReplys> GoodCommentReplys { get; set; }
        public virtual ICollection<GoodComments> GoodComments { get; set; }
        public virtual ICollection<MessageCommentReplys> MessageCommentReplys { get; set; }
        public virtual ICollection<MessageComments> MessageComments { get; set; }
        public virtual ICollection<Feedback> Feedback { get; set; }
        public virtual ICollection<Entrys> Entrys { get; set; }
    }
}
