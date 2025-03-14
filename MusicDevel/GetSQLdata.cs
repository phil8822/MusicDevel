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
    public class GetSQLdata
    {

        static string connectionString = Properties.Settings.Default.CnxStringHome;
        public static DataTable musicTable = new DataTable();

        public static void Go() 
        {


            // Local database query
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


            // Add Duration column and compute values
            musicTable.Columns.Add("Duration", typeof(int));
            musicTable.Columns.Add("StartSubeat", typeof(int));
            musicTable.Columns.Add("EndSubeat", typeof(int));

            // Calculate StartSubeat values
            for (int i = 0; i < musicTable.Rows.Count; i++)
            {
                DataRow row = musicTable.Rows[i];
                int measure = Convert.ToInt32(row["Measure"]);
                int note = Convert.ToInt32(row["Note"]);
                int startSubeat = 8 * (measure - 1) + note;
                row["StartSubeat"] = startSubeat;
            }

            // Calculate EndSubeat and Duration values
            for (int i = 0; i < musicTable.Rows.Count; i++)
            {
                DataRow row = musicTable.Rows[i];
                int startSubeat = Convert.ToInt32(row["StartSubeat"]);
                int endSubeat = (i < musicTable.Rows.Count - 1) ? Convert.ToInt32(musicTable.Rows[i + 1]["StartSubeat"]) : startSubeat + 1;
                row["EndSubeat"] = endSubeat;
                row["Duration"] = endSubeat - startSubeat;

                if (i == musicTable.Rows.Count -1 ) 
                {
                    int lastSubeat = 8* Convert.ToInt32(row["Measure"]);
                    row["EndSubeat"] = lastSubeat;
                    row["Duration"] = endSubeat - startSubeat;
                }
            }


        }
    } // end of class GetSQLdata
} // end of namespace MusicDevel
