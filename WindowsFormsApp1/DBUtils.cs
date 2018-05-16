using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace WindowsFormsApp1
{
    class DBUtils
    {
        public static MySqlConnection GetDBConnection()
        {
            string host = "127.0.0.1";
            int port = 3306;
            string database = "videos";
            string username = "root";
            string password = "mysql";

            return DBMySQLUtils.GetDBConnection(host, port, database, username, password);
        }
    }
}
