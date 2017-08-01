   using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using Microsoft.Office.Interop;


namespace MHData
{
    public static class OleDbHelper
    {

        public static string ACCESS_CONNECTIONSTRING= @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=E:\MyProjects\GitHubRepo\MedcialAndHealth.accdb;";

        private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());

        public static int ExecuteNonQuery(string connString, string sqlQuery)
        {
            OleDbCommand cmd = new OleDbCommand();

            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;
                cmd.CommandText = sqlQuery;
                   
                int val = cmd.ExecuteNonQuery(); 
                cmd.Parameters.Clear();
                conn.Close();
                return val;
            }
        }
        public static int ExecuteNonQuery(OleDbConnection conn, OleDbTransaction trans, string sqlQuery)
        {
                OleDbCommand cmd = new OleDbCommand();
             
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;
                cmd.CommandText = sqlQuery;
                cmd.Transaction = trans;
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
               
                return val; 
        }
        public static int ExecuteNonQuery(string connString, CommandType cmdType, string cmdText, params OleDbParameter[] cmdParms)
        {
            OleDbCommand cmd = new OleDbCommand();

            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms, ConnectionActionType.Open);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        public static int ExecuteNonQuery(OleDbConnection conn, CommandType cmdType, string cmdText, params OleDbParameter[] cmdParms)
        {
            OleDbCommand cmd = new OleDbCommand();
            PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms, ConnectionActionType.AutoDetection);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        public static int ExecuteNonQuery(OleDbTransaction trans, CommandType cmdType, string cmdText, params OleDbParameter[] cmdParms)
        {
            OleDbCommand cmd = new OleDbCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, cmdParms, ConnectionActionType.None);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        public static OleDbDataReader ExecuteReader(string connString, CommandType cmdType, string cmdText, params OleDbParameter[] cmdParms)
        {
            OleDbCommand cmd = new OleDbCommand();
            OleDbConnection conn = new OleDbConnection(connString);
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms, ConnectionActionType.Open);
                OleDbDataReader rdr = cmd.ExecuteReader();
                cmd.Parameters.Clear();
                return rdr;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }
        public static OleDbDataReader ExecuteReader(string connString, string sqlQuery)
        {
            OleDbCommand cmd = new OleDbCommand();
            OleDbConnection conn = new OleDbConnection(connString);
            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;
                cmd.CommandText = sqlQuery;
                 
                OleDbDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                 
                return rdr;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }

         public static OleDbDataReader ExecuteReader(string connString, string sqlQuery, params OleDbParameter[] cmdParms)
        {
            OleDbCommand cmd = new OleDbCommand();
            OleDbConnection conn = new OleDbConnection(connString);
            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;
                cmd.CommandText = sqlQuery;

                if (cmdParms != null)
                {
                    foreach (OleDbParameter parm in cmdParms)
                        cmd.Parameters.Add(parm);
                }
                OleDbDataReader rdr = cmd.ExecuteReader();
                cmd.Parameters.Clear();
                return rdr;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }
         public static OleDbDataReader ExecuteReader(OleDbConnection conn, CommandType cmdType, string cmdText, params OleDbParameter[] cmdParms)
        {
            OleDbCommand cmd = new OleDbCommand();
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms, ConnectionActionType.AutoDetection);
                OleDbDataReader rdr = cmd.ExecuteReader();
                cmd.Parameters.Clear();
                return rdr;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }

        public static object ExecuteScalar(string connString, CommandType cmdType, string cmdText, params OleDbParameter[] cmdParms)
        {
            OleDbCommand cmd = new OleDbCommand();
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms, ConnectionActionType.Open);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }
        public static object ExecuteScalar(OleDbConnection conn,OleDbTransaction trans,  string sqlquery)
        {
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;
                cmd.Transaction = trans;
                cmd.CommandType = CommandType.Text ;
                cmd.CommandText = sqlquery;
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val; 
        }
        public static object ExecuteScalar(string connString,  string sqlQuery)
        {
            OleDbCommand cmd = new OleDbCommand();
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;
                cmd.CommandText = sqlQuery;

                object val = cmd.ExecuteScalar();
                
                return val;
            }
        }
       
        public static object ExecuteScalar(OleDbConnection conn, CommandType cmdType, string cmdText, params OleDbParameter[] cmdParms)
        {
            OleDbCommand cmd = new OleDbCommand();
            PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms, ConnectionActionType.AutoDetection);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }


        public static DataSet ExecuteDataset(string connString, CommandType cmdType, string cmdText, params OleDbParameter[] cmdParms)
        {
            OleDbCommand cmd = new OleDbCommand();
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms, ConnectionActionType.Open);
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                cmd.Parameters.Clear();
                return ds;
            }
        }


        public static DataSet ExecuteDataset(OleDbConnection conn, CommandType cmdType, string cmdText, params OleDbParameter[] cmdParms)
        {
            OleDbCommand cmd = new OleDbCommand();
            PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms, ConnectionActionType.AutoDetection);
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);                                                               
            DataSet ds = new DataSet();
            da.Fill(ds);
            cmd.Parameters.Clear();
            return ds;
        }
        public static DataTable ExecuteDataTable(string connString, string sqlQuery)
        {
            OleDbCommand cmd = new OleDbCommand();
            OleDbConnection conn = new OleDbConnection(connString);
            DataTable dt = null;
            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;
                cmd.CommandText = sqlQuery;
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);
                cmd.Parameters.Clear();
                conn.Close();
                return dt;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }
        public static DataTable ExecuteDataTable(string connString, string sqlQuery, params OleDbParameter[] cmdParms)
        {
             
            OleDbConnection conn = new OleDbConnection(connString);
            OleDbCommand cmd = new OleDbCommand(); 
            DataTable dt = null;
            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                if (cmdParms != null)
                {
                    foreach (OleDbParameter parm in cmdParms)
                        cmd.Parameters.Add(parm);
                }
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;
               cmd.CommandText = sqlQuery;
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);
                cmd.Parameters.Clear();
                conn.Close();
                return dt;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }

        public static void CacheParameters(string cacheKey, params OleDbParameter[] cmdParms)
        {
            parmCache[cacheKey] = cmdParms;
        }


        public static OleDbParameter[] GetCachedParameters(string cacheKey)
        {
            OleDbParameter[] cachedParms = (OleDbParameter[])parmCache[cacheKey];

            if (cachedParms == null)
                return null;

            OleDbParameter[] clonedParms = new OleDbParameter[cachedParms.Length];
            for (int i = 0, j = cachedParms.Length; i < j; i++)
                clonedParms[i] = (OleDbParameter)((ICloneable)cachedParms[i]).Clone();

            return clonedParms;
        }


        private static void PrepareCommand(OleDbCommand cmd, OleDbConnection conn, OleDbTransaction trans, CommandType cmdType, string cmdText, OleDbParameter[] cmdParms, ConnectionActionType connActionType)
        {
            if (connActionType == ConnectionActionType.Open)
            {
                conn.Open();
            }
            else
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
            }

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {
                foreach (OleDbParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }

        public static DataTable ExecutePager(string connString, int pageIndex, int pageSize, string fileds, string table, string where, string order, out int pageCount, out int recordCount, string id)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 10;
            if (string.IsNullOrEmpty(fileds)) fileds = "*";
            if (string.IsNullOrEmpty(order)) order = "ID desc";
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                string myVw = string.Format(" {0} ", table);
                string sqlText = string.Format(" select count(0) as recordCount from {0} {1}", myVw, where);
                OleDbCommand cmdCount = new OleDbCommand(sqlText, conn);
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                recordCount = Convert.ToInt32(cmdCount.ExecuteScalar());

                if ((recordCount % pageSize) > 0)
                    pageCount = recordCount / pageSize + 1;
                else
                    pageCount = recordCount / pageSize;
                OleDbCommand cmdRecord;
                if (pageIndex == 1)
                {
                    cmdRecord = new OleDbCommand(string.Format("select top {0} {1} from {2} {3} order by {4} ", pageSize, fileds, myVw, where, order), conn);
                }
                else if (pageIndex > pageCount)
                {
                    cmdRecord = new OleDbCommand(string.Format("select top {0} {1} from {2} {3} order by {4} ", pageSize, fileds, myVw, "where 1=2", order), conn);
                }
                else
                {
                    int pageLowerBound = pageSize * pageIndex;
                    int pageUpperBound = pageLowerBound - pageSize;
                    string recordIDs = RecordID(string.Format("select top {0} {1} from {2} {3} order by {4} ", pageLowerBound, id, myVw, where, order), pageUpperBound, conn);
                    cmdRecord = new OleDbCommand(string.Format("select {0} from {1} where {4} in ({2}) order by {3} ", fileds, myVw, recordIDs, order, id), conn);

                }
                OleDbDataAdapter dataAdapter = new OleDbDataAdapter(cmdRecord);
                DataTable dt = new DataTable();
                dataAdapter.Fill(dt);
                return dt;
            }
        }

        private static string RecordID(string query, int passCount, OleDbConnection conn)
        {
            OleDbCommand cmd = new OleDbCommand(query, conn);
            string result = string.Empty;
            using (IDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    if (passCount < 1)
                    {
                        result += "," + dr.GetInt32(0);
                    }
                    passCount--;
                }
            }
            return result.Substring(1);
        }

         public static void ExportToExcel(DataTable Tbl, string ExcelFilePath = null)
        {
            try
            {
                if (Tbl == null || Tbl.Columns.Count == 0)
                    throw new Exception("ExportToExcel: Null or empty input table!\n");

                // load excel, and create a new workbook
                Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
                excelApp.Workbooks.Add();

                // single worksheet
                Microsoft.Office.Interop.Excel._Worksheet workSheet = excelApp.ActiveSheet;

                // column headings
                for (int i = 0; i < Tbl.Columns.Count; i++)
                {
                    workSheet.Cells[1, (i + 1)] = Tbl.Columns[i].ColumnName;
                }

                // rows
                for (int i = 0; i < Tbl.Rows.Count; i++)
                {
                    // to do: format datetime values before printing
                    for (int j = 0; j < Tbl.Columns.Count; j++)
                    {
                        workSheet.Cells[(i + 2), (j + 1)] = Tbl.Rows[i][j];
                    }
                }

                // check fielpath
                if (ExcelFilePath != null && ExcelFilePath != "")
                {
                    try
                    {
                        workSheet.SaveAs(ExcelFilePath);
                        excelApp.Quit();
                      //  MessageBox.Show("Excel file saved!");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("ExportToExcel: Excel file could not be saved! Check filepath.\n"
                            + ex.Message);
                    }
                }
                else    // no filepath is given
                {
                    excelApp.Visible = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ExportToExcel: \n" + ex.Message);
            }
        }

        enum ConnectionActionType
        {
            None = 0,
            AutoDetection = 1,
            Open = 2
        }
    }
}
    
