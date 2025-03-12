using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicDevel
{
    public class LoadSQLdata
    {

        static string connectionString = Properties.Settings.Default.CnxStringHome;
        public static DataTable musicTable = new DataTable();

        public static void Go() 
        {


            string sqlQry =
$@"
  SELECT 
  [Measure]
  ,[Note]
  ,[IntVal]
  FROM [MyDemo].[dbo].[AutoMuse2]
  WHERE [SongID] = 5 and [Version] = 1 and [Track] = 1
  ORDER BY [SongID], [Track], [Measure], [Note]
";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sqlQry, connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(musicTable);
                    }
                }
            }
        }
    }
}
