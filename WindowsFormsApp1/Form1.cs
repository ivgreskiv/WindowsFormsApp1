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
using System.IO;


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
            ToolTip t = new ToolTip();
            t.SetToolTip(button6, "Press to compare screens and than press start to load Best Screen");

            ToolTip t1 = new ToolTip();
            t1.SetToolTip(Load_Images_Cmp, "Press this button to start Analyzing screens");

            ToolTip t2 = new ToolTip();
            t1.SetToolTip(Screen_Anal_Button, "Press to begin analyzing of Tube Screens");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string link = textBox1.Text;

            pictureBox1.ImageLocation = link;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

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
                    if (count > Convert.ToInt32(textBox3.Text.ToString()))
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
            progressBar2.Maximum = 1365;
            int coun1t = 0;
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
                
                if (coun1t <= progressBar2.Maximum)
                {
                    progressBar2.Value = coun1t++;
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
                    Images_Compare_Grid.Rows.Add(reader["link"], reader["vid_id"]);
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
            string truncate = "TRUNCATE `an_base_our_screens`;";
            cmd.CommandText = truncate;
            for (int n = 0; n < (rowsin); n++)
            {
                string links = Images_Compare_Grid[0, n].Value.ToString();
                var res = await client.PublicModels.GeneralModel.Predict(new ClarifaiURLImage(links)).ExecuteAsync();
                try
                {
                    var res2 = await client.PublicModels.FocusModel.Predict(new ClarifaiURLImage(links)).ExecuteAsync();
                    int vid_id = Convert.ToInt32(Images_Compare_Grid[1, n].Value.ToString());

                    try
                    {
                        foreach (var concept in res.Get().Data)
                        {
                            Screen_Analyze_Grid.Rows.Add($"{concept.Name}", $"{concept.Value}", links, vid_id);
                            string sql = "INSERT INTO an_base_our_screens (name,value,img_link,vid_id) VALUES (@name,@val,@link,@img_id)";
                            using (MySqlCommand cmd1 = new MySqlCommand(sql, conn))
                            {
                                cmd1.Parameters.AddWithValue("@name", concept.Name);
                                cmd1.Parameters.AddWithValue("@val", concept.Value);
                                cmd1.Parameters.AddWithValue("@link", links);
                                cmd1.Parameters.AddWithValue("@img_id", vid_id);

                                cmd1.ExecuteNonQuery();
                            }
                        }                        
                    }
                    catch (MySqlException ex)
                    {
                        Mini_Log.AppendText("Error: \r\n{ 0}" + ex.ToString());
                    }
                    Mini_Log.AppendText(">> Analyzed: " + n + " images \n");

                    progressBar1.Value = n + 1;
                }
                catch (Exception exps)
                {

                }
            }
            Mini_Log.AppendText(">> Analyzing completed successfully! \n");
            MessageBox.Show("Analyze Completed Successfully!", "Success!", MessageBoxButtons.OK);

        }

        private void button6_Click(object sender, EventArgs e)
        {
            dataGridView6.Rows.Clear();
            MySqlConnection conn = DBUtils.GetDBConnection();
            MySqlCommand cmd = conn.CreateCommand();

            //Подсчет из обучалки количества совпадений во всей выборочной базе и вывод среднего значения, выполняется запросом ниже
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
                    if (count > Convert.ToInt32(textBox3.Text.ToString()))
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

            string sql_our_screens = "SELECT * FROM an_base_our_screens"; // выбираем нашу БД и выводим с нее все


            cmd.Connection = conn; //открываем соединение
            cmd.CommandText = sql_our_screens; //вбиваем запрос



            int rows_c = dataGridView6.RowCount;


            //1) Выбираем из базы данных данные и построчно сравниваем с нужными нам значениями которые находятся в таблице - averages
            //2) Лучшие результаты вбиваем в dataGridView7
            //3) Выбираем лучшие результаты из dataGridView7 (предположительно складываем все основные признаки и выбираем лучшее по среднему)
            //4) Выводим в dataGridView8 результат (должно быть только 1-но изображение)

            try
            {

                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    for (int i = 0; i < (rows_c - 1); i++)
                    {
                        if ((reader["name"].ToString() == dataGridView6[0, i].Value.ToString()) && (Convert.ToDecimal(reader["value"].ToString()) > Convert.ToDecimal(dataGridView6[3, i].Value.ToString()))) //поиск совпадающих имен и сравнение value
                        {
                            dataGridView7.Rows.Add(reader["name"], reader["value"], reader["img_link"], reader["vid_id"]);
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
            double buf = 0;
            cmd.Connection.Open();            
            string sql2 = "INSERT INTO `videos`.`compare_buf` (`name`, `value`, `link`, `vid_id`) VALUES (@name, @value, @link, @vid_id );";
            for (int i = 0; i < (rows_d - 1); i++)
            {
                using (MySqlCommand cmd1 = new MySqlCommand(sql2, conn))
                {

                    cmd1.Parameters.AddWithValue("@name", dataGridView7[0, i].Value.ToString());
                    cmd1.Parameters.AddWithValue("@value", Convert.ToDouble(dataGridView7[1, i].Value.ToString()));
                    cmd1.Parameters.AddWithValue("@link", dataGridView7[2, i].Value.ToString());
                    cmd1.Parameters.AddWithValue("@vid_id", dataGridView7[3, i].Value.ToString());
                   
                    cmd1.ExecuteNonQuery();
                }
            }


            //Подсчет совпадений для каждого скриншота [START]
            string sql3 = "SELECT link, value, vid_id,  COUNT(link) as comparsions, SUM(value)/COUNT(link) as aver FROM compare_buf GROUP BY link;";
            cmd.CommandText = sql3;
            MySqlDataReader reader;

            int rows_ds = dataGridView8.RowCount;
            
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if ((Convert.ToInt32(reader["comparsions"].ToString()) > Convert.ToInt32(textBox2.Text)))
                    
                        dataGridView8.Rows.Add(reader["aver"], reader["comparsions"], reader["link"], reader["vid_id"]);
                    
                    
                }
                reader.Close();
                dataGridView8.Sort(dataGridView8.Columns[0], ListSortDirection.Descending);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error: \r\n{ 0}" + ex.ToString(), "Error", MessageBoxButtons.OK);
            }
            //Подсчет совпадений для каждого скриншота [END]

            int rows_df = dataGridView8.Rows.Count;
            string sql33 = "INSERT INTO `videos`.`result` (`ave`, `compare`, `link`, `vid_id`) VALUES (@ave, @compare, @link, @vid_id);";
            for (int i = 0; i < rows_df; i++)
            {
                using (MySqlCommand cmd1 = new MySqlCommand(sql33, conn))
                {
                    cmd1.Parameters.AddWithValue("@ave", Convert.ToDouble(dataGridView8[0, i].Value.ToString()));
                    cmd1.Parameters.AddWithValue("@compare", Convert.ToInt32(dataGridView8[1, i].Value.ToString()));
                    cmd1.Parameters.AddWithValue("@link", dataGridView8[2, i].Value.ToString());
                    cmd1.Parameters.AddWithValue("@vid_id", Convert.ToInt32(dataGridView8[3, i].Value.ToString()));

                    cmd1.ExecuteNonQuery();
                }
            }


        }


        //Две функции ниже - очистка таблиц
        private void button11_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            MySqlCommand cmd = conn.CreateCommand();

            string drop1 = "TRUNCATE `compare_buf`; ";


            cmd.Connection.Open();

            cmd.CommandText = drop1;
           
            MySqlDataReader reader;
            reader = cmd.ExecuteReader();
            reader.Read();

            MessageBox.Show("Tables Dropped!", "Success", MessageBoxButtons.OK);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            MySqlCommand cmd = conn.CreateCommand();

            string drop1 = "TRUNCATE `an_base_our_screens`;";

            cmd.Connection.Open();

            cmd.CommandText = drop1;

            MySqlDataReader reader;
            reader = cmd.ExecuteReader();
            reader.Read();

            MessageBox.Show("Tables Dropped!", "Success", MessageBoxButtons.OK);
        }


        //Выбор лучшего скрина
        //SELECT vid_id, max(ave), link from result GROUP BY vid_id 
        //по этому запросу выбирается лучшее изображение по максимальному среднему
        private void button13_Click(object sender, EventArgs e)
        {
            int rzm = Images_Compare_Grid.Rows.Count;
            MySqlConnection conn = DBUtils.GetDBConnection();
            MySqlCommand cmd = conn.CreateCommand();
            cmd.Connection.Open();

            string sql3 = "SELECT vid_id, max(ave), link from result GROUP BY vid_id";
            cmd.CommandText = sql3;
            MySqlDataReader reader;
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    
                    Mini_Log.AppendText("Best screen link for video: \nID: " + reader["vid_id"] + "\nLink for screen:" + reader["link"] + "\n");
                    dataGridView1.Rows.Add(reader["vid_id"], GetImageFromUrl(reader["link"].ToString()));
                 
                }
                reader.Close();                
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error: \r\n{ 0}" + ex.ToString(), "Error", MessageBoxButtons.OK);
            }                       
        }

        private void button14_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            MySqlCommand cmd = conn.CreateCommand();

            string drop1 = "TRUNCATE `result`;";


            cmd.Connection.Open();

            cmd.CommandText = drop1;

            MySqlDataReader reader;
            reader = cmd.ExecuteReader();
            reader.Read();

            MessageBox.Show("Tables Dropped!", "Success", MessageBoxButtons.OK);
        }


        //Очистка всех dataGridView
        private void button15_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            dataGridView3.Rows.Clear();
            dataGridView4.Rows.Clear();
            dataGridView5.Rows.Clear();
            dataGridView6.Rows.Clear();
            dataGridView7.Rows.Clear();
            dataGridView8.Rows.Clear();
            Images_Compare_Grid.Rows.Clear();
            Screen_Analyze_Grid.Rows.Clear();
        }

        private void groupBox10_Enter(object sender, EventArgs e)
        {

        }

        private void button15_Click_1(object sender, EventArgs e)
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            MySqlCommand cmd = conn.CreateCommand();

            string drop1 = "TRUNCATE `averages`;";

            cmd.Connection.Open();

            cmd.CommandText = drop1;

            MySqlDataReader reader;
            reader = cmd.ExecuteReader();
            reader.Read();

            MessageBox.Show("Tables Dropped!", "Success", MessageBoxButtons.OK);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 newForm = new Form2();
            newForm.Show();
        }

        public static Image GetImageFromUrl(string url)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            //if you have proxy server, you may need to set proxy details like below 
            //httpWebRequest.Proxy = new WebProxy("proxyserver",port){ Credentials = new NetworkCredential(){ UserName ="uname", Password = "pw"}};

            using (HttpWebResponse httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (Stream stream = httpWebReponse.GetResponseStream())
                {
                    return Image.FromStream(stream);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = DBUtils.GetDBConnection();            
            MySqlCommand cmd1 = conn.CreateCommand();
            string sql = "SELECT vid_id, link FROM our_screens GROUP BY vid_id";
            cmd1.CommandText = sql;
            MySqlDataReader reader;            
            int rzm2 = dataGridView1.ColumnCount;
            cmd1.Connection.Open();

            try
            {
                reader = cmd1.ExecuteReader();                
                int im = 101;
                while (reader.Read())
                {
                    if (im == Convert.ToInt32(reader["vid_id"].ToString()))
                    {
                        for (int i = 0; i < dataGridView1.RowCount; i++)
                        {
                            dataGridView1.Rows.Add();
                            dataGridView1[0, i].Value = im;
                            
                            for (int j = 2; j < dataGridView1.ColumnCount; j++)
                            {
                                dataGridView1.Rows[i].Cells[j].Value = reader["link"];                                
                            }
                        }
                    }
                    else im = Convert.ToInt32(reader["vid_id"].ToString());
                }
                reader.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error: \r\n{ 0}" + ex.ToString(), "Error", MessageBoxButtons.OK);
            }
        }

        private void button3_Click_1(object sender, EventArgs e) //вывод всех скринов с таймлайна
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            MySqlCommand cmd = conn.CreateCommand();

            string sql = "SELECT link, vid_id FROM our_screens WHERE vid_id = '"+ comboBox1.Text +"'";

            dataGridView9.Rows.Clear();

            cmd.CommandText = sql;
            MySqlDataReader reader;

            cmd.Connection.Open();
            reader = cmd.ExecuteReader();

            string[] linkse = new string[12];
            int i = 0;

            while ((reader.Read() && (i<12)))
            {
                linkse[i] = reader["link"].ToString();
                i++;
            }

            int k = 0;

            for (i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    dataGridView9.Rows.Add();
                    dataGridView9[i, j].Value = GetImageFromUrl(linkse[k]);
                    if (k < 12)
                    {
                        k++;
                    }
                    else k = 0;
                }
            }

        }

        private async void button16_Click(object sender, EventArgs e)
        {
            var client = new ClarifaiClient("f5d04074f20343b787952a2605d34b3a");
            int rowsin = Images_Compare_Grid.RowCount - 1;
            MySqlConnection conn = DBUtils.GetDBConnection();
            MySqlCommand cmd = conn.CreateCommand();
            cmd.Connection.Open();
            string truncate = "TRUNCATE `an_base_our_screens`;";
            cmd.CommandText = truncate;
            for (int n = 0; n < (rowsin); n++)
            {
                string links = Images_Compare_Grid[0, n].Value.ToString();

                int vid_id = Convert.ToInt32(Images_Compare_Grid[1, n].Value.ToString());
                //MessageBox.Show(links);
                if (Images_Compare_Grid[0, n].Value.ToString() != null)
                {
                    try
                    {
                        var resor = await client.PublicModels.FocusModel.Predict(new ClarifaiURLImage(Images_Compare_Grid[0, n].Value.ToString())).ExecuteAsync();
                        //MessageBox.Show(resor.Get().Data.ToString());
                        if (resor != null)
                        {
                            foreach (var concept in resor.Get().Data)
                            {
                                if (Convert.ToDouble(concept.Value) != 0)
                                {
                                    Screen_Analyze_Grid.Rows.Add($"Focus Value", $"{concept.Value}", links, vid_id);
                                    string sql = "INSERT INTO an_base_our_screens (name,value,img_link,vid_id) VALUES (@name,@val,@link,@img_id)";
                                    using (MySqlCommand cmd1 = new MySqlCommand(sql, conn))
                                    {
                                        cmd1.Parameters.AddWithValue("@name", "Focus Value");
                                        cmd1.Parameters.AddWithValue("@val", concept.Value);
                                        cmd1.Parameters.AddWithValue("@link", links);
                                        cmd1.Parameters.AddWithValue("@img_id", vid_id);

                                        cmd1.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ext)
                    {

                    }
                }
            }
        }
    }
}
