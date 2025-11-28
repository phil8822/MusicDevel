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
            this.btnGo = new System.Windows.Forms.Button();
            this.dgvMusicTable = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMusicTable)).BeginInit();
            this.SuspendLayout();
            // 
            // btnGo
            // 
            this.btnGo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGo.Location = new System.Drawing.Point(118, 156);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(413, 127);
            this.btnGo.TabIndex = 0;
            this.btnGo.Text = "Go";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGO_Click);
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
            this.Controls.Add(this.btnGo);
            this.Name = "HomeForm";
            this.Text = "Music Development";
            ((System.ComponentModel.ISupportInitialize)(this.dgvMusicTable)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.DataGridView dgvMusicTable;
    }
}

