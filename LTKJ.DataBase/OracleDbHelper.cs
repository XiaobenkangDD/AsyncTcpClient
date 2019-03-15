using System;
using System.Collections;
using System.Configuration;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace LTKJ.DataBase
{
    /// <summary>
    /// Oracle数据库操作类库
    /// </summary>
    public abstract class OracleDbHelper
    {

        /// <summary>
        /// 获取配置文件中的数据库连接字符串
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                if (ConfigurationManager.ConnectionStrings["OracleConnection"] != null)
                    return ConfigurationManager.ConnectionStrings["OracleConnection"].ConnectionString;
                else
                    return null;
            }
        }

        //创建一个哈希表用来缓存参数
        private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// 检查记录是否存在
        /// </summary>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="cmdText">查询语句</param>
        /// <param name="cmdParams">查询语句参数</param>
        /// <returns>返回结果</returns>
        public static bool Exists(string connString, string cmdText, params OracleParameter[] cmdParams)
        {
            object obj = OracleDbHelper.GetSingle(connString, cmdText, cmdParams);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 执行一条计算查询语句，返回查询结果（object）。
        /// </summary>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="cmdText">查询语句</param>
        /// <param name="cmdParams">查询语句参数</param>
        /// <returns>返回结果</returns>
        public static object GetSingle(string connString, string cmdText, params OracleParameter[] cmdParams)
        {
            using (OracleConnection connection = new OracleConnection(connString))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, CommandType.Text, cmdText, cmdParams);
                        object obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (OracleException e)
                    {
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回DataSet对象
        /// </summary>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="cmdText">查询语句</param>
        /// <returns>返回结果</returns>
        public static DataSet Query(string connString, string cmdText)
        {
            using (OracleConnection connection = new OracleConnection(connString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    OracleDataAdapter command = new OracleDataAdapter(cmdText, connection);
                    command.Fill(ds, "ds");
                }
                catch (OracleException ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
                return ds;
            }
        }

        /// <summary>
        /// 执行查询语句，返回一个DataSet对象
        /// </summary>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">查询语句</param>
        /// <param name="cmdParams">查询语句参数</param>
        /// <returns>返回结果</returns>
        public static DataSet Query(string connString, CommandType cmdType, string cmdText, params OracleParameter[] cmdParams)
        {
            using (OracleConnection connection = new OracleConnection(connString))
            {
                OracleCommand cmd = new OracleCommand();

                PrepareCommand(cmd, connection, null, cmdType, cmdText, cmdParams);
                using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (OracleException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        if (connection.State != ConnectionState.Closed)
                        {
                            connection.Close();
                        }
                    }
                    return ds;
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回一个DataTable对象
        /// </summary>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">查询语句</param>
        /// <param name="cmdParams">查询语句参数</param>
        /// <returns>返回结果</returns>
        public static DataTable GetDataTable(string connString, CommandType cmdType, string cmdText, params OracleParameter[] cmdParams)
        {
            DataTable dt = new DataTable();
            OracleDataReader dr = ExecuteReader(connString, cmdType, cmdText, cmdParams);
            dt.Load(dr, LoadOption.Upsert);
            return dt;
        }

        /// <summary>
        /// 执行查询语句，返回受影响的行数，非增删改命令时返回-1
        /// </summary>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">查询语句</param>
        /// <param name="cmdParams">查询语句参数</param>
        /// <returns>返回结果</returns>
        public static int ExecuteNonQuery(string connString, CommandType cmdType, string cmdText, params OracleParameter[] cmdParams)
        {
            // Create a new Oracle command
            OracleCommand cmd = new OracleCommand();

            //Create a connection
            using (OracleConnection connection = new OracleConnection(connString))
            {
                //Prepare the command
                PrepareCommand(cmd, connection, null, cmdType, cmdText, cmdParams);

                //Execute the command
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// 重载ExecuteNonQuery方法,实现对事务处理的可选择性.
        /// </summary>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">查询语句</param>
        /// <param name="cmdParams">查询语句参数</param>
        /// <param name="isTrans">是否使用事务处理</param>
        /// <returns>返回结果</returns>
        public static int ExecuteNonQuery(string connString, CommandType cmdType, string cmdText, bool isTrans, params OracleParameter[] cmdParams)
        {
            int val = 0;
            if (isTrans)
            {
                //使用事务处理
                // Create a new Oracle command
                OracleCommand cmd = new OracleCommand();

                OracleTransaction trans = null;

                //Create a connection
                using (OracleConnection connection = new OracleConnection(connString))
                {
                    connection.Open();
                    //Start a local transaction
                    trans = connection.BeginTransaction(IsolationLevel.ReadCommitted);

                    //Prepare the command
                    PrepareCommand(cmd, connection, trans, cmdType, cmdText, cmdParams);

                    try
                    {

                        //Execute the command
                        val = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                    }
                    finally
                    {
                        connection.Close();
                    }

                    return val;
                }
            }
            else
            {
                //非事务处理
                val = ExecuteNonQuery(connString, cmdType, cmdText, cmdParams);
            }
            return val;
        }

        /// <summary>
        /// Execute an OracleCommand (that returns no resultset) against an existing database transaction 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "PublishOrders", new OracleParameter(":prodid", 24));
        /// </remarks>
        /// <param name="trans">an existing database transaction</param>
        /// <param name="cmdType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="cmdText">the stored procedure name or PL/SQL command</param>
        /// <param name="cmdParams">an array of OracleParamters used to execute the command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(OracleTransaction trans, CommandType cmdType, string cmdText, params OracleParameter[] cmdParams)
        {
            OracleCommand cmd = new OracleCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, cmdParams);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// Execute an OracleCommand (that returns no resultset) against an existing database connection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new OracleParameter(":prodid", 24));
        /// </remarks>
        /// <param name="conn">an existing database connection</param>
        /// <param name="cmdType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="cmdText">the stored procedure name or PL/SQL command</param>
        /// <param name="cmdParams">an array of OracleParamters used to execute the command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(OracleConnection connection, CommandType cmdType, string cmdText, params OracleParameter[] cmdParams)
        {

            OracleCommand cmd = new OracleCommand();
            PrepareCommand(cmd, connection, null, cmdType, cmdText, cmdParams);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// Execute a select query that will return a result set
        /// </summary>
        /// <param name="connString">Connection string</param>
        /// <param name="cmdType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="cmdText">the stored procedure name or PL/SQL command</param>
        /// <param name="cmdParams">an array of OracleParamters used to execute the command</param>
        /// <returns>返回结果</returns>
        public static OracleDataReader ExecuteReader(string connString, CommandType cmdType, string cmdText, params OracleParameter[] cmdParams)
        {

            //Create the command and connection
            OracleCommand cmd = new OracleCommand();
            OracleConnection conn = new OracleConnection(connString);
            try
            {
                //Prepare the command to execute
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParams);
                //Execute the query, stating that the connection should close when the resulting datareader has been read
                OracleDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;

            }
            catch
            {
                //If an error occurs close the connection as the reader will not be used and we expect it to close the connection
                conn.Close();
                throw;
            }
        }

        /// <summary>
        /// Execute a select query that will return a result set
        /// </summary>
        /// <param name="connString">Connection string</param>
        /// <param name="cmdType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="cmdText">the stored procedure name or PL/SQL command</param>
        /// <returns>返回结果</returns>
        public static OracleDataReader ExecuteReader(string connString, CommandType cmdType, string cmdText)
        {
            OracleConnection conn = new OracleConnection(connString);
            if (conn.State != ConnectionState.Open)
                conn.Open();
            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = cmdText;
            cmd.CommandType = cmdType;
            cmd.Connection = conn;

            try
            {
                OracleDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return rdr;
            }
            catch (Exception ex)
            {
                conn.Close();
                throw ex;
            }
        }

        /// <summary>
        /// Execute an OracleCommand that returns the first column of the first record against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new OracleParameter(":prodid", 24));
        /// </remarks>
        /// <param name="connString">a valid connection string for a SqlConnection</param>
        /// <param name="cmdType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="cmdText">the stored procedure name or PL/SQL command</param>
        /// <param name="cmdParams">an array of OracleParamters used to execute the command</param>
        /// <returns>An object that should be converted to the expected type using Convert.To{Type}</returns>
        public static object ExecuteScalar(string connString, CommandType cmdType, string cmdText, params OracleParameter[] cmdParams)
        {
            OracleCommand cmd = new OracleCommand();

            using (OracleConnection conn = new OracleConnection(connString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParams);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        ///	<summary>
        ///	Execute	a OracleCommand (that returns a 1x1 resultset)	against	the	specified SqlTransaction
        ///	using the provided parameters.
        ///	</summary>
        ///	<param name="transaction">A	valid SqlTransaction</param>
        ///	<param name="cmdType">The CommandType (stored procedure, text, etc.)</param>
        ///	<param name="cmdText">The stored procedure name	or PL/SQL command</param>
        ///	<param name="cmdParams">An array of	OracleParamters used to execute the command</param>
        ///	<returns>An	object containing the value	in the 1x1 resultset generated by the command</returns>
        public static object ExecuteScalar(OracleTransaction trans, CommandType cmdType, string cmdText, params OracleParameter[] cmdParams)
        {
            if (trans == null)
                throw new ArgumentNullException("transaction");
            if (trans != null && trans.Connection == null)
                throw new ArgumentException("The transaction was rollbacked	or commited, please	provide	an open	transaction.", "transaction");

            // Create a	command	and	prepare	it for execution
            OracleCommand cmd = new OracleCommand();

            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, cmdParams);

            // Execute the command & return	the	results
            object retval = cmd.ExecuteScalar();

            // Detach the SqlParameters	from the command object, so	they can be	used again
            cmd.Parameters.Clear();
            return retval;
        }

        /// <summary>
        /// Execute an OracleCommand that returns the first column of the first record against an existing database connection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  Object obj = ExecuteScalar(conn, CommandType.StoredProcedure, "PublishOrders", new OracleParameter(":prodid", 24));
        /// </remarks>
        /// <param name="conn">an existing database connection</param>
        /// <param name="cmdType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="cmdText">the stored procedure name or PL/SQL command</param>
        /// <param name="cmdParams">an array of OracleParamters used to execute the command</param>
        /// <returns>An object that should be converted to the expected type using Convert.To{Type}</returns>
        public static object ExecuteScalar(OracleConnection conn, CommandType cmdType, string cmdText, params OracleParameter[] cmdParams)
        {
            OracleCommand cmd = new OracleCommand();

            PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParams);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// Add a set of parameters to the cached
        /// </summary>
        /// <param name="cacheKey">Key value to look up the parameters</param>
        /// <param name="cmdParams">Actual parameters to cached</param>
        public static void CacheParameters(string cacheKey, params OracleParameter[] cmdParams)
        {
            parmCache[cacheKey] = cmdParams;
        }

        /// <summary>
        /// Fetch parameters from the cache
        /// </summary>
        /// <param name="cacheKey">Key to look up the parameters</param>
        /// <returns>返回结果</returns>
        public static OracleParameter[] GetCachedParameters(string cacheKey)
        {
            OracleParameter[] cachedParms = (OracleParameter[])parmCache[cacheKey];

            if (cachedParms == null)
                return null;

            // If the parameters are in the cache
            OracleParameter[] clonedParms = new OracleParameter[cachedParms.Length];

            // return a copy of the parameters
            for (int i = 0, j = cachedParms.Length; i < j; i++)
                clonedParms[i] = (OracleParameter)((ICloneable)cachedParms[i]).Clone();

            return clonedParms;
        }

        /// <summary>
        /// Internal function to prepare a command for execution by the database
        /// </summary>
        /// <param name="cmd">Existing command object</param>
        /// <param name="conn">Database connection object</param>
        /// <param name="trans">Optional transaction object</param>
        /// <param name="cmdType">Command type, e.g. stored procedure</param>
        /// <param name="cmdText">Command test</param>
        /// <param name="cmdParams">Parameters for the command</param>
        private static void PrepareCommand(OracleCommand cmd, OracleConnection conn, OracleTransaction trans, CommandType cmdType, string cmdText, OracleParameter[] cmdParams)
        {

            //Open the connection if required
            if (conn.State != ConnectionState.Open)
                conn.Open();

            //Set up the command
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = cmdType;

            //Bind it to the transaction if it exists
            if (trans != null)
                cmd.Transaction = trans;

            // Bind the parameters passed in
            if (cmdParams != null)
            {
                cmd.Parameters.Clear();
                foreach (OracleParameter parm in cmdParams)
                    cmd.Parameters.Add(parm);
            }
        }

        /// <summary>
        /// Converter to use boolean data type with Oracle
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>返回结果</returns>
        public static string OraBit(bool value)
        {
            if (value)
                return "Y";
            else
                return "N";
        }

        /// <summary>
        /// Converter to use boolean data type with Oracle
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>返回结果</returns>
        public static bool OraBool(string value)
        {
            if (value.Equals("Y"))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>OracleDataReader</returns>
        public static OracleDataReader RunProcedure(string connString, string storedProcName, IDataParameter[] parameters)
        {
            OracleConnection connection = new OracleConnection(connString);
            OracleDataReader returnReader;
            connection.Open();
            OracleCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            command.CommandType = CommandType.StoredProcedure;
            returnReader = command.ExecuteReader();
            return returnReader;
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns></returns>
        public static void RunPro(string storedProcName, IDataParameter[] parameters)
        {
            OracleCommand command = new OracleCommand();
            command.CommandText = storedProcName;//声明存储过程名
            command.CommandType = CommandType.StoredProcedure;
            foreach (OracleParameter parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }
            command.ExecuteNonQuery();//执行存储过程          
        }

        /// <summary>
        /// 构建 OracleCommand 对象(用来返回一个结果集，而不是一个整数值)
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>OracleCommand</returns>
        private static OracleCommand BuildQueryCommand(OracleConnection conn, string storedProcName, IDataParameter[] parameters)
        {
            OracleCommand command = new OracleCommand(storedProcName, conn);
            command.CommandType = CommandType.StoredProcedure;
            foreach (OracleParameter parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }
            return command;
        }


    }
}
