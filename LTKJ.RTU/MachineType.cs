using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LTKJ.RTU
{
    /// <summary>
    /// 设备参数信息
    /// </summary>
    public class Type
    {

        /// <summary>
        /// 水表类型 1西安思坦 2上海一诺
        /// </summary>
        public string WaterType { get; set; }
        /// <summary>
        /// 气表类型 1 西安思坦 2 浙江天信
        /// </summary>
        public string AirType { get; set; }

        /// <summary>
        /// 井底压力类型   1西安思坦 2 贵州凯山
        /// </summary>
        public string BottomType { get; set; }



    }
}
