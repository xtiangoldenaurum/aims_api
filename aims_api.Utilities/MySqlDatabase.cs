using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Utilities
{
    public class MySqlDatabase : IDisposable
    {
        public MySqlConnection Conn;

        public MySqlDatabase()
        {
            Conn = new MySqlConnection();
        }

        public void Open(string connectionString)
        {
            Conn.ConnectionString = connectionString;
            Conn.Open();
        }

        public void Dispose()
        {
            Conn.Close();
        }
    }
}
