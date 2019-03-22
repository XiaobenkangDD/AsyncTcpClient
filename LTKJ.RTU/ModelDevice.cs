using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LTKJ.RTU
{
    /// <summary>
    /// 设备参数信息
    /// </summary>
    public class ModelDevice
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
        /// 1#加热炉进口温度
        /// </summary>
        public double? AI1_VART { get; set; }
        /// <summary>
        /// 1#加热炉出口温度
        /// </summary>
        public double? AI2_VART { get; set; }
        /// <summary>
        /// 2#加热炉进口温度
        /// </summary>
        public double? AI3_VART { get; set; }

        /// <summary>
        /// 2#加热炉出口温度
        /// </summary>
        public double? AI4_VART { get; set; }
        /// <summary>
        /// 1#分离器压力
        /// </summary>
        public double? AI5_VART { get; set; }
        /// <summary>
        /// 1#分离器液位
        /// </summary>
        public double? AI6_VART { get; set; }
        /// <summary>
        /// 2#分离器液位
        /// </summary>
        public double? AI7_VART { get; set; }
        /// <summary>
        /// 泵进口压力
        /// </summary>
        public double? AI8_VART { get; set; }
        /// <summary>
        /// 泵出口压力
        /// </summary>
        public double? AI9_VART { get; set; }



        /// <summary>
        /// 1#加热炉进口温度上限
        /// </summary>
        public double? AI1_HI { get; set; }
        /// <summary>
        /// 1#加热炉出口温度上限
        /// </summary>
        public double? AI2_HI { get; set; }
        /// <summary>
        /// 2#加热炉进口温度上限
        /// </summary>
        public double? AI3_HI { get; set; }
        /// <summary>
        /// 2#加热炉出口温度上限
        /// </summary>
        public double? AI4_HI { get; set; }
        /// <summary>
        /// 1#分离器压力上限
        /// </summary>
        public double? AI5_HI { get; set; }
        /// <summary>
        /// 1#分离器液位上限
        /// </summary>
        public double? AI6_HI { get; set; }

        /// <summary>
        /// 2#分离器液位上限
        /// </summary>
        public double? AI7_HI { get; set; }
        /// <summary>
        /// 泵进口压力上限
        /// </summary>
        public double? AI8_HI { get; set; }
        /// <summary>
        /// 泵出口压力上限
        /// </summary>
        public double? AI9_HI { get; set; }


        /// <summary>
        /// 1#加热炉进口温度下限
        /// </summary>
        public double? AI1_LOW { get; set; }
        /// <summary>
        /// 1#加热炉出口温度下限
        /// </summary>
        public string AI2_LOW { get; set; }
        /// <summary>
        /// 2#加热炉进口温度下限
        /// </summary>
        public double? AI3_LOW { get; set; }
        /// <summary>
        /// 2#加热炉出口温度下限
        /// </summary>
        public double? AI4_LOW { get; set; }
        /// <summary>
        /// 1#分离器压力下限
        /// </summary>
        public double? AI5_LOW { get; set; }
        /// <summary>
        /// 1#分离器液位下限
        /// </summary>
        public double? AI6_LOW { get; set; }
        /// <summary>
        /// 2#分离器液位下限
        /// </summary>
        public double? AI7_LOW { get; set; }
        /// <summary>
        /// 泵进口压力下限
        /// </summary>
        public double? AI8_LOW { get; set; }
        /// <summary>
        /// 泵出口压力下限
        /// </summary>
        public double? AI9_LOW { get; set; }


        /// <summary>
        /// 加热炉熄火报警
        /// </summary>
        public double? DI1ZT { get; set; }
        /// <summary>
        /// 发电机运行状态
        /// </summary>
        public double? DI2ZT { get; set; }
        /// <summary>
        /// 照明灯控制
        /// </summary>
        public double? ZMDKZ { get; set; }
        
             
        /// <summary>
        /// 采集时间
        /// </summary>
        public DateTime? ReadDate { get; set; }
   

    }
}
