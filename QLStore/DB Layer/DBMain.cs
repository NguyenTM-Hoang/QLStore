using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data;
using System.Data.SqlClient;

namespace QLStore.Database_layer
{
    public class DBMain
    {
        string ConnStr = @"Data Source=DESKTOP-S3G5KIT\SQLEXPRESS;Initial Catalog=MANAGEMENT_STORE;Integrated Security=True";
        SqlConnection conn = null;
        SqlCommand comm = null;
        SqlDataAdapter da = null;
        public DBMain()
        {
            conn = new SqlConnection(ConnStr);
            comm = conn.CreateCommand();
        }
        public DataTable ExecuteQueryDataSet(string strSQL, CommandType ct)
        {
            if (conn.State == ConnectionState.Open)
                conn.Close();
            DataTable t = new DataTable();
            conn.Open();
            try
            {
                comm.CommandText = strSQL;
                comm.CommandType = ct;
                da = new SqlDataAdapter(comm);
                DataTable ds = new DataTable();
                da.Fill(ds);
                t = ds;
            }
            catch
            {

            }
            finally
            {
                conn.Close();
            }


            return t;

        }

        public void MyExecuteQuery(string strSQL, CommandType ct)
        {
            if (conn.State == ConnectionState.Open)
                conn.Close();
            conn.Open();
            comm.CommandText = strSQL;
            comm.CommandType = ct;
            try
            {
                comm.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }

            finally
            {
                conn.Close();
            }
        }

        public void MyExecuteQuery(string strSQL,CommandType ct, SqlParameter parameter)
        {
            if (conn.State == ConnectionState.Open)
                conn.Close();
            conn.Open();
            comm.Parameters.Clear();
            comm.CommandText = strSQL;
            comm.CommandType = ct;
            comm.Parameters.Add(parameter);
            try
            {
                comm.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }

            finally
            {
                conn.Close();
            }
        }

        public bool MyExecuteNonQuery(string strSQL, CommandType ct, List<SqlParameter> param)
        {
            bool f = false;
            if (conn.State == ConnectionState.Open)
                conn.Close();
            conn.Open();
            comm.Parameters.Clear();
            comm.CommandText = strSQL;
            comm.CommandType = ct;
            foreach (SqlParameter p in param)
                comm.Parameters.Add(p);
            try
            {
                comm.ExecuteNonQuery();
                f = true;
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }

            finally
            {
                conn.Close();
            }

            return f;
        }

        public string FindOneValue(string strSQL, CommandType ct, SqlParameter parameter)
        {
            if (conn.State == ConnectionState.Open)
                conn.Close();
            conn.Open();
            comm.Parameters.Clear();
            comm.CommandText = strSQL;
            comm.CommandType = ct;
            comm.Parameters.Add(parameter);
            try
            {
                da = new SqlDataAdapter(comm);
                DataTable ds = new DataTable();
                da.Fill(ds);
                if (ds.Rows.Count == 0) return "";
                else
                    return ds.Rows[0][0].ToString();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }

            finally
            {
                conn.Close();
            }
        }

        public DataTable ExecuteQueryData(string strSQL, CommandType ct,List<SqlParameter> para)
        {
            if (conn.State == ConnectionState.Open)
                conn.Close();
            conn.Open();
            DataTable t = new DataTable();
            comm.Parameters.Clear();
            try
            {
                comm.CommandText = strSQL;
                comm.CommandType = ct;
                foreach (SqlParameter p in para)
                    comm.Parameters.Add(p);
                da = new SqlDataAdapter(comm);
                DataTable ds = new DataTable();
                da.Fill(ds);
                t = ds;
            }
            catch { }
            finally
            {
                conn.Close();
            }
            
          
            return t;
        }

    }

}

