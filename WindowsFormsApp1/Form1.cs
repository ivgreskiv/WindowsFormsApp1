using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using Clarifai.API;
using Clarifai.DTOs.Inputs;
using MySql.Data.MySqlClient;


namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string link = textBox1.Text;

            pictureBox1.ImageLocation = link;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

        }

        private async void button2_Click(object sender, EventArgs e)
        {
            var client = new ClarifaiClient("f5d04074f20343b787952a2605d34b3a");

            var res = await client.PublicModels.GeneralModel.Predict(new ClarifaiURLImage(textBox1.Text)).ExecuteAsync();
            foreach (var concept in res.Get().Data)
            {
                dataGridView1.Rows.Add($"{concept.Name}", $"{concept.Value}");
            }

            var res1 = await client.PublicModels.NsfwModel.Predict(new ClarifaiURLImage(textBox1.Text)).ExecuteAsync();
            foreach (var concept in res1.Get().Data)
            {
                dataGridView1.Rows.Add($"{concept.Name}", $"{concept.Value}");
            }


        }

        private async void button3_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();

            var client = new ClarifaiClient("f5d04074f20343b787952a2605d34b3a");

            var res2 = await client.PublicModels.FocusModel.Predict(new ClarifaiURLImage(textBox1.Text)).ExecuteAsync();
            foreach (var concept in res2.Get().Data)
            {
                dataGridView1.Rows.Add($"Density", $"{concept.Density}");
                dataGridView1.Rows.Add($"Focus Value", $"{concept.Value}");
            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            dataGridView2.Rows.Clear();
            dataGridView5.Rows.Clear();
            MySqlConnection conn = DBUtils.GetDBConnection();
            MySqlCommand cmd = conn.CreateCommand();

            string sql = "SELECT * FROM screens"; //ссылки на скрины из базы

            cmd.Connection = conn;
            cmd.CommandText = sql;

            MySqlDataReader reader;

            try
            {
                cmd.Connection.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    dataGridView2.Rows.Add(reader["id"], reader["scr_link"]);
                }
                reader.Close();
            }
            catch (MySqlException ex)
            {
                groupBox1.Text = "Error: \r\n{ 0}" + ex.ToString();
            }

            string sql1 = "SELECT * FROM an_base"; //ссылки на скрины из базы

            cmd.Connection = conn;
            cmd.CommandText = sql1;



            try
            {

                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    dataGridView5.Rows.Add(reader["id"], reader["name"], reader["value"], reader["img_link"]);
                }
                reader.Close();
            }
            catch (MySqlException ex)
            {
                groupBox1.Text = "Error: \r\n{ 0}" + ex.ToString();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            MySqlCommand cmd = conn.CreateCommand();

            string sql = "SELECT * FROM screens"; //ссылки на скрины из базы

            cmd.Connection = conn;
            cmd.CommandText = sql;

            MySqlDataReader reader;

            try
            {
                cmd.Connection.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    dataGridView3.Rows.Add(reader["id"], reader["scr_link"]);
                }
                reader.Close();
            }
            catch (MySqlException ex)
            {
                groupBox1.Text = "Error: \r\n{ 0}" + ex.ToString();
            }
        }

        private void dataGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }



        private void button7_Click(object sender, EventArgs e)
        {
            string sql_select = "SELECT * FROM an_base";
            MySqlConnection conn = DBUtils.GetDBConnection();
            MySqlCommand cmd = conn.CreateCommand();

            cmd.Connection = conn;
            cmd.CommandText = sql_select;

            MySqlDataReader reader;

            string name;
            string buf_v;
            decimal val;
            int val1;
            int i = 1;
            int count = 0;
            name = "adult";
            try
            {
                cmd.Connection.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    if (name == reader["name"].ToString())
                    {
                        buf_v = reader["value"].ToString();
                        val = Convert.ToDecimal(buf_v) * 100;
                        val1 = Convert.ToInt32(val);
                        count = count + val1;
                        MessageBox.Show(count.ToString());
                        i++;
                    }
                    else
                    {
                        //ave = count / i;
                        //dataGridView6.Rows.Add(reader["name"], ave, i);
                        name = reader["name"].ToString();
                        i = 1;
                        count = 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error: \r\n{ 0}" + ex.ToString(), "Error", MessageBoxButtons.OK);
            }
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            dataGridView6.Rows.Clear();
            MySqlConnection conn = DBUtils.GetDBConnection();
            MySqlCommand cmd = conn.CreateCommand();

            string sql_count = "SELECT name, value, COUNT(name) as count, SUM(value) as suma, SUM(value) / COUNT(name) as aver FROM an_base GROUP BY name";

            cmd.Connection = conn;
            cmd.CommandText = sql_count;
            MySqlDataReader reader;
            try
            {
                cmd.Connection.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string buf = reader["count"].ToString();
                    int count = Convert.ToInt32(buf);
                    if (count > 5)
                    {
                        dataGridView6.Rows.Add(reader["name"], reader["count"], reader["suma"], reader["aver"]);

                    }
                }
                reader.Close();

            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error: \r\n{ 0}" + ex.ToString(), "Error", MessageBoxButtons.OK);
            }

            button8.Visible = true;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            //dataGridView6.Rows.Clear();
            MySqlConnection conn = DBUtils.GetDBConnection();
            MySqlCommand cmd = conn.CreateCommand();

            try
            {
                cmd.Connection.Open();
                int row = dataGridView6.RowCount;
                for (int n = 0; n < (row - 1); n++)
                {
                    string names = Convert.ToString(dataGridView6[0, n].Value.ToString());
                    decimal aver = Convert.ToDecimal(dataGridView6[3, n].Value.ToString());
                    int summa = Convert.ToInt32(dataGridView6[1, n].Value.ToString());
                    string sql = "INSERT INTO `videos`.`averages` (`name`, `average`, `summa`) VALUES (@name,@aver,@summa);";
                    using (MySqlCommand cmd1 = new MySqlCommand(sql, conn))
                    {
                        cmd1.Parameters.AddWithValue("@name", names);
                        cmd1.Parameters.AddWithValue("@aver", aver);
                        cmd1.Parameters.AddWithValue("@summa", summa);

                        cmd1.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Data writed in DB", "Success", MessageBoxButtons.OK);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error: \r\n{ 0}" + ex.ToString(), "Error", MessageBoxButtons.OK);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private async void button9_Click(object sender, EventArgs e)
        {
            var client = new ClarifaiClient("f5d04074f20343b787952a2605d34b3a");

            MySqlConnection conn = DBUtils.GetDBConnection();
            MySqlCommand cmd = conn.CreateCommand();
            int rown = dataGridView3.RowCount;
            //progressBar2.Maximum = rown;
            cmd.Connection.Open();
            for (int i = 0; i < (rown - 1); i++)
            {
                string linked = dataGridView3[1, i].Value.ToString();
                //MessageBox.Show(linked.ToString());
                var res = await client.PublicModels.GeneralModel.Predict(new ClarifaiURLImage(linked)).ExecuteAsync();

                foreach (var concept in res.Get().Data)
                {
                    dataGridView4.Rows.Add(linked, $"{concept.Name}", $"{concept.Value}");
                   
                            
                    string sql = "INSERT INTO an_base (name,value,img_link) VALUES (@name,@val,@link)";
                    using (MySqlCommand cmd1 = new MySqlCommand(sql, conn))
                    {

                        cmd1.Parameters.AddWithValue("@name", concept.Name);
                        cmd1.Parameters.AddWithValue("@val", concept.Value);
                        cmd1.Parameters.AddWithValue("@link", linked);

                        cmd1.ExecuteNonQuery();
                    }
                        
                    
                }
            }
            //MessageBox.Show("sdjs;fk");


            string sql_count = "SELECT name, value, COUNT(name) as count, SUM(value) as suma, SUM(value) / COUNT(name) as aver FROM an_base GROUP BY name";

            cmd.Connection = conn;
            cmd.CommandText = sql_count;
            MySqlDataReader reader;
            try
            {
                //cmd.Connection.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string buf = reader["count"].ToString();
                    int count = Convert.ToInt32(buf);
                    if (count > 1)
                    {
                        dataGridView6.Rows.Add(reader["name"], reader["count"], reader["suma"], reader["aver"]);

                    }
                }
                reader.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error: \r\n{ 0}" + ex.ToString(), "Error", MessageBoxButtons.OK);
            }
        }

        private void Load_Images_Cmp_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            MySqlCommand cmd = conn.CreateCommand();

            string sql_count = "SELECT * FROM our_screens";

            cmd.Connection = conn;
            cmd.CommandText = sql_count;
            MySqlDataReader reader;
            try
            {
                cmd.Connection.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {                                 
                        Images_Compare_Grid.Rows.Add(reader["link"]);                    
                }
                reader.Close();
                
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error: \r\n{ 0}" + ex.ToString(), "Error", MessageBoxButtons.OK);
            }
            int rowsin = Images_Compare_Grid.RowCount - 1;
            Mini_Log.AppendText(">> Added: " + rowsin + " links \n");
            Screen_Anal_Button.Visible = true;

        }

        private async void Screen_Anal_Button_Click(object sender, EventArgs e)
        {
            int rowsin = Images_Compare_Grid.RowCount - 1;
            progressBar1.Maximum = rowsin;

            var client = new ClarifaiClient("f5d04074f20343b787952a2605d34b3a");

            MySqlConnection conn = DBUtils.GetDBConnection();
            MySqlCommand cmd = conn.CreateCommand();
            cmd.Connection.Open();
            string truncate = "TRUNCATE `an_base_our_screens`;" ;
            cmd.CommandText = truncate;
            for (int n = 0; n < (rowsin); n++)
            {
                string links = Images_Compare_Grid[0, n].Value.ToString();
                var res = await client.PublicModels.GeneralModel.Predict(new ClarifaiURLImage(links)).ExecuteAsync();

                try
                {
                    foreach (var concept in res.Get().Data)
                    {
                        Screen_Analyze_Grid.Rows.Add($"{concept.Name}", $"{concept.Value}", links);
                        string sql = "INSERT INTO an_base_our_screens (name,value,img_link) VALUES (@name,@val,@link)";
                        using (MySqlCommand cmd1 = new MySqlCommand(sql, conn))
                        {

                            cmd1.Parameters.AddWithValue("@name", concept.Name);
                            cmd1.Parameters.AddWithValue("@val", concept.Value);
                            cmd1.Parameters.AddWithValue("@link", links);

                            cmd1.ExecuteNonQuery();
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    Mini_Log.AppendText("Error: \r\n{ 0}" + ex.ToString());
                }
                Mini_Log.AppendText(">> Analyzed: " + n + " images \n");

                progressBar1.Value = n+1;
                
            }
            Mini_Log.AppendText(">> Analyzing completed successfully! \n");
            MessageBox.Show("Analyze Completed Successfully!", "Success!", MessageBoxButtons.OK);

        }

        private void button6_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            MySqlCommand cmd = conn.CreateCommand();

            string sql_our_screens = "SELECT * FROM an_base_our_screens"; // выбираем нашу БД и выводим с нее все
            

            cmd.Connection = conn; //открываем соединение
            cmd.CommandText = sql_our_screens; //вбиваем запрос

            MySqlDataReader reader;

            int rows_c = dataGridView6.RowCount;
            

            //1) Выбираем из базы данных данные и построчно сравниваем с нужными нам значениями которые находятся в таблице - averages
            //2) Лучшие результаты вбиваем в dataGridView7
            //3) Выбираем лучшие результаты из dataGridView7 (предположительно складываем все основные признаки и выбираем лучшее по среднему)
            //4) Выводим в dataGridView8 результат (должно быть только 1-но изображение)
            //

            try
            {
                cmd.Connection.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {                    
                    for (int i = 0; i < (rows_c - 1); i++)
                    {
                        if ((reader["name"].ToString() == dataGridView6[0, i].Value.ToString()) && (Convert.ToDecimal(reader["value"].ToString()) > Convert.ToDecimal(dataGridView6[3, i].Value.ToString()))) //поиск совпадающих имен и сравнение value
                        {
                            dataGridView7.Rows.Add(reader["name"], reader["value"], reader["img_link"]);
                            Mini_Log.AppendText(reader["img_link"] + "\n");
                           
                            
                        }                        
                    }
                }
                reader.Close();
                
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error: \r\n{ 0}" + ex.ToString(), "Error", MessageBoxButtons.OK);
            }
            

        }

        private void button10_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            MySqlCommand cmd = conn.CreateCommand();
            int rows_d = dataGridView7.RowCount;
            cmd.Connection.Open();
            MessageBox.Show(rows_d.ToString(), "Error", MessageBoxButtons.OK);
            string sql2 = "INSERT INTO `videos`.`compare_buf` (`name`, `value`, `link`) VALUES (@name, @value, @link);";
            for (int i = 0; i < (rows_d - 1); i++)
            {
                using (MySqlCommand cmd1 = new MySqlCommand(sql2, conn))
                {

                    cmd1.Parameters.AddWithValue("@name", dataGridView7[0, i].Value.ToString());
                    cmd1.Parameters.AddWithValue("@value", Convert.ToDouble(dataGridView7[1, i].Value.ToString()));
                    cmd1.Parameters.AddWithValue("@link", dataGridView7[2, i].Value.ToString());

                    cmd1.ExecuteNonQuery();
                }
            }
            string sql3 = "SELECT link, value,  COUNT(link) as comparsions, SUM(value)/COUNT(link) as aver FROM compare_buf GROUP BY link;";
            cmd.CommandText = sql3;
            MySqlDataReader reader;
            try
            {
                
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (Convert.ToInt32(reader["comparsions"].ToString()) > 5)
                    {
                        dataGridView8.Rows.Add(reader["aver"], reader["comparsions"], reader["link"]);                       
                    }
                }
                reader.Close();
                dataGridView8.Sort(dataGridView8.Columns[0], ListSortDirection.Descending);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error: \r\n{ 0}" + ex.ToString(), "Error", MessageBoxButtons.OK);
                
            }
            Mini_Log.AppendText(">> BEST SCREEN: " + dataGridView8[2, 0].Value.ToString());
        }

        private void button11_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            MySqlCommand cmd = conn.CreateCommand();

            string drop1 = "TRUNCATE `an_base_our_screens`; TRUNCATE `compare_buf`; TRUNCATE `an_base`; TRUNCATE `averages`; ";


            cmd.Connection.Open();

            cmd.CommandText = drop1;
           
            MySqlDataReader reader;
            reader = cmd.ExecuteReader();
            reader.Read();

            MessageBox.Show("Tables Dropped!", "Success", MessageBoxButtons.OK);
        }
    }
}
