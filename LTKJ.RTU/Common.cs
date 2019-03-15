using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using LTKJ.DataBase;

namespace LTKJ.RTU
{
    public abstract class Common
    {
        #region 数据库基础操作方法

        /// <summary>
        /// 以Sql方式获取表格数据
        /// 刘金鹏 2015-9-10
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public static DataTable GetTableData(string tableName)
        {
            DataTable dtResult = GetTableData(tableName, null, null, null, null);
            return dtResult;
        }

        /// <summary>
        /// 获取表格/视图数据datatable
        /// 刘金鹏 2016-3-14
        /// </summary>
        /// <param name="tableName">必选：表/视图名</param>
        /// <param name="where">可选：查询条件</param>
        /// <param name="jsonWhere">可选：JSON条件</param>
        /// <returns></returns>
        public static DataTable GetTableData(string tableName, string where)
        {
            DataTable dtResult = GetTableData(tableName, where, null, null, null);
            return dtResult;
        }

        /// <summary>
        /// 获取表格/视图数据datatable
        /// 刘金鹏 2016-3-14
        /// </summary>
        /// <param name="tableName">必选：表/视图名</param>
        /// <param name="where">可选：查询条件</param>
        /// <param name="jsonWhere">可选：JSON条件</param>
        /// <param name="fields">可选：查询的列名1,列名2...</param>
        /// <returns></returns>
        public static DataTable GetTableData(string tableName, string where, string fields)
        {
            DataTable dtResult = GetTableData(tableName, where, fields, null, null);
            return dtResult;
        }

        /// <summary>
        /// 获取表格/视图数据datatable
        /// 刘金鹏 2016-3-14
        /// 修改人员：刘金鹏 2017-3-27
        /// 修改内容：添加JSON条件和查询字段
        /// </summary>
        /// <param name="tableName">必选：表/视图名</param>
        /// <param name="where">可选：查询条件</param>
        /// <param name="jsonWhere">可选：JSON条件</param>
        /// <param name="fields">可选：查询的列名1,列名2...</param>
        /// <param name="sortName">可选：排序字段</param>
        /// <param name="sortOrder">可选：排序顺序asc/desc</param>
        /// <returns></returns>
        public static DataTable GetTableData(string tableName, string where, string fields, string sortName, string sortOrder)
        {
            DataTable dtResult = new DataTable();//返回数据表
            string sqlText = string.Empty;//SQL语句
            try
            {
                //没有查询条件时默认为空
                where = string.IsNullOrEmpty(where) ? "" : where;
                //查询的列名为空时默认为*
                fields = string.IsNullOrEmpty(fields) ? "*" : fields;
                //SQL语句拼接
                sqlText = string.Format(" select {2} from {0} t where 1=1 {1}", tableName, where, fields);
                //排序字段和顺序，没有排序时默认为asc
                if (!string.IsNullOrEmpty(sortName))
                {
                    sortOrder = string.IsNullOrEmpty(sortOrder) ? " asc " : sortOrder;
                    sqlText += string.Format(" order by {0} {1} ", sortName, sortOrder);
                }
                //执行SQL 
                dtResult = DbHelper.GetDataTable(sqlText);
            }
            catch (Exception ex)
            {
                WriteErrorLog(MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);//将出错信息写入错误日志
            }
            return dtResult;
        }

        /// <summary>
        /// 根据表名主键获取主键最大值+1 取不值时默认为1
        /// 刘金鹏 2015-9-14
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="keyName">主键名</param>
        /// <returns></returns>
        public static string GetTableMaxID(string tableName, string keyName)
        {
            string strResult = GetTableMaxID(tableName, keyName, "");
            return strResult;
        }

        /// <summary>
        /// 根据表名主键获取主键最大值+1 取不值时默认为1
        /// 刘金鹏 2015-9-14
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="keyName">主键名</param>
        /// <param name="where">条件</param>
        /// <returns></returns>
        public static string GetTableMaxID(string tableName, string keyName, string where)
        {
            string strResult = string.Empty;//返回结果
            try
            {
                //没有查询条件时默认为空
                where = string.IsNullOrEmpty(where) ? "" : where;
                //获取最大值+1
                string sqlText = string.Format("select max({1})+1 as ID from {0} where 1=1 {2} ", tableName, keyName, where);
                object objResult = DbHelper.GetSingle(sqlText, null);
                //为空时默认值为1
                strResult = objResult == null ? "1" : objResult.ToString();
            }
            catch (Exception ex)
            {
                WriteErrorLog(MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);
            }
            return strResult;
        }

        /// <summary>
        /// 根据序列名获取序列值
        /// 刘金鹏 2015-12-20
        /// </summary>
        /// <param name="seqName">序列名称</param>
        /// <returns></returns>
        public static string GetSeqMaxId(string seqName)
        {
            string strResult = "";//返回结果
            try
            {
                //SQL语句
                string sqlText = string.Format("SELECT {0}.nextval FROM dual ", seqName);
                //执行SQL 
                object objResult = DbHelper.GetSingle(sqlText, null);
                strResult = objResult.ToString();
            }
            catch (Exception ex)
            {
                WriteErrorLog(MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);
            }
            return strResult;
        }

        /// <summary>
        /// 删除表数据，支持按删除多个ID(1,2,3)
        /// 刘金鹏 2017-1-21
        /// </summary>
        /// <param name="tableName">表/视图名</param>
        /// <param name="keyName">主键字段名</param>
        /// <param name="keyId">主键ID(1,2,3)</param>
        /// <returns></returns>
        public static int DeleteTableData(string tableName, string keyName, string keyId)
        {
            int intResult = DeleteTableData(tableName, keyName, keyId, "");
            return intResult;
        }

        /// <summary>
        /// 删除表数据，支持按条件删除
        /// 刘金鹏 2017-1-21
        /// </summary>
        /// <param name="tableName">表/视图名</param>
        /// <param name="keyName">主键字段名</param>
        /// <param name="keyId">主键ID(1,2,3)</param>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public static int DeleteTableData(string tableName, string keyName, string keyId, string where)
        {
            int intResult = -1;//返回结果
            try
            {
                //没有查询条件时默认为空
                where = string.IsNullOrEmpty(where) ? "" : where;
                //查询条件
                if (!string.IsNullOrEmpty(keyName) && !string.IsNullOrEmpty(keyId))
                {
                    //将主键值1,2,3处理为'1','2','3'格式
                    string[] arrId = keyId.Split(',');
                    for (int i = 0; i < arrId.Length; i++)
                    {
                        arrId[i] = "'" + arrId[i] + "'";
                    }
                    keyId = string.Join(",", arrId);
                }
                where += string.Format(" and {0} in ({1})", keyName, keyId);
                //SQL语句
                string sqlText = string.Format("delete from {0}  where 1=1 {1}", tableName, where);
                // 执行SQL
                intResult = DbHelper.ExecuteNonQuery(sqlText);
            }
            catch (Exception ex)
            {
                WriteErrorLog(MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);//将出错信息写入错误日志
            }
            return intResult;
        }

        /// <summary>
        /// 执行查询语句，返回受影响的行数，非增删改命令时返回-1
        /// 刘金鹏 2017-1-21
        /// </summary>
        /// <param name="sqlText">查询语句</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string sqlText)
        {
            int intResult = -1;//返回结果
            //try
            //{
                // 执行SQL
                intResult = DbHelper.ExecuteNonQuery(sqlText);
            //}
            //catch (Exception ex)
            //{
            //    WriteErrorLog(MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);//将出错信息写入错误日志
            //}
            return intResult;
        }

        #endregion

        #region 保存错误日志方法

        /// <summary>
        /// 将出错信息写入错误日志
        /// </summary>
        /// <param name="strError">错误信息</param>
        public static void WriteErrorLog(string strError)
        {
            try
            {
                ////出错时重新启动监听服务
                //tsbtnStopReceive_Click(null, null);
                //tsbtnStartReceive_Click(null, null);

                //没有消息内容时返回
                if (string.IsNullOrEmpty(strError))
                {
                    return;
                }
                //默认保存文件路径
                string filePath = Directory.GetCurrentDirectory() + "\\Log";
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);//创建目录
                }
                //取系统时间作为文件名
                string fileName = filePath + "\\ErrorLog" + DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString() + ".txt";
                if (!File.Exists(fileName))
                {
                    File.CreateText(fileName).Close();//创建文本文件后关闭
                }
                //将消息内容写入文件中
                File.AppendAllText(fileName, DateTime.Now.ToString() + "  " + strError + "\r\n\r\n");
            }
            catch (Exception ex)
            {
                WriteErrorLog(MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);//将出错信息写入错误日志
            }
        }

        #endregion

        #region      数值转换

        /// <summary>
        /// 字节数组转十六进制字符串
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index">起始位置</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        public static string ByteArrayToHexString(byte[] data, int index, int count)
        {
            StringBuilder sb = new StringBuilder(count * 3);
            int endIndex = index + count;
            do
            {
                byte b = data[index];
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
            } while (++index < endIndex);

            return sb.ToString().Trim().ToUpper();
        }

        /// <summary> 
        /// 字节数组转换成16进制字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
            {
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
            }
            return sb.ToString().Trim().ToUpper();
        }

        /// <summary>
        /// 16进制字符串转换成字节数组
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
            {
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            }
            return buffer;
        }
        public string HexToStr(string mHex) // 返回十六进制代表的字符串
        {
            mHex = mHex.Replace(" ", "");
            if (mHex.Length <= 0) return "";
            byte[] vBytes = new byte[mHex.Length / 2];
            for (int i = 0; i < mHex.Length; i += 2)
                if (!byte.TryParse(mHex.Substring(i, 2), NumberStyles.HexNumber, null, out vBytes[i / 2]))
                    vBytes[i / 2] = 0;
            return Encoding.Default.GetString(vBytes);
        }
        #endregion

        #region CRC校验
        /// <summary>
        /// CRC16校验 返回原校验数据和检验码
        /// </summary>
        /// <param name="data">校验数据</param>
        /// <returns>校验数据+高低8位</returns>
        public static string CRC16Calc(string data)
        {
            data = data.Trim();
            string[] datas = data.Split(' ');
            List<byte> bytedata = new List<byte>();

            foreach (string str in datas)
            {
                bytedata.Add(byte.Parse(str, NumberStyles.AllowHexSpecifier));
            }
            byte[] crcbuf = bytedata.ToArray();
            //计算并填写CRC校验码
            int crc = 0xffff;
            int len = crcbuf.Length;
            for (int n = 0; n < len; n++)
            {
                byte i;
                crc = crc ^ crcbuf[n];
                for (i = 0; i < 8; i++)
                {
                    int TT;
                    TT = crc & 1;
                    crc = crc >> 1;
                    crc = crc & 0x7fff;
                    if (TT == 1)
                    {
                        crc = crc ^ 0xa001;
                    }
                    crc = crc & 0xffff;
                }
            }
            string[] redata = new string[2];
            redata[1] = Convert.ToString((byte)((crc >> 8) & 0xff), 16);
            redata[0] = Convert.ToString((byte)((crc & 0xff)), 16);
            string result = (data + " " + redata[0] + " " + redata[1]).ToUpper();
            //此处不确定，为00时应该是字符串已经校验过了，不需要再校验
            if (redata[0] == "0" && redata[1] == "0")
            {
                result = data.ToUpper();
            }
            return result;
        }
        /// <summary>
        /// CRC16校验 返回检验码
        /// </summary>
        /// <param name="data">校验数据</param>
        /// <returns>高低8位</returns>
        public static string CRC16Code(string data)
        {
            string[] datas = data.Split(' ');
            List<byte> bytedata = new List<byte>();

            foreach (string str in datas)
            {
                bytedata.Add(byte.Parse(str, NumberStyles.AllowHexSpecifier));
            }
            byte[] crcbuf = bytedata.ToArray();
            //计算并填写CRC校验码
            int crc = 0xffff;
            int len = crcbuf.Length;
            for (int n = 0; n < len; n++)
            {
                byte i;
                crc = crc ^ crcbuf[n];
                for (i = 0; i < 8; i++)
                {
                    int TT;
                    TT = crc & 1;
                    crc = crc >> 1;
                    crc = crc & 0x7fff;
                    if (TT == 1)
                    {
                        crc = crc ^ 0xa001;
                    }
                    crc = crc & 0xffff;
                }
            }
            string[] redata = new string[2];
            redata[1] = Convert.ToString((byte)((crc >> 8) & 0xff), 16);
            redata[0] = Convert.ToString((byte)((crc & 0xff)), 16);
            string result = (redata[0] + " " + redata[1]).ToUpper();
            return result;
        }
        #endregion

        #region 获取/设置数据库类型和连接字符串
        /// <summary>
        /// 获取数据库类型
        /// 刘金鹏 2016-3-14
        /// </summary>
        /// <returns></returns>
        public static string GetDbType()
        {
            string strResult = DbHelper.DbType;
            return strResult;
        }
        /// <summary>
        /// 设置数据库类型
        /// 刘金鹏 2016-3-14
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        public static void SetDbType(string dbType)
        {
            DbHelper.DbType = dbType;
        }

        /// <summary>
        /// 获取数据库连接字符串
        /// 刘金鹏 2016-3-14
        /// </summary>
        /// <returns></returns>
        public static string GetDbConnectionString()
        {
            string strResult = DbHelper.DbConnectionString;
            return strResult;
        }

        /// <summary>
        /// 设置数据库连接字符串
        /// 刘金鹏 2016-3-14
        /// </summary>
        /// <param name="dbConn">数据库连接字符串</param>
        /// <returns></returns>
        public static void SetDbConnectionString(string dbConn)
        {
            DbHelper.DbConnectionString = dbConn;
        }

        #endregion
    }
}
