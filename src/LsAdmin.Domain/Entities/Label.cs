using System;
using System.Collections.Generic;
using System.Text;

namespace LsAdmin.Domain.Entities
{
    public class Label: BaseEntity {
        ///<summary>
        ///标签类型
        ///</summary>
        public string Type { get; set; }

        ///<summary>
        ///标签名称
        ///</summary>
        public string Name { get; set; }
      

    }
}
