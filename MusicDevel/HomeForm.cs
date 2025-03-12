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
    // Main WinForms class
    public partial class HomeForm : Form
    {
        public HomeForm()
        {
            InitializeComponent();
            SetupForm();
        }

        // UI Components
        private TextBox txtFilename;
        private TextBox txtNotes;
        private Button btnGenerate;
        private Label lblStatus;

        private void SetupForm()
        {
            // Form properties
            this.Text = "MIDI Generator";
            //this.Size = new System.Drawing.Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Filename label and textbox
            Label lblFilename = new Label
            {
                Text = "Output Filename (e.g., guitar.mid):",
                Location = new System.Drawing.Point(20, 20),
                AutoSize = true
            };
            txtFilename = new TextBox
            {
                Location = new System.Drawing.Point(20, 40),
                Size = new System.Drawing.Size(340, 20),
                Text = "guitar.mid" // Default value
            };

            // Notes label and textbox
            Label lblNotes = new Label
            {
                Text = "Notes (e.g., E3 A3 D4 G4 B4 E5):",
                Location = new System.Drawing.Point(20, 70),
                AutoSize = true
            };
            txtNotes = new TextBox
            {
                Location = new System.Drawing.Point(20, 90),
                Size = new System.Drawing.Size(340, 20),
                Text = "E3 A3 D4 G4 B4 E5" // Default value
            };

            // Generate button
            btnGenerate = new Button
            {
                Text = "Generate MIDI",
                Location = new System.Drawing.Point(20, 120),
                Size = new System.Drawing.Size(100, 30)
            };
            btnGenerate.Click += BtnGenerate_Click;

            // Status label
            lblStatus = new Label
            {
                Text = "",
                Location = new System.Drawing.Point(20, 160),
                Size = new System.Drawing.Size(340, 20),
                ForeColor = System.Drawing.Color.Black
            };

            // Add controls to form
            this.Controls.Add(lblFilename);
            this.Controls.Add(txtFilename);
            this.Controls.Add(lblNotes);
            this.Controls.Add(txtNotes);
            this.Controls.Add(btnGenerate);
            this.Controls.Add(lblStatus);
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            try
            {

                // Define the output directory
                string outputDirectory = @"c:\@temp";
                // Combine directory with user-entered filename
                string outputFilename = Path.Combine(outputDirectory, txtFilename.Text.Trim());

                // Ensure the directory exists
                if (!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                // Get inputs from textboxes
                    string[] notes = txtNotes.Text.Trim().Split(' ');

                // Validate inputs
                if (string.IsNullOrEmpty(outputFilename))
                {
                    lblStatus.Text = "Please enter a filename.";
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                    return;
                }
                if (notes.Length == 0 || notes.All(string.IsNullOrWhiteSpace))
                {
                    lblStatus.Text = "Please enter at least one note.";
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                // Parse and generate MIDI
                PitchParser pitchParser = new PitchParser();
                var midiValues = pitchParser.Parse(notes);

                if (midiValues.Any())
                {
                    var exporter = new MidiExporter();
                    exporter.SaveToFile(outputFilename, midiValues);
                    lblStatus.Text = $"MIDI file saved as {outputFilename}";
                    lblStatus.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    lblStatus.Text = "No valid notes parsed.";
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void btnLoadSQLdata_Click(object sender, EventArgs e)
        {

            LoadSQLdata.Go();
            this.dgvMusicTable.DataSource = LoadSQLdata.musicTable;

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            /* F1 key for help */
            if (keyData == Keys.F1)
            {
                MessageBox.Show("F1 button was pressed", "Keyboard feedback...", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            /* close form on escape key */
            try
            {
                if (msg.WParam.ToInt32() == (int)Keys.Escape) this.Close();
                /*    else return base.ProcessCmdKey(ref msg, keyData); is this line needed???? */
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Key Overrided Events Error:" + Ex.Message);
            }

            /* Call the base class for normal key processing */
            return base.ProcessCmdKey(ref msg, keyData);
        }

    } // end of class


    public class PatchParser
    {
        Dictionary<string, int> patchMap = new Dictionary<string, int>();

        public PatchParser()
        {
            this.patchMap.Add("nylon", 25);
            this.patchMap.Add("steel", 26);
            this.patchMap.Add("jazz", 27);
            this.patchMap.Add("clean", 28);
            this.patchMap.Add("muted", 29);
            this.patchMap.Add("distortion", 31);
            this.patchMap.Add("bass", 33);
            this.patchMap.Add("violin", 41);
            this.patchMap.Add("viola", 42);
            this.patchMap.Add("cello", 43);
            this.patchMap.Add("sitar", 105);
            this.patchMap.Add("banjo", 106);
            this.patchMap.Add("fiddle", 111);
        }

        public int Patch(string value)
        {
            const int DefaultPatch = 25;
            int patch = DefaultPatch;
            try
            {
                patch = this.patchMap[value.ToLower()];
            }
            catch (KeyNotFoundException)
            {
                patch = DefaultPatch;
            }
            return patch;
        }
    }

    public class PitchParser
    {
        Dictionary<char, int> pitchOffsets;
        Dictionary<char, int> pitchModifiers;

        public PitchParser()
        {
            this.pitchOffsets = new Dictionary<char, int>();
            this.pitchModifiers = new Dictionary<char, int>();
            this.pitchOffsets.Add('c', 0);
            this.pitchOffsets.Add('d', 2);
            this.pitchOffsets.Add('e', 4);
            this.pitchOffsets.Add('f', 5);
            this.pitchOffsets.Add('g', 7);
            this.pitchOffsets.Add('a', 9);
            this.pitchOffsets.Add('b', 11);
            this.pitchModifiers.Add('#', 1);
            this.pitchModifiers.Add('b', -1);
        }

        public Pitch PitchFromString(string midiNotation)
        {
            const int MinimumStringLength = 2;
            if (midiNotation.Length < MinimumStringLength)
                throw new ArgumentException();
            string name = FindName(midiNotation);
            string modifier = FindModifier(midiNotation);
            int octave = FindOctave(midiNotation);
            int midiValue = CalculateMidiValue(name, modifier, octave);
            return new Pitch(midiValue);
        }

        public IEnumerable<Pitch> Parse(IEnumerable<string> noteList)
        {
            var list = new List<Pitch>();
            noteList.Where(note => !String.IsNullOrWhiteSpace(note)).ToList().ForEach(note => list.Add(this.PitchFromString(note)));
            return list;
        }

        private string FindName(string midiNotation)
        {
            return midiNotation.Substring(0, 1).ToLower();
        }

        private string FindModifier(string midiNotation)
        {
            const int MaximumStringLength = 3;
            if (midiNotation.Length == MaximumStringLength)
            {
                return midiNotation.Substring(1, 1).ToLower();
            }
            return string.Empty;
        }

        private int FindOctave(string midiNotation)
        {
            const int MaximumStringLength = 3;
            int startIndex = 1;
            if (midiNotation.Length == MaximumStringLength)
            {
                startIndex = 2;
            }
            return Int32.Parse(midiNotation.Substring(startIndex, 1));
        }

        private int CalculateMidiValue(string noteName, string modifier, int octave)
        {
            int value = 0;
            try
            {
                value = this.pitchOffsets[noteName[0]];
                if (!String.IsNullOrEmpty(modifier))
                {
                    value += this.pitchModifiers[modifier[0]];
                }
            }
            catch (KeyNotFoundException)
            {
                value = 0;
            }
            const int SemitonesInOctave = 12;
            return value + (SemitonesInOctave * octave);
        }
    }

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
        public void SaveToFile(string fileName, IEnumerable<Pitch> allNotes)
        {
            const int MidiFileType = 0;
            const int BeatsPerMinute = 60;
            const int TicksPerQuarterNote = 120;
            const int TrackNumber = 0;
            const int ChannelNumber = 1;
            long absoluteTime = 0;
            var collection = new MidiEventCollection(MidiFileType, TicksPerQuarterNote);
            collection.AddEvent(new TextEvent("Note Stream", MetaEventType.TextEvent, absoluteTime), TrackNumber);
            ++absoluteTime;
            collection.AddEvent(new TempoEvent(CalculateMicrosecondsPerQuaterNote(BeatsPerMinute), absoluteTime), TrackNumber);
            var patchParser = new PatchParser();
            int patchNumber = patchParser.Patch("steel");
            collection.AddEvent(new PatchChangeEvent(0, ChannelNumber, patchNumber), TrackNumber);
            const int NoteVelocity = 100;
            const int NoteDuration = 3 * TicksPerQuarterNote / 4;
            const long SpaceBetweenNotes = TicksPerQuarterNote;
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
    }
}
