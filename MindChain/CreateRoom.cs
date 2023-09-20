using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Net;
using System.Net.Sockets;
using System.Data.SqlClient;

namespace MindChain
{
    public partial class CreateRoom : Form
    {
        UdpClient udp;

        SqlConnection cnn = new SqlConnection("Data Source=" + Center.DataBase_ip +
            ";Initial Catalog="+ Center.DataBase_Name +
            ";User ID=" + Center.DataBase_id +
            ";Password=" + Center.DataBase_Password + "");//資料庫參數

        String[] Player_id = new String[7];//使用者帳號id
        String[] Player_Address = new String[7];//使用者的ip位置
        int Player_sort;//第幾個使用者進來 (房間人數用)

        public CreateRoom()
        {
            InitializeComponent();
        }

        private int GiveCard(String Now_Position,String User_id,String room_id)//發牌
        {
            int Point = 0;
            //String room_id = "";
            bool[] PictureCk = new bool[53];
            Random random_card = new Random(); //

            //搜尋資料庫那些牌組還沒被選走
            SqlCommand cmd = new SqlCommand("select card_point from game_info_table where room_id='" + room_id + "'", cnn);
            cnn.Open();
            //String val = (String)cmd.ExecuteScalar();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())//讀取伺服器 看哪些牌已經被選走
            {
                PictureCk[Convert.ToInt32(dr.GetValue(0))] = true;//當PictureCk = 0 代表這張牌沒出現過 = 1 出現過
            }
            do 
            {
                Point = random_card.Next(1, 53);//隨機發牌 取得1~52編號
            } while (PictureCk[Point] != false);
            cnn.Close();
           
            //新增此牌組到 該局資料庫
            cmd = new SqlCommand(String.Format("INSERT INTO game_info_table([room_id],[member_id],[card_obtain],[card_point]) VALUES ('{0}','{1}','{2}','{3}')", 
                room_id, User_id, "True", Point), cnn);
            cnn.Open();
            cmd.ExecuteNonQuery();//執行sql
            cnn.Close();

            return Point;
        }

        private void add_bit(String bit , String User_id)//寫入賭金資訊
        {
            SqlCommand cmd = new SqlCommand("UPDATE game_room_member " +
                "SET bit_coin = " + bit + " WHERE member_id = '" + User_id + "'", cnn);
            cnn.Open();
            cmd.ExecuteScalar();
            cnn.Close();
        }

        private void Update_bit(String room_id, String Giver ,String Recipient)//更新賭金資訊
        {
            /*"SELECT bit_coin FROM game_room_member WHERE room_id='" + room_id + "' and member_id='" + User_id + "'"*/;//使用者賭的錢
            SqlCommand cmd = new SqlCommand("UPDATE member_coin " +
                "SET member_coin=member_coin+" +
                "(SELECT bit_coin FROM game_room_member WHERE game_room_id='" + room_id + "' and member_id='" + Recipient + "')" +
                " WHERE member_id_add='" + Recipient + "'", cnn);
            cnn.Open();
            cmd.ExecuteScalar();
            cnn.Close();

            cmd = new SqlCommand("UPDATE member_coin " +
               "SET member_coin=member_coin-" +
               "(SELECT bit_coin FROM game_room_member WHERE game_room_id='" + room_id + "' and member_id='" + Giver + "')" +
               " WHERE member_id_add='" + Giver + "'", cnn);
            cnn.Open();
            cmd.ExecuteScalar();
            cnn.Close();
        }

        private String open_card(String room_id,String dealer_id)//開牌
        {
            //計算莊家點數
            int dealer_point = 0,Player_point = 0 ;
            List<int> temp_point = new List<int> { };
            String result = "";

            SqlCommand cmd = new SqlCommand("select card_point from game_info_table where room_id='" + room_id + "' and member_id='" + dealer_id + "'", cnn); ;
            cnn.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())//讀取莊家的點數
            {
                temp_point.Add(Convert.ToInt32(dr.GetValue(0)) % 13);
            }
            dealer_point = compile_CardPoint(temp_point); //計算莊家的點數
            cnn.Close();

            //計算各個玩家的點數
            for (int i = 0; i < Player_id.Length; i++)
            {
                if(Player_id[i] != null && Player_id[i] != dealer_id)
                {
                    temp_point = new List<int> { };//初始化 temp_point
                    cmd = new SqlCommand("select card_point from game_info_table where room_id='" + room_id + "' and member_id='" + Player_id[i] + "'", cnn);
                    cnn.Open();
                    dr = cmd.ExecuteReader();
                    while (dr.Read())//讀取莊家的點數
                    {
                        temp_point.Add(Convert.ToInt32(dr.GetValue(0))%13);
                    }
                    Player_point = compile_CardPoint(temp_point); //計算莊家的點數
                    cnn.Close();

                    if (i != 0)
                    {
                        if (Player_point < dealer_point || Player_point > 21)//莊家贏
                        {
                            result += ",Lose";
                            Update_bit(room_id,  Player_id[i] , dealer_id);
                        }
                        else if (Player_point == dealer_point)//平手
                        {
                            result += ",平手";
                        }
                        else if (Player_point > dealer_point)//莊家輸
                        {
                            result += ",Win";
                            Update_bit(room_id, dealer_id, Player_id[i]);
                        }
                    }
                }
            }
            return result;
        }

        private int compile_CardPoint(List<int> temp_point)//計算牌的點數
        {
            int total_point = 0, A_Point_Count = 0;
            for (int i = 0; i < temp_point.Count; i++)
            {
                if (temp_point[i] == 11 || temp_point[i] == 12 || temp_point[i] == 0) temp_point[i] = 10; //如果是JQK =10點
                if (temp_point[i] == 1)//紀錄有幾張A
                {
                    A_Point_Count++;
                }
                else
                {//沒特殊情況 就單純紀錄牌的點數
                    total_point += temp_point[i];
                }
            }
            for (int i = 0; i < A_Point_Count; i++)//A牌 最後處理
            {//如果A當10點算的話 會不會爆牌
                if ((total_point + 10) > 21)
                {
                    total_point += 1;
                }
                else
                {
                    total_point += 10;
                }
            }
            return total_point;
            //MessageBox.Show(Convert.ToString(total_point));
        }

        private void Update_Result(String room_id)//更新賭金資訊
        {
            /*"SELECT bit_coin FROM game_room_member WHERE room_id='" + room_id + "' and member_id='" + User_id + "'"*/
            ;//使用者賭的錢
            SqlCommand cmd = new SqlCommand("UPDATE game_room_table " +
                "SET game_result='已結束'" +
                " WHERE game_room_id_add='" + room_id + "' and game_result='未結束'", cnn);
            cnn.Open();
            cmd.ExecuteScalar();
            cnn.Close();
        }

        private void Button1_Click(object sender, EventArgs e)//建立遊戲室
        {
            //Player_id = new String[Convert.ToInt32(textBox3.Text)];//定義 使用者的ip位置與人數陣列
            //Player_Address = new String[Convert.ToInt32(textBox3.Text)];//定義 使用者的ip位置與人數陣列
            timer1.Enabled = true;

            if (room_name_txt.Text != "" && room_ip_txt.Text != "")
            {
                try
                {
                    //新增房間
                    cnn.Open();
                    SqlCommand sql_com = new SqlCommand("INSERT INTO dbo.game_room_table(member_id,room_name,room_ip,game_result)" +
                    "VALUES('" + Center.User_id + "'," + "'" + room_name_txt.Text + "','" + Center.User_ip + "'" + ",'未結束'); ");
                    sql_com.Connection = cnn;
                    sql_com.ExecuteNonQuery();
                    cnn.Close();
                    button1.Enabled = false;
                }
                catch (Exception ex)
                {
                    button1.Enabled = false;
                    MessageBox.Show("房間已存在");
                }
            }
            else
            {
                MessageBox.Show("建立房間未正確填寫");
            }
        }

        //-------------------------Socket----------------------------------------------
        private void Timer1_Tick(object sender, EventArgs e)//接收端
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);//
            if (udp.Available > 0)
            {
                String msg = Encoding.Unicode.GetString(udp.Receive(ref ep));//取得收到的字串
                //MessageBox.Show(msg);
                //String[] ans = msg.Split(',');
                String[] ans = Center.Decrypt(Center.private_key, msg).Split(',');
                if (ans[0] != "Fasle")
                {
                    if (ans[0] == "Connect")
                    { //連線要求
                        connect_socket(ans);
                        send_msg("Update,");
                    }
                    else if (ans[0] == "GiveCard")//要求補牌 或 發牌
                    {
                        send_msg("Update_Card," + ans[1] + "," + Convert.ToString(GiveCard(ans[1], ans[2], ans[3])));
                    }
                    else if (ans[0] == "Dealer_Open")//莊家顯示第二章牌
                    {
                        send_msg("Dealer_Open,");
                        send_msg("Result" + open_card(ans[1], ans[2])); //開牌
                        Update_Result(ans[1]);//更新遊戲結束狀態至資料庫
                    }
                    else if (ans[0] == "Dealer_Close")//莊家顯示第二章牌
                    {
                        send_msg("Dealer_Close,");
                    }
                    else if (ans[0] == "Bit")//傳送賭金資訊 前往資料庫
                    {
                        add_bit(ans[1], ans[2]);//
                        send_msg("Update_Bit," + ans[3] + "," + ans[1]);// 3 = 更新位置 1 = 金額
                    }
                }
                //textBox1.Text += ep.Address.ToString() + "：" + DateTime.Now.ToString() + " " + Encoding.Unicode.GetString(msg) + Environment.NewLine;
            }
        }

        private void connect_socket(String[] data)// 0 type 1 id 2 ip
        {
            Player_id[Player_sort] = data[1];
            Player_Address[Player_sort] = data[2];
            Player_sort++;//累加人數
        }

        private void send_msg(String msg_str)//傳送訊息 "Update,"更新房間人數
        {
            for (int i = 0; i < Player_sort; i++)
            {
                UdpClient udp1 = new UdpClient();
                IPEndPoint ep_send = new IPEndPoint(IPAddress.Parse(Player_Address[i]), 4000);
                byte[] msg = Encoding.Unicode.GetBytes(Center.encryption(Player_id[i], msg_str)); // 
                udp1.Send(msg, msg.Length, ep_send);
            }
        }

        private void button2_Click(object sender, EventArgs e)//取消
        {
            udp.Close();
            this.Close();
            //GiveCard("0");
            //send_msg("hello");
        }

        private void CreateRoom_Load(object sender, EventArgs e)
        {
            if(udp == null)
            {
                udp = new UdpClient(3000);//伺服器 房主 設定為3000
            }

            room_ip_txt.Text = Center.User_ip;
        }
        //end
    }
}
