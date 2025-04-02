using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.SoundFont;
using NAudio.Midi;

namespace MusicDevel
{
    public class GetSQLdata
    {
        const int startBar = 1;               // leftover from original code in AutoMuse 
        const int ticksPerQtrNote = 96;       // adopted from VFP 
        const int colCount = 8;               // 4 for four quarter notes; value is 8 when eigth-notes allowed 
        const int onCommand = 143;		      // dec144/hex90 if note-on channel 1, 91=chan 2, etc  
        const int harmonyChannel = 4;         // adopted from VFP 
        const int ticksPerQuarterNote = 120;  // ** REDUNDANT TO Class HomeForm **
        const int ticksPerHalfBeat = ticksPerQuarterNote / 2;
        static string connectionString = Properties.Settings.Default.CnxStringHome;

        public static DataTable tblInv = new DataTable();
        public static DataTable melodyMidiEvents = new DataTable();
        public static DataTable harmonyData = new DataTable();
        public static DataTable hamonyMidiNotes = new DataTable();
        public static DataTable harmonyMidiEvents = new DataTable();

        public static void Melody() 
        {
            // Clear existing columns and rows in music tables
            melodyMidiEvents.Clear();
            melodyMidiEvents.Columns.Clear();

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
                        adapter.Fill(melodyMidiEvents);
                    }
                }
            }

            // Add Duration column and other columns and compute values
            melodyMidiEvents.Columns.Add("Duration", typeof(int));
            melodyMidiEvents.Columns.Add("StartHalfBeat", typeof(int));
            melodyMidiEvents.Columns.Add("EndHalfBeat", typeof(int));
            melodyMidiEvents.Columns.Add("AbsoluteTime", typeof(int));
            melodyMidiEvents.Columns.Add("DurationTicks", typeof(int));

            // Calculate StartHalfBeat values
            for (int i = 0; i < melodyMidiEvents.Rows.Count; i++)
            {
                DataRow row = melodyMidiEvents.Rows[i];
                int measure = Convert.ToInt32(row["Measure"]);
                int note = Convert.ToInt32(row["Note"]);
                int startHalfBeat = 8 * (measure - 1) + note;
                row["StartHalfBeat"] = startHalfBeat;
            }

            int absoluteTime = 0;
            int durationTicks = 0;

            // Calculate EndHalfBeat and Duration values
            for (int i = 0; i < melodyMidiEvents.Rows.Count; i++)
            {
                DataRow row = melodyMidiEvents.Rows[i];
                int startHalfBeat = Convert.ToInt32(row["StartHalfBeat"]);
                int endHalfBeat = (i < melodyMidiEvents.Rows.Count - 1) ? Convert.ToInt32(melodyMidiEvents.Rows[i + 1]["StartHalfBeat"]) : startHalfBeat + 1;
                row["EndHalfBeat"] = endHalfBeat;
                row["Duration"] = endHalfBeat - startHalfBeat;
                durationTicks =  ticksPerHalfBeat * (endHalfBeat - startHalfBeat);
                row["DurationTicks"] = durationTicks;
                row["AbsoluteTime"] = absoluteTime;
                
                absoluteTime += durationTicks;

                // Last row
                if (i == melodyMidiEvents.Rows.Count -1 ) 
                {
                    int lastHalfBeat = 8 * Convert.ToInt32(row["Measure"]);
                    row["EndHalfBeat"] = lastHalfBeat;
                    row["Duration"] = lastHalfBeat - startHalfBeat + 1;

                    durationTicks = ticksPerHalfBeat * (lastHalfBeat - startHalfBeat + 1);
                    row["DurationTicks"] = durationTicks;
                }
            }
        }

        public static void Harmony(int keyShift, int octaveShift)
        {
            // Supporting tables
            SupportingTables();
            Inversion Inv = new Inversion();

            // Clear existing columns and rows in music tables
            harmonyData.Clear();
            harmonyData.Columns.Clear();
            harmonyMidiEvents.Clear();

            // Copy melodyEvents table structure to harmonyEvents
            harmonyMidiEvents = melodyMidiEvents.Clone(); 

            // Local database query
            string sqlQry =
$@"
if 1 = 1
  SELECT 
  [Measure],
  [Note],
  [IntVal],
  [StrVal],
  [Inversion],
  [Octaveshift],
  [Pattern],
  [Percussion]
  ,c.Xsymbol ,c.NoteCount, c.Tone1, c.Tone2, c.Tone3, c.Tone4
  ,s.description, s.n01, s.n02, s.n03, s.n04, s.n05, s.n06, s.n07, s.n08
FROM [MyDemo].[dbo].[AutoMuse2]
LEFT JOIN Chords c
  ON LTRIM(RTRIM(AutoMuse2.StrVal)) = LTRIM(RTRIM(c.Xsymbol))
LEFT JOIN StrumPatterns s
  ON AutoMuse2.Pattern = s.ID
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
                        adapter.Fill(harmonyData);
                    }
                }
            }

            // Populate "previous" row to row zero if needed by "repeat"
            DataRow previousRow = harmonyData.Rows[0];


            // Iterate thru harmonyData rows
            foreach (DataRow r in harmonyData.Rows)
            {
                // Implement repeat row 
                if ((r["StrVal"].ToString().Trim()) == "repeat")
                {
                    foreach (DataColumn dc in harmonyData.Columns)
                    {
                        if (dc.ColumnName.ToString().Trim() == "Measure") continue;
                        if (dc.ColumnName.ToString().Trim() == "Beat") continue;
                        r[dc] = previousRow[dc];
                    }
                }
                else
                {
                    previousRow = r;
                }

                // Bail-out if missing data 
                if (
                    DBNull.Value.Equals(r["IntVal"])
                    | DBNull.Value.Equals(r["StrVal"])
                    | DBNull.Value.Equals(r["Pattern"])
                    | DBNull.Value.Equals(r["NoteCount"])
                    )
                { continue; }

                /* Iterate through data, create notes */
                int first = (int)r["Note"];
                int last = first + 3;

                for (double subBeat = first; subBeat <= last; subBeat++)
                {
                    bool makeChord = false;
                    int velocityB = (int)r["n0" + subBeat.ToString("G")];
                    if (velocityB > 0) makeChord = true;

                    if (makeChord)
                    {
                        /* iterate through notes in chord */
                        for (int k = 1; k <= Convert.ToInt32(r["NoteCount"]); k++)
                        {
                            int myTone;
                            int chordOctaveShift;
                            int chordInversion;

                            if (r["OctaveShift"] == DBNull.Value)
                            {
                                chordOctaveShift = 0;
                            }
                            else
                            {
                                chordOctaveShift = Convert.ToInt16(r["OctaveShift"]);
                            }

                            if (r["Inversion"] == DBNull.Value)
                            {
                                chordInversion = 0;
                            }
                            else
                            {
                                chordInversion = Convert.ToInt16(r["Inversion"]);
                            }

                            myTone =
                                (int)r["IntVal"]
                                + (int)r["Tone" + k.ToString("G")]
                                + keyShift + 12 * octaveShift
                                + 12 * chordOctaveShift
                                + 12 * Inv.inversion(Convert.ToInt32(r["NoteCount"]), k, chordInversion);

                            int bar = (int)r["measure"] - startBar + 1;
                            double myTick = (4 * ticksPerQtrNote) * ((bar - 1) + ((subBeat - 1) / colCount));
                            int midiCommand = onCommand + harmonyChannel;

                            hamonyMidiNotes.Rows.Add(
                                harmonyChannel, bar, subBeat, (int)myTick, 0, midiCommand, myTone, velocityB, 0);
                        }
                    }
                }
            }

            foreach (DataRow r in hamonyMidiNotes.Rows) 
            {
                DataRow newRow = harmonyMidiEvents.NewRow();
                newRow["Measure"] = r["Bar"];
                newRow["Note"] = r["SubBeat"];
                newRow["IntVal"] = r["I1"];
                newRow["Duration"] = 1;
                newRow["StartHalfBeat"] = 8 * (Convert.ToInt32(r["Bar"]) - 1) + Convert.ToInt32(r["SubBeat"]);
                newRow["EndHalfBeat"] = 8 * (Convert.ToInt32(r["Bar"]) - 1) + Convert.ToInt32(r["SubBeat"]) + 1;
                newRow["AbsoluteTime"] = Convert.ToInt32(r["Tick"]);
                newRow["DurationTicks"] = 96;
                harmonyMidiEvents.Rows.Add(newRow);
            }
        
        }


        public static void SupportingTables()
        {
            // Inversion  
            tblInv = new DataTable();
            tblInv.Columns.Add("Disp", typeof(string));
            tblInv.Columns.Add("Value", typeof(int));
            tblInv.Rows.Add(new object[] { "-2", -2 });
            tblInv.Rows.Add(new object[] { "-1", -1 });
            tblInv.Rows.Add(new object[] { " 0", 0 });
            tblInv.Rows.Add(new object[] { "+1", +1 });
            tblInv.Rows.Add(new object[] { "+2", +2 });

            // Harmon Midi Notes
            hamonyMidiNotes = new DataTable();
            List<string> colNames = new List<string>() { "Channel", "Bar", "SubBeat", "Tick", "Di", "I1", "I2", "I3", "I4" };
            foreach (string name in colNames)
            {
                hamonyMidiNotes.Columns.Add(name, typeof(int));
            }
            hamonyMidiNotes.DefaultView.Sort = ("Tick ASC, I1 ASC");
        }



    } // end of class GetSQLdata


    class Inversion
    {
        public int inversion(int totalNotes, int noteNumber, int invert)
        {
            int rtn = 0;

            /* rule out inversion >= number of notes */
            if (Math.Abs(invert) >= totalNotes)
            {
                rtn = 0;
            }

            /* lower inversion (typical) */
            else if (invert < 0 & invert - noteNumber <= -totalNotes - 1)
            {
                rtn = -1;
            }

            /* higher inversion */
            else if (invert > 0 & invert - noteNumber >= 0)
            {
                rtn = +1;
            }

            /* probably bad data */
            else
            {
                rtn = 0;
            }

            return rtn;
        }
    } // end of class Inversion

} // end of namespace MusicDevel
