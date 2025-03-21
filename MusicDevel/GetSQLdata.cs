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
        public static DataTable melodyTable = new DataTable();
        public static DataTable harmonyTable = new DataTable();

        public static void Melody() 
        {
            // Clear existing columns and rows in music tables
            melodyTable.Clear();
            melodyTable.Columns.Clear();

            // Local database query
            string sqlQry =
$@"
  SELECT 
  [Measure]
  ,[Note]
  ,[IntVal]
  FROM [MyDemo].[dbo].[AutoMuse2]
  WHERE [SongID] = 5 and [Version] = 0 and [Track] = 1 and [IntVal] <= 127
  ORDER BY [SongID], [Track], [Measure], [Note]
";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sqlQry, connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(melodyTable);
                    }
                }
            }


            // Add Duration column and compute values
            melodyTable.Columns.Add("Duration", typeof(int));
            melodyTable.Columns.Add("StartHalfBeat", typeof(int));
            melodyTable.Columns.Add("EndHalfBeat", typeof(int));
            melodyTable.Columns.Add("AbsoluteTime", typeof(int));
            melodyTable.Columns.Add("DurationTicks", typeof(int));

            // Calculate StartHalfBeat values
            for (int i = 0; i < melodyTable.Rows.Count; i++)
            {
                DataRow row = melodyTable.Rows[i];
                int measure = Convert.ToInt32(row["Measure"]);
                int note = Convert.ToInt32(row["Note"]);
                int startHalfBeat = 8 * (measure - 1) + note;
                row["StartHalfBeat"] = startHalfBeat;
            }

            int absoluteTime = 0;
            int durationTicks = 0;

            // Calculate EndHalfBeat and Duration values
            for (int i = 0; i < melodyTable.Rows.Count; i++)
            {
                DataRow row = melodyTable.Rows[i];
                int startHalfBeat = Convert.ToInt32(row["StartHalfBeat"]);
                int endHalfBeat = (i < melodyTable.Rows.Count - 1) ? Convert.ToInt32(melodyTable.Rows[i + 1]["StartHalfBeat"]) : startHalfBeat + 1;
                row["EndHalfBeat"] = endHalfBeat;
                row["Duration"] = endHalfBeat - startHalfBeat;
                durationTicks =  TicksPerHalfBeat * (endHalfBeat - startHalfBeat);
                row["DurationTicks"] = durationTicks;
                row["AbsoluteTime"] = absoluteTime;
                
                absoluteTime += durationTicks;

                // Last row
                if (i == melodyTable.Rows.Count -1 ) 
                {
                    int lastHalfBeat = 8 * Convert.ToInt32(row["Measure"]);
                    row["EndHalfBeat"] = lastHalfBeat;
                    row["Duration"] = lastHalfBeat - startHalfBeat + 1;

                    durationTicks = TicksPerHalfBeat * (lastHalfBeat - startHalfBeat + 1);
                    row["DurationTicks"] = durationTicks;
                }
            }
        }

        public static void Harmony()
        {
            // Clear existing columns and rows in music tables
            harmonyTable.Clear();
            harmonyTable.Columns.Clear();

            // Local database query
            string sqlQry =
$@"
  SELECT 
  [Idx],
  [SongID],
  [Version],
  [Measure],
  [Note],
  [Track],
  [IntVal],
  [StrVal],
  [Inversion],
  [Octaveshift],
  [Pattern],
  [Percussion]
FROM [MyDemo].[dbo].[AutoMuse2]
LEFT JOIN Chords 
  ON LTRIM(RTRIM(AutoMuse2.StrVal)) = LTRIM(RTRIM(Chords.Xsymbol))
LEFT JOIN StrumPatterns 
  ON AutoMuse2.Pattern = StrumPatterns.ID
WHERE [SongID] = 5 
  AND [Version] = 0 
  AND [Track] = 4
ORDER BY [SongID], [Track], [Measure], [Note];
";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sqlQry, connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(harmonyTable);
                    }
                }
            }


            // Add Duration column and compute values
            harmonyTable.Columns.Add("Duration", typeof(int));
            harmonyTable.Columns.Add("StartHalfBeat", typeof(int));
            harmonyTable.Columns.Add("EndHalfBeat", typeof(int));
            harmonyTable.Columns.Add("AbsoluteTime", typeof(int));
            harmonyTable.Columns.Add("DurationTicks", typeof(int));

            // Calculate StartHalfBeat values
            for (int i = 0; i < harmonyTable.Rows.Count; i++)
            {
                DataRow row = harmonyTable.Rows[i];
                int measure = Convert.ToInt32(row["Measure"]);
                int note = Convert.ToInt32(row["Note"]);
                int startHalfBeat = 8 * (measure - 1) + note;
                row["StartHalfBeat"] = startHalfBeat;
            }

            int absoluteTime = 0;
            int durationTicks = 0;

            // Calculate EndHalfBeat and Duration values
            for (int i = 0; i < harmonyTable.Rows.Count; i++)
            {
                DataRow row = harmonyTable.Rows[i];
                int startHalfBeat = Convert.ToInt32(row["StartHalfBeat"]);
                int endHalfBeat = (i < harmonyTable.Rows.Count - 1) ? Convert.ToInt32(melodyTable.Rows[i + 1]["StartHalfBeat"]) : startHalfBeat + 1;
                row["EndHalfBeat"] = endHalfBeat;
                row["Duration"] = endHalfBeat - startHalfBeat;
                durationTicks = TicksPerHalfBeat * (endHalfBeat - startHalfBeat);
                row["DurationTicks"] = durationTicks;
                row["AbsoluteTime"] = absoluteTime;

                absoluteTime += durationTicks;

                // Last row
                if (i == harmonyTable.Rows.Count - 1)
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
