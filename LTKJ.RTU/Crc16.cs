using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LTKJ.RTU
{
    class Crc16
    {
        public byte[] crc16_bytes(byte[] data, int byteLen)
        {
            int xda, xdapoly;
            int i, j, xdabit;
            xda = 0xFFFF;
            xdapoly = 0xA001;

            for (i = 0; i < byteLen; i++)
            {
                xda ^= data[i];
                for (j = 0; j < 8; j++)
                {
                    xdabit = (int)(xda & 0x01);
                    xda >>= 1;
                    if (xdabit == 1)
                        xda ^= xdapoly;
                }
            }

            byte[] outBytes = { (byte)(xda & 0xff), (byte)(xda >> 8) };
            return outBytes;
        }
    }
}
