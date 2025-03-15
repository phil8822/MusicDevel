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
        const int TicksPerQuarterNote = 120;  // ** REDUNDANT TO Class HomeForm **
        const int TicksPerHalfBeat = TicksPerQuarterNote / 2 ; 
        static string connectionString = Properties.Settings.Default.CnxStringHome;
        public static DataTable musicTable = new DataTable();

        public static void Go() 
        {
            // Clear existing columns and rows in musicTable
            musicTable.Clear();
            musicTable.Columns.Clear();

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
            musicTable.Columns.Add("StartHalfBeat", typeof(int));
            musicTable.Columns.Add("EndHalfBeat", typeof(int));
            musicTable.Columns.Add("AbsoluteTime", typeof(int));
            musicTable.Columns.Add("DurationTicks", typeof(int));

            // Calculate StartHalfBeat values
            for (int i = 0; i < musicTable.Rows.Count; i++)
            {
                DataRow row = musicTable.Rows[i];
                int measure = Convert.ToInt32(row["Measure"]);
                int note = Convert.ToInt32(row["Note"]);
                int startHalfBeat = 8 * (measure - 1) + note;
                row["StartHalfBeat"] = startHalfBeat;
            }

            int absoluteTime = 0;
            int durationTicks = 0;

            // Calculate EndHalfBeat and Duration values
            for (int i = 0; i < musicTable.Rows.Count; i++)
            {
                DataRow row = musicTable.Rows[i];
                int startHalfBeat = Convert.ToInt32(row["StartHalfBeat"]);
                int endHalfBeat = (i < musicTable.Rows.Count - 1) ? Convert.ToInt32(musicTable.Rows[i + 1]["StartHalfBeat"]) : startHalfBeat + 1;
                row["EndHalfBeat"] = endHalfBeat;
                row["Duration"] = endHalfBeat - startHalfBeat;
                durationTicks =  TicksPerHalfBeat * (endHalfBeat - startHalfBeat);
                row["DurationTicks"] = durationTicks;
                row["AbsoluteTime"] = absoluteTime;
                
                absoluteTime += durationTicks;

                // Last row
                if (i == musicTable.Rows.Count -1 ) 
                {
                    int lastHalfBeat = 8 * Convert.ToInt32(row["Measure"]);
                    row["EndHalfBeat"] = lastHalfBeat;
                    row["Duration"] = lastHalfBeat - startHalfBeat + 1;

                    durationTicks = TicksPerHalfBeat * (lastHalfBeat - startHalfBeat + 1);
                    row["DurationTicks"] = durationTicks;
                }
            }
        }
    } // end of class GetSQLdata
} // end of namespace MusicDevel
