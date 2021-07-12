using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;
using Dapper;
using System.Configuration;

namespace cpu_monitor
{
    public class DatabaseDataAccess
    {
        public static List<PointModel> SelectAll()
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = conn.Query<PointModel>("SELECT * FROM Points", new DynamicParameters());
                return output.ToList();
            }
        }

        public static void AddPoint(PointModel point)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                conn.Execute("INSERT INTO Points (Cpu, Created_at) VALUES (@Cpu, @Created_at)", point);
            }
        }

        private static string LoadConnectionString(string id = "Database")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}