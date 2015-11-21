using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;


namespace GBE.QueryEngine
{
    /// Database Access class
    /// </summary>
    public class DataHelpe
    {
        private static string m_constr = "server=.\\SQLEXPRESS;" +
                                           "Trusted_Connection=yes;" +
                                           "database=LUBM; " +
                                           "connection timeout=30;" +
                                           "Integrated Security = SSPI;";

        public static string Constr
        {
            get { return DataHelpe.m_constr; }
            set { DataHelpe.m_constr = value; }
        }

        public DataHelpe()
        {

        }

        #region  execute simple sql query

        /// <summary>
        /// execute simple sql query, return the number of records changed
        /// </summary>
        /// <param name="SQLString">sql query</param>
        /// <returns>number of records changed</returns>
        public static int ExecuteSql(string SQLString)
        {
            using (SqlConnection conn = new SqlConnection(m_constr))
            {
                using (SqlCommand cmd = new SqlCommand(SQLString, conn))
                {
                    try
                    {
                        conn.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (SqlException E)
                    {
                        conn.Close();
                        throw new Exception(E.Message);
                    }
                }
            }
        }

        /// <summary>
        ///  execute simple sql query, return the datatable
        /// </summary>
        /// <param name="sql">sql query</param>
        /// <returns>DataTable</returns>
        public static DataTable GetDataTable(string sql)
        {
            using (SqlConnection conn = new SqlConnection(m_constr))
            {
                DataTable dt = new DataTable();
                try
                {
                    SqlDataAdapter command = new SqlDataAdapter(sql, conn);
                    command.SelectCommand.CommandTimeout = 500;
                    command.Fill(dt);
                }
                catch (SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return dt;
            }
        }


        /// <summary>
        /// execute mutiple sql queries, implement transactions
        /// </summary>
        /// <param name="SQLStringList">mutiple sql queries</param>		
        public static void ExecuteSqlTran(List<string> SQLStringList)
        {
            using (SqlConnection conn = new SqlConnection(m_constr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                SqlTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    for (int n = 0; n < SQLStringList.Count; n++)
                    {
                        string strsql = SQLStringList[n].ToString();
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                }
                catch (SqlException E)
                {
                    tx.Rollback();
                    throw new Exception(E.Message);
                }
            }
        }

        /// <summary>
        /// use proceduce
        /// </summary>
        /// <param name="procName">proceduce</param>
        /// <param name="prams">proceduce SqlParameter</param>
        /// <returns>SqlCommand</returns>
        public static SqlCommand GetCommand(string storeName, SqlParameter[] prams)
        {
            SqlConnection connection = new SqlConnection(m_constr);
            try
            {

                SqlCommand cmd = new SqlCommand(storeName, connection); //proceduce name
                cmd.CommandType = CommandType.StoredProcedure;
                connection.Open();
                if (prams != null)
                {
                    foreach (SqlParameter item in prams)
                    {
                        cmd.Parameters.Add(item);
                    }
                }
                cmd.Prepare();
                return cmd;
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// execute simple sql query, return SqlDataReader
        /// </summary>
        /// <param name="strSQL">sql query</param>
        /// <returns>SqlDataReader</returns>
        public static SqlDataReader ExecuteReader(string strSQL)
        {
            SqlConnection connection = new SqlConnection(m_constr);
            try
            {
                SqlCommand cmd = new SqlCommand(strSQL, connection);
                connection.Open();
                SqlDataReader Reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return Reader;
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }



        /// <summary>
        /// execute simple sql query, return DataSet
        /// </summary>
        /// <param name="SQLString">sql query</param>
        /// <returns>DataSet</returns>
        public static DataSet GetDataSet(string SQLString)
        {
            using (SqlConnection connection = new SqlConnection(m_constr))
            {
                DataSet ds = new DataSet();
                try
                {
                    SqlDataAdapter command = new SqlDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }

        #endregion
    }
}
