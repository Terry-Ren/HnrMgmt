//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace HnrMgmtAPI.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class T_ExmRecord
    {
        public string RecordID { get; set; }
        public string ApplyID { get; set; }
        public System.DateTime ApplyTime { get; set; }
        public string ExmID { get; set; }
        public Nullable<System.DateTime> ExmTime { get; set; }
        public string Reason { get; set; }
        public string State { get; set; }
    }
}