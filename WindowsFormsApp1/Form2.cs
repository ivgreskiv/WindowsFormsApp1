using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {

        PictureBox[] pic1 = new PictureBox[12];

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 12; i++)
            {
                PictureBox pic2 = new PictureBox();
                pic2.Location = new Point(pic2.Location.X + (i * 100), pic2.Location.Y);
                pic2.BorderStyle = BorderStyle.FixedSingle;
                pic1[i] = pic2;
                Controls.Add(pic1[i]);
            }
        }
    }
}
