using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Configuration;

namespace DataAccessLayer.GenericRepository
{
    public class DapperGenericRepository
    {
        private readonly string _connectionString = String.Empty;

        public DapperGenericRepository(string connectionString = null)
        {
            if (connectionString != null)
            {
                _connectionString = connectionString;
            }
            else
            {
                _connectionString = ConfigurationManager.ConnectionStrings["dbConnection"].ConnectionString;
            }
        }
        public SqlConnection GetOpenConnection()
        {
            SqlConnection con = new SqlConnection(_connectionString);
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }
            return con;
        }

        public List<T> Query<T>(string query, object parameters = null)
        {
            try
            {
                using (SqlConnection db = GetOpenConnection())
                {
                    return db.Query<T>(query, parameters, commandType: CommandType.StoredProcedure).ToList();
                }
            }
            catch (Exception)
            {

                return null;
            }
        }

        public T QueryFirstOrDefault<T>(string query, object parameters = null)
        {
            try
            {
                using (SqlConnection db = GetOpenConnection())
                {
                    return db.QueryFirstOrDefault<T>(query, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception)
            {

                return default;
            }

        }

        public int Execute(string query, object parameters = null)
        {
            int sonuc = 0;
            try
            {
                using (SqlConnection db = GetOpenConnection())
                {
                    try
                    {

                        return sonuc = db.Execute(query, parameters, commandType: CommandType.StoredProcedure);

                    }
                    catch (Exception)
                    {

                        return sonuc;
                    }
                    finally
                    {
                        db.Close();
                        db.Dispose();
                    }
                }
            }
            catch (Exception)
            {

                return sonuc;
            }


        }

        public T QuerySingleOrDefault<T>(string query, object parameters = null)
        {
            try
            {
                using (SqlConnection db = GetOpenConnection())
                {
                    return db.QuerySingleOrDefault<T>(query, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception)
            {
                return default;


            }
        }
    }
}
