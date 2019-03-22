using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LTKJ.RTU
{
    /// <summary>
    /// 设备参数信息
    /// </summary>
    public class TempData
    {

        /// <summary>
        /// 设备编号ID
        /// </summary>
        public string DeviceId { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 设备IP
        /// </summary>
        public string DeviceIp { get; set; }

        /// <summary>
        /// 设备PORT
        /// </summary>
        public string DevicePort { get; set; }


        /// <summary>
        /// 指令类型
        /// </summary>
        public string cmdType { get; set; }
        /// <summary>
        /// D1
        /// </summary>
        public double? D1 { get; set; }
        /// <summary>
        ///D2
        /// </summary>
        public double? D2 { get; set; }
        /// <summary>
        /// D3
        /// </summary>
        public double? D3 { get; set; }

        /// <summary>
        /// D4
        /// </summary>
        public double? D4 { get; set; }
        /// <summary>
        /// D5
        /// </summary>
        public double? D5 { get; set; }
        /// <summary>
        /// D6
        /// </summary>
        public double? D6 { get; set; }
        /// <summary>
        /// D7
        /// </summary>
        public double? D7 { get; set; }
        /// <summary>
        /// D8
        /// </summary>
        public double? D8 { get; set; }
        /// <summary>
        /// D9
        /// </summary>
        public double? D9 { get; set; }

                     
        /// <summary>
        /// 采集时间
        /// </summary>
        public DateTime? Stime { get; set; }
   

    }
}
