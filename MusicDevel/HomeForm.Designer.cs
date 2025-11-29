namespace MusicDevel
{
    partial class HomeForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnCreateMidi = new System.Windows.Forms.Button();
            this.dgvMusicTable = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMusicTable)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCreateMidi
            // 
            this.btnCreateMidi.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCreateMidi.Location = new System.Drawing.Point(118, 156);
            this.btnCreateMidi.Name = "btnCreateMidi";
            this.btnCreateMidi.Size = new System.Drawing.Size(413, 127);
            this.btnCreateMidi.TabIndex = 0;
            this.btnCreateMidi.Text = "Create MIDI files";
            this.btnCreateMidi.UseVisualStyleBackColor = true;
            this.btnCreateMidi.Click += new System.EventHandler(this.btnCreateMidi_Click);
            // 
            // dgvMusicTable
            // 
            this.dgvMusicTable.AllowUserToAddRows = false;
            this.dgvMusicTable.AllowUserToDeleteRows = false;
            this.dgvMusicTable.AllowUserToOrderColumns = true;
            this.dgvMusicTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMusicTable.Location = new System.Drawing.Point(76, 1257);
            this.dgvMusicTable.Name = "dgvMusicTable";
            this.dgvMusicTable.ReadOnly = true;
            this.dgvMusicTable.RowHeadersVisible = false;
            this.dgvMusicTable.RowHeadersWidth = 82;
            this.dgvMusicTable.RowTemplate.Height = 33;
            this.dgvMusicTable.Size = new System.Drawing.Size(1594, 560);
            this.dgvMusicTable.TabIndex = 1;
            // 
            // HomeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2388, 1864);
            this.Controls.Add(this.dgvMusicTable);
            this.Controls.Add(this.btnCreateMidi);
            this.Name = "HomeForm";
            this.Text = "Music Development";
            ((System.ComponentModel.ISupportInitialize)(this.dgvMusicTable)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCreateMidi;
        private System.Windows.Forms.DataGridView dgvMusicTable;
    }
}

