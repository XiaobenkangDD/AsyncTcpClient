using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Objects;
using System.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;

namespace LTKJ.DataBase
{
    /// <summary>
    /// 数据库操作类库，支持Oracle/Sql
    /// </summary>
    public abstract class DbHelper
    {
        /// <summary>
        /// 根据配置文件中的数据库类型 Oralce/Sql
        /// </summary>
        public static string DbType = ConfigurationManager.AppSettings["DbType"];

        /// <summary>
        /// 根据数据库类型获取数据库连接字符串
        /// </summary>
        public static string DbConnectionString = GetDbConnectionString();

        /// <summary>
        /// 根据数据库类型获取数据库连接字符串
        /// </summary>
        public static string GetDbConnectionString()
        {
            string strResult = null;
            switch (DbType.ToLower())
            {
                case "oracle":
                   strResult = OracleDbHelper.ConnectionString;
                  //     strResult = "User Id = cbmdb; Password = cbmdb1qazxsw2; Data Source = (DESCRIPTION = (ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST = 10.218.143.141)(PORT = 11521)))(CONNECT_DATA = (SERVICE_NAME = cbmdb)))";
                    break;
                case "sql":
                    strResult = SqlDbHelper.ConnectionString;
                    break;
            }
            return strResult;
        }


        #region 根据数据库类型，替换SQL语句中的参数占位符@或:

        /// <summary>
        /// 根据数据库类型，替换SQL语句中的参数占位符@或:（Oracle为：Sql@）
        /// 注意：可能出现替换掉正常内容的情况，请在本方法中处理并备注
        /// </summary>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public static string FormatCommandText(string cmdText)
        {
            switch (DbType.ToLower())
            {
                case "oracle":
                    cmdText = cmdText.Replace('@', ':');
                    break;
                case "sql":
                    cmdText = cmdText.Replace(':', '@');
                    break;
            }
            return cmdText;
        }

        #endregion

        #region 转换ObjectParameter参数类型

        ///// <summary>
        ///// 转换ObjectParameter参数为DbParameter类型
        ///// </summary>
        ///// <param name="objParams">参数列表</param>
        ///// <returns></returns>
        //public static DbParameter[] ObjectParamterToDbParameter(ObjectParameter[] objParams)
        //{
        //    if (objParams == null)
        //        return null;
        //    DbParameter[] dbParams = new DbParameter[objParams.Length];
        //    for (int i = 0; i < objParams.Length; i++)
        //    {
        //        dbParams[i].ParameterName = objParams[i].Name;
        //        dbParams[i].Value = objParams[i].Value;
        //    }
        //    return dbParams;
        //}

        /// <summary>
        /// 转换ObjectParameter参数为OracleParameter类型
        /// </summary>
        /// <param name="objParams">参数列表</param>
        /// <returns></returns>
        public static OracleParameter[] ObjectParamterToOracleParameter(ObjectParameter[] objParams)
        {
            if (objParams == null)
                return null;
            List<OracleParameter> paramList = new List<OracleParameter>();
            foreach (ObjectParameter item in objParams)
            {
                OracleParameter param = null;
                //须知：sqlserver中 占位符 加 @ Oracle中 加 :
                //参数类型为数值或日期且参数值为空时，默认为 DBNull.Value
                if (item.ParameterType == typeof(int))
                {
                    param = new OracleParameter(":" + item.Name, OracleDbType.Int32);
                    if (string.IsNullOrEmpty(Convert.ToString(item.Value)))
                    {
                        param.Value = DBNull.Value;
                    }
                    else
                    {
                        param.Value = Convert.ToInt32(item.Value);
                    }
                }
                else if (item.ParameterType == typeof(decimal) || item.ParameterType == typeof(double) || item.ParameterType == typeof(float))
                {
                    param = new OracleParameter(":" + item.Name, OracleDbType.Decimal);
                    if (string.IsNullOrEmpty(Convert.ToString(item.Value)))
                    {
                        param.Value = DBNull.Value;
                    }
                    else
                    {
                        param.Value = Convert.ToDecimal(item.Value);
                    }
                }
                else if (item.ParameterType == typeof(string))
                {
                    param = new OracleParameter(":" + item.Name, OracleDbType.NVarchar2, 500);
                    param.Value = Convert.ToString(item.Value);

                    // 林庆河   2017-3-21  添加调用存储过程是的输出参数
                    if (item.Value.ToString().ToLower() == "output")
                    {
                        //注意：参数值为output时，为存储过程输出参数
                        param.Direction = ParameterDirection.Output;
                    }
                    if (item.Value.ToString().ToLower() == "outputcursor")
                    {
                        //注意：参数值为outputcursor时，为存储过程输出游标参数
                        param.Direction = ParameterDirection.Output;
                        param.OracleDbType = OracleDbType.RefCursor;
                    }
                }
                else if (item.ParameterType == typeof(DateTime))
                {
                    param = new OracleParameter(":" + item.Name, OracleDbType.Date);
                    if (string.IsNullOrEmpty(Convert.ToString(item.Value)))
                    {
                        param.Value = DBNull.Value;
                    }
                    else
                    {
                        param.Value = Convert.ToDateTime(item.Value);
                    }
                }
                paramList.Add(param);
            }
            return paramList.ToArray();
        }
        /// <summary>
        /// 转换ObjectParameter参数为OracleParameter类型
        /// </summary>
        /// <param name="objParams">参数列表</param>
        public static SqlParameter[] ObjectParamterToSqlParameter(ObjectParameter[] objParams)
        {
            if (objParams == null)
                return null;
            List<SqlParameter> paramList = new List<SqlParameter>();
            foreach (ObjectParameter item in objParams)
            {
                SqlParameter param = null;
                //须知：sqlserver中 占位符 加 @ Oracle中 加 :
                //参数类型为数值或日期且参数值为空时，默认为 DBNull.Value
                if (item.ParameterType == typeof(int))
                {
                    param = new SqlParameter("@" + item.Name, SqlDbType.Int);
                    if (string.IsNullOrEmpty(Convert.ToString(item.Value)))
                    {
                        param.Value = DBNull.Value;
                    }
                    else
                    {
                        param.Value = Convert.ToInt32(item.Value);
                    }
                }
                else if (item.ParameterType == typeof(decimal) || item.ParameterType == typeof(double) || item.ParameterType == typeof(float))
                {
                    param = new SqlParameter("@" + item.Name, SqlDbType.Decimal);
                    if (string.IsNullOrEmpty(Convert.ToString(item.Value)))
                    {
                        param.Value = DBNull.Value;
                    }
                    else
                    {
                        param.Value = Convert.ToDecimal(item.Value);
                    }
                }
                else if (item.ParameterType == typeof(string))
                {
                    param = new SqlParameter("@" + item.Name, SqlDbType.NVarChar);
                    param.Value = Convert.ToString(item.Value);
                }
                else if (item.ParameterType == typeof(DateTime))
                {
                    param = new SqlParameter("@" + item.Name, SqlDbType.DateTime);
                    if (string.IsNullOrEmpty(Convert.ToString(item.Value)))
                    {
                        param.Value = DBNull.Value;
                    }
                    else
                    {
                        param.Value = Convert.ToDateTime(item.Value);
                    }
                }
                paramList.Add(param);
            }
            return paramList.ToArray();
        }

        #endregion

        /// <summary>
        /// 检查记录是否存在
        /// </summary>
        /// <param name="cmdText">查询语句</param>
        /// <returns>返回结果</returns>
        public static bool Exists(string cmdText)
        {
            bool bResult = Exists(cmdText, null);
            return bResult;
        }

        /// <summary>
        /// 检查记录是否存在
        /// </summary>
        /// <param name="cmdText">查询语句</param>
        /// <param name="objParams">查询语句参数</param>
        /// <returns>返回结果</returns>
        public static bool Exists(string cmdText, params ObjectParameter[] objParams)
        {
            bool bResult = false;
            switch (DbType.ToLower())
            {
                case "oracle":
                    bResult = OracleDbHelper.Exists(DbConnectionString, FormatCommandText(cmdText), ObjectParamterToOracleParameter(objParams));
                    break;
                case "sql":
                    bResult = SqlDbHelper.Exists(DbConnectionString, FormatCommandText(cmdText), ObjectParamterToSqlParameter(objParams));
                    break;
            }
            return bResult;
        }

        /// <summary>
        /// 执行查询语句，返回一个DataTable对象
        /// </summary>
        /// <param name="cmdText">查询语句</param>
        /// <returns>返回结果</returns>
        public static DataTable GetDataTable(string cmdText)
        {
            DataTable dtResult = GetDataTable(CommandType.Text, cmdText, null);
            return dtResult;
        }

        /// <summary>
        /// 执行查询语句，返回一个DataTable对象
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">查询语句</param>
        /// <param name="objParams">查询语句参数</param>
        /// <returns>返回结果</returns>
        public static DataTable GetDataTable(string cmdText, params ObjectParameter[] objParams)
        {
            DataTable dtResult = GetDataTable(CommandType.Text, cmdText, objParams);
            return dtResult;
        }

        /// <summary>
        /// 执行查询语句，返回一个DataTable对象
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">查询语句</param>
        /// <returns>返回结果</returns>
        public static DataTable GetDataTable(CommandType cmdType, string cmdText)
        {
            DataTable dtResult = GetDataTable(cmdType, cmdText, null);
            return dtResult;
        }

        /// <summary>
        /// 执行查询语句，返回一个DataTable对象
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">查询语句</param>
        /// <param name="objParams">查询语句参数</param>
        /// <returns>返回结果</returns>
        public static DataTable GetDataTable(CommandType cmdType, string cmdText, params ObjectParameter[] objParams)
        {
            DataTable dtResult = null;
            switch (DbType.ToLower())
            {
                case "oracle":
                    dtResult = OracleDbHelper.GetDataTable(DbConnectionString, cmdType, FormatCommandText(cmdText), ObjectParamterToOracleParameter(objParams));
                    break;
                case "sql":
                    dtResult = SqlDbHelper.GetDataTable(DbConnectionString, cmdType, FormatCommandText(cmdText), ObjectParamterToSqlParameter(objParams));
                    break;
            }
            return dtResult;
        }

        /// <summary>
        /// 执行一条计算查询语句，返回查询结果（object）。
        /// </summary>
        /// <param name="cmdText">查询语句</param>
        /// <returns>返回结果</returns>
        public static object GetSingle(string cmdText)
        {
            object objResult = GetSingle(cmdText, null);
            return objResult;
        }

        /// <summary>
        /// 执行一条计算查询语句，返回查询结果（object）。
        /// </summary>
        /// <param name="cmdText">查询语句</param>
        /// <param name="objParams">查询语句参数</param>
        /// <returns>返回结果</returns>
        public static object GetSingle(string cmdText, params ObjectParameter[] objParams)
        {
            object objResult = null;
            switch (DbType.ToLower())
            {
                case "oracle":
                    objResult = OracleDbHelper.GetSingle(DbConnectionString, FormatCommandText(cmdText), ObjectParamterToOracleParameter(objParams));
                    break;
                case "sql":
                    objResult = SqlDbHelper.GetSingle(DbConnectionString, FormatCommandText(cmdText), ObjectParamterToSqlParameter(objParams));
                    break;
            }
            return objResult;
        }

        /// <summary>
        /// 执行查询语句，返回DataSet对象
        /// </summary>
        /// <param name="cmdText">查询语句</param>
        /// <returns>返回结果</returns>
        public static DataSet Query(string cmdText)
        {
            DataSet dsResult = null;
            switch (DbType.ToLower())
            {
                case "oracle":
                    dsResult = OracleDbHelper.Query(DbConnectionString, cmdText);
                    break;
                case "sql":
                    dsResult = SqlDbHelper.Query(DbConnectionString, cmdText);
                    break;
            }
            return dsResult;
        }

        /// <summary>
        /// 执行查询语句，返回一个DataSet对象
        /// </summary>
        /// <param name="cmdText">查询语句</param>
        /// <returns>返回结果</returns>
        public static DataSet Query(string cmdText, params ObjectParameter[] objParams)
        {
            DataSet dsResult = Query(CommandType.Text, cmdText, objParams);
            return dsResult;
        }

        /// <summary>
        /// 执行查询语句，返回一个DataSet对象
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">查询语句</param>
        /// <returns>返回结果</returns>
        public static DataSet Query(CommandType cmdType, string cmdText)
        {
            DataSet dsResult = Query(cmdType, cmdText, null);
            return dsResult;
        }

        /// <summary>
        /// 执行查询语句，返回一个DataSet对象
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">查询语句</param>
        /// <param name="objParams">查询语句参数</param>
        /// <returns>返回结果</returns>
        public static DataSet Query(CommandType cmdType, string cmdText, params ObjectParameter[] objParams)
        {
            DataSet dsResult = null;
            switch (DbType.ToLower())
            {
                case "oracle":
                    dsResult = OracleDbHelper.Query(DbConnectionString, cmdType, FormatCommandText(cmdText), ObjectParamterToOracleParameter(objParams));
                    break;
                case "sql":
                    dsResult = SqlDbHelper.Query(DbConnectionString, cmdType, FormatCommandText(cmdText), ObjectParamterToSqlParameter(objParams));
                    break;
            }
            return dsResult;
        }

        /// <summary>
        /// 执行查询语句，返回受影响的行数，非增删改命令时返回-1
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">查询语句</param>
        /// <returns>返回结果</returns>
        public static int ExecuteNonQuery(string cmdText)
        {
            int iResult = ExecuteNonQuery(CommandType.Text, cmdText, null);
            return iResult;
        }

        /// <summary>
        /// 执行查询语句，返回受影响的行数，非增删改命令时返回-1
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">查询语句</param>
        /// <param name="objParams">查询语句参数</param>
        /// <returns>返回结果</returns>
        public static int ExecuteNonQuery(string cmdText, ObjectParameter[] objParams)
        {
            int iResult = ExecuteNonQuery(CommandType.Text, cmdText, objParams);
            return iResult;
        }

        /// <summary>
        /// 执行查询语句，返回受影响的行数，非增删改命令时返回-1
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">查询语句</param>
        /// <returns>返回结果</returns>
        public static int ExecuteNonQuery(CommandType cmdType, string cmdText)
        {
            int iResult = ExecuteNonQuery(cmdType, cmdText, null);
            return iResult;
        }

        /// <summary>
        /// 执行查询语句，返回受影响的行数，非增删改命令时返回-1
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">查询语句</param>
        /// <param name="objParams">查询语句参数</param>
        /// <returns>返回结果</returns>
        public static int ExecuteNonQuery(CommandType cmdType, string cmdText, ObjectParameter[] objParams)
        {
            int iResult = -1;
            switch (DbType.ToLower())
            {
                case "oracle":
                    iResult = OracleDbHelper.ExecuteNonQuery(DbConnectionString, cmdType, FormatCommandText(cmdText), ObjectParamterToOracleParameter(objParams));
                    break;
                case "sql":
                    iResult = SqlDbHelper.ExecuteNonQuery(DbConnectionString, cmdType, FormatCommandText(cmdText), ObjectParamterToSqlParameter(objParams));
                    break;
            }

            return iResult;
        }

        /// <summary>
        /// 重载ExecuteNonQuery方法,实现对事务处理的可选择性.
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">查询语句</param>
        /// <param name="objParams">查询语句参数</param>
        /// <param name="isTrans">是否使用事务处理</param>
        /// <returns>返回结果</returns>
        public static int ExecuteNonQuery(CommandType cmdType, string cmdText, bool isTrans, params ObjectParameter[] objParams)
        {
            int iResult = -1;
            switch (DbType.ToLower())
            {
                case "oracle":
                    iResult = OracleDbHelper.ExecuteNonQuery(DbConnectionString, cmdType, FormatCommandText(cmdText), isTrans, ObjectParamterToOracleParameter(objParams));
                    break;
                case "sql":
                    iResult = SqlDbHelper.ExecuteNonQuery(DbConnectionString, cmdType, FormatCommandText(cmdText), isTrans, ObjectParamterToSqlParameter(objParams));
                    break;
            }
            return iResult;
        }

        /// <summary>
        /// 执行查询语句，返回OracleDataReader
        /// </summary>
        /// <param name="cmdText">查询语句</param>
        /// <returns>返回结果</returns>
        public static DbDataReader ExecuteReader(string cmdText)
        {
            DbDataReader drResult = ExecuteReader(CommandType.Text, cmdText);
            return drResult;
        }

        /// <summary>
        /// 执行查询语句，返回OracleDataReader
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">查询语句</param>
        /// <param name="objParams">查询语句参数</param>
        /// <returns>返回结果</returns>
        public static DbDataReader ExecuteReader(string cmdText, params ObjectParameter[] objParams)
        {
            DbDataReader drResult = ExecuteReader(CommandType.Text, cmdText, objParams);
            return drResult;
        }

        /// <summary>
        /// 执行查询语句，返回OracleDataReader
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">查询语句</param>
        /// <returns>返回结果</returns>
        public static DbDataReader ExecuteReader(CommandType cmdType, string cmdText)
        {
            DbDataReader drResult = null;
            switch (DbType.ToLower())
            {
                case "oracle":
                    drResult = OracleDbHelper.ExecuteReader(DbConnectionString, cmdType, cmdText);
                    break;
                case "sql":
                    drResult = SqlDbHelper.ExecuteReader(DbConnectionString, cmdType, cmdText);
                    break;
            }
            return drResult;
        }

        /// <summary>
        /// 执行查询语句，返回OracleDataReader
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">查询语句</param>
        /// <param name="objParams">查询语句参数</param>
        /// <returns>返回结果</returns>
        public static DbDataReader ExecuteReader(CommandType cmdType, string cmdText, params ObjectParameter[] objParams)
        {
            DbDataReader drResult = null;
            switch (DbType.ToLower())
            {
                case "oracle":
                    drResult = OracleDbHelper.ExecuteReader(DbConnectionString, cmdType, FormatCommandText(cmdText), ObjectParamterToOracleParameter(objParams));
                    break;
                case "sql":
                    drResult = SqlDbHelper.ExecuteReader(DbConnectionString, cmdType, FormatCommandText(cmdText), ObjectParamterToSqlParameter(objParams));
                    break;
            }
            return drResult;
        }

        /// <summary>
        /// 这个方法返回sql语句执行后的第一行第一列的值
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">查询语句</param>
        /// <param name="objParams">查询语句参数</param>
        /// <returns>返回结果</returns>
        public static object ExecuteScalar(CommandType cmdType, string cmdText, params ObjectParameter[] objParams)
        {
            object objResult = null;
            switch (DbType.ToLower())
            {
                case "oracle":
                    objResult = OracleDbHelper.ExecuteScalar(DbConnectionString, cmdType, FormatCommandText(cmdText), ObjectParamterToOracleParameter(objParams));
                    break;
                case "sql":
                    objResult = SqlDbHelper.ExecuteScalar(DbConnectionString, cmdType, FormatCommandText(cmdText), ObjectParamterToSqlParameter(objParams));
                    break;
            }
            return objResult;
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>OracleDataReader</returns>
        public static DbDataReader RunProcedure(string storedProcName, IDataParameter[] parameters)
        {
            DbDataReader drResult = null;
            switch (DbType.ToLower())
            {
                case "oracle":
                    drResult = OracleDbHelper.RunProcedure(DbConnectionString, storedProcName, parameters);
                    break;
                case "sql":
                    drResult = SqlDbHelper.RunProcedure(DbConnectionString, storedProcName, parameters);
                    break;
            }
            return drResult;
        }

        ///// <summary>
        ///// 执行存储过程
        ///// </summary>
        ///// <param name="storedProcName">存储过程名</param>
        ///// <param name="parameters">存储过程参数</param>
        ///// <returns></returns>
        //public static void RunPro(string storedProcName, IDataParameter[] parameters)
        //{
        //    switch (DbType.ToLower())
        //    {
        //        case "oracle":
        //            OracleDbHelper.RunPro(DbConnectionString, storedProcName, parameters);
        //            break;
        //        case "sql":
        //            SqlDbHelper.RunPro(DbConnectionString, storedProcName, parameters);
        //            break;
        //    }
        //}


    }
}
