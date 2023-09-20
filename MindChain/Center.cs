using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace MindChain
{
    public partial class Center : Form
    {
        

        public static String DataBase_ip { set; get; }
        public static String DataBase_Name { set; get; }
        public static String DataBase_id { set; get; }
        public static String DataBase_Password { set; get; }

        public static String User_id ; //玩家編號
        public static String username;   //玩家名稱
        public static String User_ip;   //玩家 IP
        public static String private_key;   //玩家私鑰
        //public static String Account_id;
        private String Room_ip;
        private String Room_id;
        private String Room_Head_id;

        SqlConnection cnn = new SqlConnection("Data Source=" + Center.DataBase_ip +
                ";Initial Catalog=" + Center.DataBase_Name +
                ";User ID=" + Center.DataBase_id +
                ";Password=" + Center.DataBase_Password + "");//資料庫參數

        public Center()
        {
            InitializeComponent();
        }

        private void Center_Load(object sender, EventArgs e)
        {
            username_txt.Text = username;

            // 取得 user IP 位址
            string strHostName = Dns.GetHostName();
            IPHostEntry iphostentry = Dns.GetHostEntry(strHostName);

            foreach (IPAddress ipaddress in iphostentry.AddressList)
            {
                if (ipaddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    user_IP_txt.Text = ipaddress.ToString();         
                }
            }

            //SQL 查詢取得 member ip
            SqlCommand sql_com = new SqlCommand("Select * from dbo.member_table where member_username = '" + Center.username + "'");
            cnn.Open();
            sql_com.Connection = cnn;
            SqlDataReader s_read = sql_com.ExecuteReader();
            while (s_read.Read())
            {
                Center.User_id = s_read["member_id_add"].ToString();
            }
            cnn.Close();

            //抓取 玩家的 Coin
            SqlCommand cmd = new SqlCommand("Select * from dbo.member_coin where member_id_add='" + User_id + "'", cnn);
            cnn.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                user_coin_txt.Text = dr.GetValue(2).ToString();
            }
            cnn.Close();

            //listbox 取得遊戲房間
            get_gaame_room();

        }

        private void Button1_Click(object sender, EventArgs e)//建立遊戲室
        {
            CreateRoom Room = new CreateRoom();
            Room.Show();
        }

        private void Button2_Click(object sender, EventArgs e)//加入
        {

            //取得點選的遊戲房間
            string game_room_name = game_room_item[int.Parse(listBox1.SelectedIndex.ToString())];

            //SQL 查詢取得 房間ID IP
            SqlCommand sql_com = new SqlCommand("Select * from dbo.game_room_table where room_name='" + game_room_name + "'where game_result ='未結束'");
            cnn.Open();
            sql_com.Connection = cnn;
            SqlDataReader s_read = sql_com.ExecuteReader();
            while (s_read.Read())
            {
                Room_ip = s_read["room_ip"].ToString();
                Room_id = s_read["game_room_id_add"].ToString();
                Room_Head_id = s_read["member_id"].ToString();
            }
            cnn.Close();
            
            GameRoom game = new GameRoom();
            game.Room_ip = Room_ip; //遊戲房間建立人的ip
            game.Room_id = Room_id;
            game.User_ip = User_ip;
            game.User_id = User_id;
            game.Room_Head_id = Room_Head_id;
            game.Show();

        }

        //重新整理遊戲室
        private void Reload_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            get_gaame_room();
            
        }

        //listbox 取得遊戲房間
        string[] game_room_item = new string[20]; //房間數量陣列
        public void get_gaame_room()
        {
            
            int c = 0; //計數 

            cnn.Open();
            SqlCommand sql_com = new SqlCommand();
            sql_com.CommandText = "select * from dbo.game_room_table where game_result='未結束'";
            sql_com.Connection = cnn;

            SqlDataReader dr = sql_com.ExecuteReader();
            while (dr.Read())
            {
                if (!dr[0].Equals(DBNull.Value))
                {
                    game_room_item[c] = dr.GetValue(3).ToString();
                    c += 1;
                }
            }
            cnn.Close();

            for (int i = 0; i < game_room_item.Length; i++)
            {
                if (game_room_item[i] != null)
                {
                    listBox1.Items.Add(game_room_item[i]);
                }
            }
        }

        //登出
        private void button3_Click_1(object sender, EventArgs e)
        {
            Sign_in sign_in = new Sign_in();
            sign_in.Show();
            this.Hide();
        }

        //加密
        
        public static string encryption(string User_id, string content)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            string public_key = "";
            SqlConnection cnn1 = new SqlConnection("Data Source=" + Center.DataBase_ip +
                ";Initial Catalog=" + Center.DataBase_Name +
                ";User ID=" + Center.DataBase_id +
                ";Password=" + Center.DataBase_Password + "");//資料庫參數
            cnn1.Open();
            //取得 玩家的公鑰
            SqlCommand cmd = new SqlCommand("Select * from dbo.member_publickey_table where member_id_add='" + User_id + "'", cnn1);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                public_key = dr.GetValue(2).ToString();
            }
            cnn1.Close();

            rsa.FromXmlString(public_key);
            return Convert.ToBase64String(rsa.Encrypt(Encoding.UTF8.GetBytes(content), false));
        }

        //解密
        public static string Decrypt(string private_key, string encryptString)
        {
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(private_key);
                string decryptString = Encoding.UTF8.GetString(rsa.Decrypt(Convert.FromBase64String(encryptString), false));
                return decryptString;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("解密錯誤");
                return "False";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            user_IP_txt.Text = User_ip;
        }
    }
}
