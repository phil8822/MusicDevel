using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Midi;
using System.Diagnostics;
using System.IO;


namespace MusicDevel
{
    public partial class HomeForm : Form
    {

        const string outputFilename = @"c:\@temp\cs2.mid";

        public HomeForm()
        {
            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            // Form properties
            this.Text = "MIDI Generator";
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void btnLoadSQLdata_Click(object sender, EventArgs e)
        {
            // SQL tasks
            GetSQLdata.Go();
            this.dgvMusicTable.DataSource = GetSQLdata.musicTable;

            // Set DataGridView columns to width 80
            foreach (DataGridViewColumn column in dgvMusicTable.Columns)
            {
                column.Width = 80;
            }

            // Set DataGridView column headers to bold font, centered, and wrap text
            DataGridViewCellStyle headerStyle = new DataGridViewCellStyle();
            headerStyle.Font = new Font(dgvMusicTable.Font, FontStyle.Bold);
            headerStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            headerStyle.WrapMode = DataGridViewTriState.True;
            dgvMusicTable.ColumnHeadersDefaultCellStyle = headerStyle;

            // Set default cell style to centered
            DataGridViewCellStyle cellStyle = new DataGridViewCellStyle();
            cellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMusicTable.DefaultCellStyle = cellStyle;

            // Midi disc file creation
            var exporter = new MidiExporter();
            exporter.CreateMidiFile(@"c:\@temp\cs3.mid", GetSQLdata.musicTable);
        }

        public static IEnumerable<int> MyNotes()
        {
            return new List<int> { 60, 62, 64, 65, 67, 69, 71, 72 };
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // F1 key for help 
            if (keyData == Keys.F1)
            {
                MessageBox.Show("F1 button was pressed", "Keyboard feedback...", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // close form on escape key
            try
            {
                if (msg.WParam.ToInt32() == (int)Keys.Escape) this.Close();
                // //  else return base.ProcessCmdKey(ref msg, keyData); is this line needed??
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Key Overrided Events Error:" + Ex.Message);
            }

            // Call the base class for normal key processing 
            return base.ProcessCmdKey(ref msg, keyData);
        }

    } // end of class HomeForm


    public class MidiExporter
    {

        public void CreateMidiFile(string fileName, DataTable musicTable)
        {
            int MidiFileType = 0;
            int BeatsPerMinute = 120;
            int TicksPerQuarterNote = 120;
            int NoteDuration = 4 * TicksPerQuarterNote / 4;
            long SpaceBetweenNotes = TicksPerQuarterNote;
            long absoluteTime = 0;
            int TrackNumber = 0;
            int ChannelNumber = 1;
            int patchNumber = 1; // temporary fixed value Acoutic Grand Piano
            int NoteVelocity = 100;

            var midiEvts = new MidiEventCollection(MidiFileType, TicksPerQuarterNote);
            midiEvts.AddEvent(new TextEvent("Note Stream", MetaEventType.TextEvent, absoluteTime), TrackNumber);

            midiEvts.AddEvent(new TempoEvent(MicrosecondsPerQuaterNote(BeatsPerMinute), absoluteTime), TrackNumber);
            midiEvts.AddEvent(new PatchChangeEvent(0, ChannelNumber, patchNumber), TrackNumber);

            for (int i = 0; i < musicTable.Rows.Count; i++)
            {
                DataRow row = musicTable.Rows[i];
                absoluteTime = Convert.ToInt32(row["AbsoluteTime"]);
                int midiValue = Convert.ToInt32(row["IntVal"]);
                int durationTicks = Convert.ToInt32(row["DurationTicks"]);

                midiEvts.AddEvent(new NoteOnEvent(absoluteTime, ChannelNumber, midiValue, NoteVelocity, durationTicks), TrackNumber);
                midiEvts.AddEvent(new NoteEvent(absoluteTime + durationTicks, ChannelNumber, MidiCommandCode.NoteOff, midiValue, 0), TrackNumber);
            }

            midiEvts.PrepareForExport();
            MidiFile.Export(fileName, midiEvts);
        }


        private static int MicrosecondsPerQuaterNote(int bpm)
        {
            return 60 * 1000 * 1000 / bpm;
        }

    }  // end of class
} // end of namespace
