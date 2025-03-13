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

        const string outputFilename = @"c:\@temp\cs.mid";

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
            LoadSQLdata.Go();
            this.dgvMusicTable.DataSource = LoadSQLdata.musicTable;

            // Convert IEnumerable<int> to IEnumerable<Pitch>
            var pitchNotes = MyNotes().Select(note => new Pitch(note));

            // Midi file creation
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

            var collection = new MidiEventCollection(MidiFileType, TicksPerQuarterNote);
            collection.AddEvent(new TextEvent("Note Stream", MetaEventType.TextEvent, absoluteTime), TrackNumber);
            ++absoluteTime;
            collection.AddEvent(new TempoEvent(CalculateMicrosecondsPerQuaterNote(BeatsPerMinute), absoluteTime), TrackNumber);
            collection.AddEvent(new PatchChangeEvent(0, ChannelNumber, patchNumber), TrackNumber);

            foreach (var note in allNotes)
            {
                collection.AddEvent(new NoteOnEvent(absoluteTime, ChannelNumber, note.MidiValue, NoteVelocity, NoteDuration), TrackNumber);
                collection.AddEvent(new NoteEvent(absoluteTime + NoteDuration, ChannelNumber, MidiCommandCode.NoteOff, note.MidiValue, 0), TrackNumber);
                absoluteTime += SpaceBetweenNotes;
            }

            collection.PrepareForExport();
            MidiFile.Export(fileName, collection);
        }

        private static int CalculateMicrosecondsPerQuaterNote(int bpm)
        {
            return 60 * 1000 * 1000 / bpm;
        }

    }  // end of class
} // end of namespace
