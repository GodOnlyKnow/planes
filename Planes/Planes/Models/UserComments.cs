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
    
    public partial class UserComments
    {
        public UserComments()
        {
            this.UserCommentImages = new HashSet<UserCommentImages>();
            this.UserCommentReplys = new HashSet<UserCommentReplys>();
        }
    
        public int id { get; set; }
        public int user_id { get; set; }
        public string body { get; set; }
        public int good { get; set; }
        public System.DateTime created_at { get; set; }
        public string name { get; set; }
        public byte gender { get; set; }
        public int type { get; set; }
    
        public virtual ICollection<UserCommentImages> UserCommentImages { get; set; }
        public virtual ICollection<UserCommentReplys> UserCommentReplys { get; set; }
        public virtual Users Users { get; set; }
    }
}
