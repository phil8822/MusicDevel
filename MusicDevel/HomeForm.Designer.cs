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
            this.SuspendLayout();
            // 
            // btnLoadSQLdata
            // 
            this.btnLoadSQLdata.Location = new System.Drawing.Point(948, 486);
            this.btnLoadSQLdata.Name = "btnLoadSQLdata";
            this.btnLoadSQLdata.Size = new System.Drawing.Size(413, 127);
            this.btnLoadSQLdata.TabIndex = 0;
            this.btnLoadSQLdata.Text = "Load Data from SQL";
            this.btnLoadSQLdata.UseVisualStyleBackColor = true;
            this.btnLoadSQLdata.Click += new System.EventHandler(this.btnLoadSQLdata_Click);
            // 
            // HomeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2304, 1524);
            this.Controls.Add(this.btnLoadSQLdata);
            this.Name = "HomeForm";
            this.Text = "Music Development";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnLoadSQLdata;
    }
}

