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
        /// 电压A
        /// </summary>
        public double? VoltageA { get; set; }
        /// <summary>
        /// 电压B
        /// </summary>
        public double? VoltageB { get; set; }
        /// <summary>
        /// 电压C
        /// </summary>
        public double? VoltageC { get; set; }

        /// <summary>
        /// 电流A
        /// </summary>
        public double? CurrentA { get; set; }
        /// <summary>
        /// 电流B
        /// </summary>
        public double? CurrentB { get; set; }
        /// <summary>
        /// 电流C
        /// </summary>
        public double? CurrentC { get; set; }


        /// <summary>
        /// 阀位值 电动执行机构
        /// </summary>
        public double? GasValue { get; set; }
        /// <summary>
        /// 设定值 电动执行机构 
        /// </summary>
        public double? GasConf { get; set; }

        /// <summary>
        /// 温度
        /// </summary>
        public double? Temperature { get; set; }
        /// <summary>
        /// 油压
        /// </summary>
        public double? YouYa { get; set; }
        /// <summary>
        /// 套压
        /// </summary>
        public double? TaoYa { get; set; }
        /// <summary>
        /// 回压
        /// </summary>
        public double? HuiYa { get; set; }
        /// <summary>
        /// 有功功率
        /// </summary>
        public double? Active_Power { get; set; }
        /// <summary>
        /// 无功功率
        /// </summary>
        public double? Reactive_Power { get; set; }
        /// <summary>
        /// 功率因数
        /// </summary>
        public double? Ecos { get; set; }

        /// <summary>
        /// 冲程
        /// </summary>
        public double? Stroke { get; set; }
        /// <summary>
        /// 冲次
        /// </summary>
        public double? Frequency { get; set; }

        /// <summary>
        /// 设定频率
        /// </summary>
        public double? Set_Frequency { get; set; }
        /// <summary>
        /// 运行频率
        /// </summary>
        public double? Run_Frequency { get; set; }

        /// <summary>
        /// 变频器状态
        /// </summary>
        public string State_Frequency { get; set; }

        /// <summary>
        /// 最大载荷
        /// </summary>
        public double? Load_Max { get; set; }
        /// <summary>
        /// 最小载荷
        /// </summary>
        public double? Load_Min { get; set; }

        /// <summary>
        /// 上行电流
        /// </summary>
        public double? I_Up { get; set; }
        /// <summary>
        /// 下行电流
        /// </summary>
        public double? I_Down { get; set; }

        /// <summary>
        /// 上行功率
        /// </summary>
        public double? P_Up { get; set; }
        /// <summary>
        /// 下行功率
        /// </summary>
        public double? P_Down { get; set; }

        /// <summary>
        /// 用电量
        /// </summary>
        public double? E_E { get; set; }

        /// <summary>
        /// 水表正向累计流量
        /// </summary>
        public double? Water_Positive_Flow { get; set; }

        /// <summary>
        /// 水表反向累计流量
        /// </summary>
        public double? Water_Reverse_Flow { get; set; }


        /// <summary>
        /// 水表总累计流量
        /// </summary>
        public double? Water_All_Flow { get; set; }

        /// <summary>
        /// 水表瞬时流量
        /// </summary>
        public double? Water_Cur_Flow { get; set; }

        /// <summary>
        /// 气表温度
        /// </summary>
        public double? Air_Temperature { get; set; }

        /// <summary>
        /// 气表压力
        /// </summary>
        public double? Air_Pressure { get; set; }


        /// <summary>
        /// 气表瞬时流量(标况)
        /// </summary>
        public double? Air_Cur_Flow { get; set; }

        /// <summary>
        /// 气表瞬时流量(工况)
        /// </summary>
        public double? Air_Work_Cur_Flow { get; set; }

        /// <summary>
        /// 气表累计流量
        /// </summary>
        public double? Air_All_Flow { get; set; }

        /// <summary>
        /// 井底温度
        /// </summary>
        public double? Bottom_Temperature { get; set; }

        /// <summary>
        /// 井底压力
        /// </summary>
        public double? Bottom_Pressure { get; set; }

        /// <summary>
        /// 位移a1,a2,a3...
        /// </summary>
        public string Shift { get; set; }
        /// <summary>
        /// 载荷b1,b2,b3...
        /// </summary>
        public string Load { get; set; }
        /// <summary>
        /// 功图a1,b1;a2,b2;a3,b3...
        /// </summary>
        public string Diagram { get; set; }

        /// <summary>
        /// 电流a1,a2,a3...
        /// </summary>
        public string I_List { get; set; }


        /// <summary>
        /// 功率a1,a2,a3...
        /// </summary>
        public string A_P_List { get; set; }

        /// <summary>
        /// 地址码
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 变频启动标志位
        /// </summary>
        public int? FreState { get; set; }

        /// <summary>
        /// 工频启动标志位
        /// </summary>
        public int? WorkState { get; set; }

        /// <summary>
        /// 启动标志位 0停止，1变频启动，2工频启动，3未采集到数据
        /// </summary>
        public int? mark { get; set; }

        /// <summary>
        /// 采集时间
        /// </summary>
        public DateTime? ReadDate { get; set; }

        /// <summary>
        /// 功图采集时间
        /// </summary>
        public DateTime? gtReadDate { get; set; }

    }
}
