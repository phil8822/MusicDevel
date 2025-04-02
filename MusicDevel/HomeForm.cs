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

        private void btnLoadGO_Click(object sender, EventArgs e)
        {
            // SQL tasks
            GetSQLdata.Melody();
            GetSQLdata.Harmony(0, 0);  // temp values, get data from Song properties

            // Midi disc file creation
            var exporter = new MidiExporter();
            exporter.CreateMidiFile(@"c:\@temp\cs3.mid", GetSQLdata.melodyMidiEvents);

            SetupDGV();
        }

        private void SetupDGV()
        {
            this.dgvMusicTable.DataSource = GetSQLdata.melodyMidiEvents;

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
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Key Overrided Events Error:" + Ex.Message);
            }

            // Call the base class for normal key processing 
            return base.ProcessCmdKey(ref msg, keyData);
        }

    } // end of class HomeForm

} // end of namespace
