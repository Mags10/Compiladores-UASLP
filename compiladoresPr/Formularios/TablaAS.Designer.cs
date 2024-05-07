namespace compiladoresPr.Formularios
{
    partial class TablaAS
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
            this.TabAS = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.TabAS)).BeginInit();
            this.SuspendLayout();
            // 
            // TabAS
            // 
            this.TabAS.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.TabAS.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.TabAS.ColumnHeadersHeight = 29;
            this.TabAS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabAS.Location = new System.Drawing.Point(0, 0);
            this.TabAS.Name = "TabAS";
            this.TabAS.RowHeadersWidth = 51;
            this.TabAS.RowTemplate.Height = 24;
            this.TabAS.Size = new System.Drawing.Size(840, 464);
            this.TabAS.TabIndex = 0;
            this.TabAS.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.TabAS_CellContentClick);
            // 
            // TablaAS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(840, 464);
            this.Controls.Add(this.TabAS);
            this.Name = "TablaAS";
            this.Text = "TablaAS";
            this.Load += new System.EventHandler(this.TablaAS_Load);
            ((System.ComponentModel.ISupportInitialize)(this.TabAS)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView TabAS;
    }
}