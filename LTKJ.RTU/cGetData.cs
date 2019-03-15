using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LTKJ.RTU
{
    class cGetData
    {
        public uint DeviceID = 0;         //设备ID
        public float TempVal = 0F;        //温度值
        public float RHVal = 0F;          //湿度值
        public uint BATVal = 0;           //电量值
        public uint SIGVal = 0;           //信号值
        public bool isOK = false;       //是否解析成功

        public cGetData(byte[] data)
        {
            uint tmpUINT = 0;
            Crc16 oCRC16 = new Crc16();
            byte[] crcHL = oCRC16.crc16_bytes(data, data.Length - 2);

            if (crcHL[0] == data[data.Length - 2] && crcHL[1] == data[data.Length - 1])
            {
                DeviceID = data[8];
                DeviceID += (uint)(data[7] << 8);
                DeviceID += (uint)(data[6] << 16);
                DeviceID += (uint)(data[5] << 24);

                tmpUINT = data[10];
                tmpUINT += (uint)(data[9] << 8);

                TempVal = ((float)tmpUINT) / 10;

                tmpUINT = data[12];
                tmpUINT += (uint)(data[11] << 8);

                RHVal = ((float)tmpUINT) / 10;

                if (data[2] > 7) BATVal = data[14];
                if (data[2] > 9) SIGVal = data[16];

                isOK = true;
            }
            else
            {
                isOK = false;
            }
        }
    }
}
