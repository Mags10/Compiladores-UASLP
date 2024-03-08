namespace compiladoresPr
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textExpresion = new System.Windows.Forms.TextBox();
            this.textPosfija = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textEva = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.labEva = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Cyan;
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(675, 110);
            this.panel1.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(237, 60);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(162, 16);
            this.label6.TabIndex = 9;
            this.label6.Text = "Juan Daniel Avalos Alaniz";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(212, 34);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(197, 16);
            this.label5.TabIndex = 8;
            this.label5.Text = "Miguel Alejandro Gutierrez Silva";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label4.Location = new System.Drawing.Point(135, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(381, 16);
            this.label4.TabIndex = 7;
            this.label4.Text = "Identificador Fundamentos de Compiladores-Deadline";
            // 
            // textExpresion
            // 
            this.textExpresion.Location = new System.Drawing.Point(55, 190);
            this.textExpresion.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textExpresion.Name = "textExpresion";
            this.textExpresion.Size = new System.Drawing.Size(180, 22);
            this.textExpresion.TabIndex = 1;
            // 
            // textPosfija
            // 
            this.textPosfija.Location = new System.Drawing.Point(55, 315);
            this.textPosfija.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textPosfija.Name = "textPosfija";
            this.textPosfija.Size = new System.Drawing.Size(180, 22);
            this.textPosfija.TabIndex = 2;
            this.textPosfija.TextChanged += new System.EventHandler(this.textPosfija_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 130);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "Analisi Lexico";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(53, 171);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(133, 16);
            this.label2.TabIndex = 4;
            this.label2.Text = "Expresion regular:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(53, 297);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 16);
            this.label3.TabIndex = 5;
            this.label3.Text = "Posfija:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(91, 218);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(144, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Convertir a posfija";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(53, 244);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(142, 16);
            this.label7.TabIndex = 8;
            this.label7.Text = "Expresion explicita:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(55, 262);
            this.textBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(180, 22);
            this.textBox1.TabIndex = 7;
            // 
            // textEva
            // 
            this.textEva.Location = new System.Drawing.Point(273, 218);
            this.textEva.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textEva.Name = "textEva";
            this.textEva.Size = new System.Drawing.Size(179, 22);
            this.textEva.TabIndex = 9;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(269, 171);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(149, 16);
            this.label8.TabIndex = 10;
            this.label8.Text = "Expresion evaluada:";
            // 
            // labEva
            // 
            this.labEva.AutoSize = true;
            this.labEva.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labEva.Location = new System.Drawing.Point(269, 194);
            this.labEva.Name = "labEva";
            this.labEva.Size = new System.Drawing.Size(65, 20);
            this.labEva.TabIndex = 11;
            this.labEva.Text = "--------";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(273, 311);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(193, 28);
            this.button2.TabIndex = 12;
            this.button2.Text = "Tabla de transiciones AFN";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(675, 466);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.labEva);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textEva);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textPosfija);
            this.Controls.Add(this.textExpresion);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textExpresion;
        private System.Windows.Forms.TextBox textPosfija;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textEva;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label labEva;
        private System.Windows.Forms.Button button2;
    }
}

