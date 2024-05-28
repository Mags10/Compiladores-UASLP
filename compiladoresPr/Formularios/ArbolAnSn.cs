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
    public partial class ArbolAnSn : Form
    {
        Tiny tiny;
        public ArbolAnSn(Tiny tiny)
        {
            this.tiny = tiny;
            InitializeComponent();
        }

        private void ArbolAnSn_FormClosed(object sender, FormClosedEventArgs e)
        {
            tiny.closedArbolAn();
        }
    }
}
