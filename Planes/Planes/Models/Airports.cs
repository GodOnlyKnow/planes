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
    
    public partial class Airports
    {
        public int seller_id { get; set; }
        public int area_id { get; set; }
        public int airport_id { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public string location { get; set; }
        public string address { get; set; }
    
        public virtual Areas Areas { get; set; }
        public virtual Sellers Sellers { get; set; }
    }
}
