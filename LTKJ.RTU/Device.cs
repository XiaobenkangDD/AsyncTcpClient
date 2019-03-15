using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LTKJ.RTU
{
    /// <summary>
    /// 设备参数信息
    /// </summary>
    public class Device
    {    
        /// <summary>
        /// 设备编号
        /// </summary>
        public string device { get; set; }

        /// <summary>
        /// 气体类型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 浓度
        /// </summary>
        public double? value { get; set; }

        /// <summary>
        /// 是否报警  true 报警，false不报警
        /// </summary>
        public bool alarm { get; set; }


    }
}
