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
            MySqlConnection conn = DBUtils.GetDBConnection();
            MySqlCommand cmd = conn.CreateCommand();

            string sql_select = "SELECT * from our_screens";
            Form1 f = new Form1();
            cmd.Connection = conn;
            cmd.CommandText = sql_select;


            MySqlDataReader reader;


            try
            {
                cmd.Connection.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    dataGridView1.Rows.Add(reader["link"], reader["vid_id"]);
                }
                reader.Close();

            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error: \r\n{ 0}" + ex.ToString(), "Error", MessageBoxButtons.OK);
            }

            int cols = dataGridView1.RowCount;
            int sc = 0;
            int k = 0;
            for (int i = 0; i < 18; i++)
            {           
                for (int j = 0; j < 12; j++)
                {
                    PictureBox pic2 = new PictureBox();
                    pic2.Location = new Point(pic2.Location.X + (i * 301), pic2.Location.Y + (j * 151));
                    pic2.BorderStyle = BorderStyle.FixedSingle;
                    pic2.SizeMode = PictureBoxSizeMode.StretchImage;
                    pic2.Height = 150;
                    pic2.Width = 300;
                    pic2.ImageLocation = dataGridView1[0, sc].Value.ToString();
                    sc++;
                    pic1[k] = pic2;
                    Controls.Add(pic1[k]);
                    k++;
                    if (k > i)
                    {
                        k = 0;
                    }
                }                                
            }
        }
    }
}
