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

            // Set DataGridView column headers to bold font and centered
            DataGridViewCellStyle headerStyle = new DataGridViewCellStyle();
            headerStyle.Font = new Font(dgvMusicTable.Font, FontStyle.Bold);
            headerStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMusicTable.ColumnHeadersDefaultCellStyle = headerStyle;

            // Set default cell style to centered
            DataGridViewCellStyle cellStyle = new DataGridViewCellStyle();
            cellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMusicTable.DefaultCellStyle = cellStyle;


            // Notes Data: Convert IEnumerable<int> to IEnumerable<Pitch>
            var pitchNotes = MyNotes().Select(note => new Pitch(note));

            // Midi disc file creation
            var exporter = new MidiExporter();
            exporter.CreateMidiFile(outputFilename, pitchNotes);
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

    } // end of class

    public class Pitch
    {
        const int MinimumMidiValue = 0;
        const int MaximumMidiValue = 255;

        public Pitch(int value)
        {
            this.MidiValue = Math.Min(MaximumMidiValue, Math.Max(value, MinimumMidiValue));
        }

        public int MidiValue { get; private set; }
    }

    public class MidiExporter
    {
        public void CreateMidiFile(string fileName, IEnumerable<Pitch> allNotes)
        {
            int MidiFileType = 0;
            int BeatsPerMinute = 60;
            int TicksPerQuarterNote = 120;
            int NoteDuration = 3 * TicksPerQuarterNote / 4;
            long SpaceBetweenNotes = TicksPerQuarterNote;
            long absoluteTime = 0;
            int TrackNumber = 0;
            int ChannelNumber = 1;
            int patchNumber = 1; // temporary fixed value Acoutic Grand Piano
            int NoteVelocity = 100;

            // at absoluteTime = 0
            var midiEvts = new MidiEventCollection(MidiFileType, TicksPerQuarterNote);
            midiEvts.AddEvent(new TextEvent("Note Stream", MetaEventType.TextEvent, absoluteTime), TrackNumber);
            ++absoluteTime;

            // at absoluteTime = 1
            midiEvts.AddEvent(new TempoEvent(CalculateMicrosecondsPerQuaterNote(BeatsPerMinute), absoluteTime), TrackNumber);
            midiEvts.AddEvent(new PatchChangeEvent(0, ChannelNumber, patchNumber), TrackNumber);

            foreach (var note in allNotes)
            {
                midiEvts.AddEvent(new NoteOnEvent(absoluteTime, ChannelNumber, note.MidiValue, NoteVelocity, NoteDuration), TrackNumber);
                midiEvts.AddEvent(new NoteEvent(absoluteTime + NoteDuration, ChannelNumber, MidiCommandCode.NoteOff, note.MidiValue, 0), TrackNumber);
                absoluteTime += SpaceBetweenNotes;
            }

            midiEvts.PrepareForExport();
            MidiFile.Export(fileName, midiEvts);
        }

        private static int CalculateMicrosecondsPerQuaterNote(int bpm)
        {
            return 60 * 1000 * 1000 / bpm;
        }

    }  // end of class
} // end of namespace
