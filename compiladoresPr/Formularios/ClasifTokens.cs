using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace compiladoresPr.Formularios
{
    public partial class ClasifTokens : Form
    {
        Tiny Tiny;
        public ClasifTokens(Tiny form)
        {
            Tiny = form;
            InitializeComponent();
        }

        private void ClasifTokens_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Tiny.closedClfToken();
        }
    }
}
