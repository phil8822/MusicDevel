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
            this.btnLoadSQLdata = new System.Windows.Forms.Button();
            this.dgvMusicTable = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMusicTable)).BeginInit();
            this.SuspendLayout();
            // 
            // btnLoadSQLdata
            // 
            this.btnLoadSQLdata.Location = new System.Drawing.Point(118, 156);
            this.btnLoadSQLdata.Name = "btnLoadSQLdata";
            this.btnLoadSQLdata.Size = new System.Drawing.Size(413, 127);
            this.btnLoadSQLdata.TabIndex = 0;
            this.btnLoadSQLdata.Text = "Load Data from SQL";
            this.btnLoadSQLdata.UseVisualStyleBackColor = true;
            this.btnLoadSQLdata.Click += new System.EventHandler(this.btnLoadSQLdata_Click);
            // 
            // dgvMusicTable
            // 
            this.dgvMusicTable.AllowUserToAddRows = false;
            this.dgvMusicTable.AllowUserToDeleteRows = false;
            this.dgvMusicTable.AllowUserToOrderColumns = true;
            this.dgvMusicTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMusicTable.Location = new System.Drawing.Point(118, 361);
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
            this.ClientSize = new System.Drawing.Size(2304, 1524);
            this.Controls.Add(this.dgvMusicTable);
            this.Controls.Add(this.btnLoadSQLdata);
            this.Name = "HomeForm";
            this.Text = "Music Development";
            ((System.ComponentModel.ISupportInitialize)(this.dgvMusicTable)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnLoadSQLdata;
        private System.Windows.Forms.DataGridView dgvMusicTable;
    }
}

