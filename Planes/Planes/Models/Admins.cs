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
    
    public partial class Admins
    {
        public Admins()
        {
            this.LoginLogs = new HashSet<LoginLogs>();
        }
    
        public int admin_id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
    
        public virtual ICollection<LoginLogs> LoginLogs { get; set; }
    }
}