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
    
    public partial class Orders
    {
        public int user_id { get; set; }
        public int good_id { get; set; }
        public int order_id { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public string orderId { get; set; }
        public Nullable<decimal> amount { get; set; }
        public string col1 { get; set; }
        public string col2 { get; set; }
        public string col3 { get; set; }
        public string col4 { get; set; }
        public string col5 { get; set; }
        public string col6 { get; set; }
        public string col7 { get; set; }
        public string col8 { get; set; }
        public string col9 { get; set; }
        public Nullable<short> status { get; set; }
        public Nullable<int> type_id { get; set; }
    
        public virtual Users Users { get; set; }
        public virtual OrderType OrderType { get; set; }
        public virtual Goods Goods { get; set; }
    }
}
