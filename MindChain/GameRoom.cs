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
    public partial class GameRoom : Form
    {
        SqlConnection cnn = new SqlConnection("Data Source=" + Center.DataBase_ip +
            ";Initial Catalog=" + Center.DataBase_Name +
            ";User ID=" + Center.DataBase_id +
            ";Password=" + Center.DataBase_Password + "");//資料庫參數
        UdpClient udp ;//伺服器 房主 設定為3000
        UdpClient udp1 = new UdpClient();
        IPEndPoint ep_send;
        public String Room_ip { get; set; }
        public String Room_id { get; set; }
        public String Room_Head_id { get; set; }
        public String User_id { get; set; }
        public String User_ip { get; set; }

        private GroupBox[] Player_GroupBox = new GroupBox[7]; //個玩家插槽
        private PictureBox[,] Card = new PictureBox[7,5] ;//牌組位置
        private Image[] Picture_Card = new Image[53]; //存放牌組照片位子
        private Button[] supply_btn = new Button[7];//補牌按鈕
        private Button[] Ok_btn = new Button[7];//確定按鈕
        private TextBox[] bit_box = new TextBox[7];//輸入賭金框格
        private int[,] CardNumber = new int[7, 5];// 每一個插槽的點數

        private String Now_Position;
        public GameRoom()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)//程式啟動時執行
        {

            if(udp == null)
            {
                udp = new UdpClient(4000);
            }

            //讀取每一個GroupBox控制項
            for (int i = 0; i < 7; i++) Player_GroupBox[i] = Find_GroupBox_Control("GroupBox" + (i+1));//定義GroupBox
            //讀取補牌的Button控制項
            for (int i = 0; i < 7; i++) {
                supply_btn[i] = Find_Button_Control("Button" + (i + 1));//定義 補牌按鈕
                supply_btn[i].Enabled = false;
            }
            //讀取確定賭金的Button控制項
            for (int i = 0; i < 7; i++)
            {
                Ok_btn[i] = Find_Button_Control("button" + (i + 9));//定義 補牌按鈕
                Ok_btn[i].Enabled = false;
            }
            //讀取賭金的輸入框 控制項
            for (int i = 0; i < 7; i++)
            {
                bit_box[i] = Find_Textbox_Control("textBox" + (i + 1));//定義 補牌按鈕
                bit_box[i].Enabled = false;
            }
            //讀取所有圖片檔案
            for (int i = 0; i <= 52; i++) Picture_Card[i] = Image.FromFile(i + ".gif");
            //讀取每一個PictureBox控制項
            for (int i = 0; i < 7; i++) {//拆解每個GroupBox
                Player_GroupBox[i].Visible = false;//temp
                for (int j = 0; j < 5; j++)//拆解每個GroupBox的PictureBox
                {
                    Card[i, j] = Find_PictureBox_Control("PictureBox" + (int)((i) * 5 + (j+1)));
                    //Card[i, j].Image = Picture_Card[0];//蓋牌
                }
            }
            //Re_Game();//遊戲初始化

            ep_send = new IPEndPoint(IPAddress.Parse(Room_ip), 3000);//設定遊戲房間ip與通訊 房間ip=遊戲房間建立人的ip

            attend_room();//加入遊戲房間
            timer1.Enabled = true;//開啟監聽
            send_connect_msg();//連線socket

        }

        private void attend_room()//加入遊戲 (新增此人物到 該局資料庫)
        {
            String[] member = query_room_member();
            String room_identity = "Player";
            if (member.Length == 0) room_identity = "Dealer";
            // cmd = new SqlCommand("INSERT INTO game_info_table([room_id],[member_id],[card_obtain],[card_point]) VALUES ('" + room_id + "','A00001','True','" + Point + "')", cnn);
            SqlCommand cmd = new SqlCommand("INSERT INTO game_room_member([game_room_id],[member_id],[room_identity]) VALUES('" + Room_id + "','" + User_id +"','" + room_identity + "')", cnn);
            cnn.Open();
            cmd.ExecuteReader();
            cnn.Close();
            Now_Position = Convert.ToString(member.Length);//紀錄自己在遊戲室的位子
            update_room();
        }

        private String[] query_room_member()//查詢房內人數
        {
            List<String> temp_member_id = new List<string> { };
            SqlCommand cmd = new SqlCommand("select member_id from game_room_member where game_room_id='" + Room_id + "'", cnn);
            cnn.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())//讀取data 
            {
                temp_member_id.Add(Convert.ToString(dr.GetValue(0)));
            }
            cnn.Close();

            String[] temp_member = new string[temp_member_id.Count];
            for(int i = 0; i < temp_member.Length; i++)
            {
                if (temp_member_id[i] == User_id) {//打開當前帳號的 補牌或開牌按鈕 or 賭金輸入匡宇確定按鈕
                    //supply_btn[i].Enabled = true;
                    Ok_btn[i].Enabled = true;
                    bit_box[i].Enabled = true;
                } 
                cmd = new SqlCommand("select member_username from member_table where member_id_add='" + temp_member_id[i] + "'", cnn);
                cnn.Open();
                temp_member[i] = Convert.ToString(cmd.ExecuteScalar());//存取玩家姓名
                cnn.Close();
            }
            cnn.Close();
            return temp_member;
        }

        private void update_room()//更新房內人數
        {
            String[] member = query_room_member();
            for(int i = 0; i < member.Length; i++)//顯示各個玩家名稱
            {
                Player_GroupBox[i].Visible = true;
                if( i == 0)
                {
                    Player_GroupBox[i].Text = "莊家-" + member[i];
                }
                else
                {
                    Player_GroupBox[i].Text = "玩家" + Convert.ToString(i) + "-" +  member[i];
                }
                
                for (int j = 0; j < 5; j++)
                {
                    Card[i, j].Image = Picture_Card[0];//蓋牌
                }
            }
        }

        private void Update_Card(int now,int Point)//發牌
        {
            int fr = 0;
            while (CardNumber[now, fr] != 0)//計算呼叫者現在有幾張牌了
            {
                fr++;
                if (fr == 5) return;//若拿了五張牌了 不繼續發
            }
            CardNumber[now, fr] = Point;//寫入抽到的牌
            Card[now, fr].Image = Picture_Card[Point];//顯示抽到的牌
            supply_btn[Convert.ToInt32(Now_Position)].Enabled = true;
        }
 

        private void Update_Result(String[] temp)//
        {
            for (int i = 1; i < temp.Length; i++)
            {
                supply_btn[i].Text = temp[i];
            }
        }

        private void enable_all()//遊戲結束 關閉所有補牌功能 
        {
            //關閉補牌控制項
            for (int i = 0; i < 7; i++)
            {
                supply_btn[i].Enabled = false;
            }
        }

        //控制項區Start--------------------------------------------------------------------------------------
        private PictureBox Find_PictureBox_Control(string controlName)  // 尋找PictureBox控制項
        {
            //MessageBox.Show(controlName);
            foreach (Control ctl in this.Controls)
            {
                if (ctl is GroupBox)
                {
                    foreach (Control ctl2 in ctl.Controls)
                    {
                        if (ctl2 is PictureBox)
                        {
                            if (ctl2.Name == controlName) return (PictureBox)ctl2;//MessageBox.Show(ctl2.Name);
                        }
                    }
                }
            }
            return null;
        }

        private GroupBox Find_GroupBox_Control(string controlName)  // 尋找GroupBox控制項
        {
            foreach (Control ctl in this.Controls)
            {
                if (ctl.Name == controlName) return (GroupBox)ctl;// MessageBox.Show(ctl.Name);
            }
            return null;
        }

        private Button Find_Button_Control(string controlName)  // 尋找Button控制項
        {
            //MessageBox.Show(controlName);
            foreach (Control ctl in this.Controls)
            {
                if (ctl is GroupBox)
                {
                    foreach (Control ctl2 in ctl.Controls)
                    {
                        if (ctl2 is Button)
                        {
                            if (ctl2.Name == controlName) return (Button)ctl2;//MessageBox.Show(ctl2.Name);
                        }
                    }
                }
            }
            return null;
        }

        private TextBox Find_Textbox_Control(string controlName)  // 尋找TextBox控制項
        {
            //MessageBox.Show(controlName);
            foreach (Control ctl in this.Controls)
            {
                if (ctl is GroupBox)
                {
                    foreach (Control ctl2 in ctl.Controls)
                    {
                        if (ctl2 is TextBox)
                        {
                            if (ctl2.Name == controlName) return (TextBox)ctl2;//MessageBox.Show(ctl2.Name);
                        }
                    }
                }
            }
            return null;
        }
        //控制項區End--------------------------------------------------------------------------------------

        private void Button9_Click(object sender, EventArgs e)//莊家確定
        {
            send_bit_msg(textBox1.Text);
            textBox1.Enabled = false;
            button9.Enabled = false;
            send_GiveCard_msg(0); send_GiveCard_msg(0);
            send_dealer_msg("Close");//通知蓋牌
        }

        private void Button10_Click(object sender, EventArgs e)//玩家1確定
        {
            send_bit_msg(textBox2.Text);
            textBox2.Enabled = false;
            button10.Enabled = false;
            send_GiveCard_msg(1); send_GiveCard_msg(1);
        }

        private void Button11_Click(object sender, EventArgs e)//玩家二確定
        {
            send_bit_msg(textBox3.Text);
            textBox3.Enabled = false;
            button11.Enabled = false;
            send_GiveCard_msg(2); send_GiveCard_msg(2);
        }

        private void Button12_Click(object sender, EventArgs e)//玩家三確定
        {
            send_bit_msg(textBox4.Text);
            textBox4.Enabled = false;
            button12.Enabled = false;
            send_GiveCard_msg(3); send_GiveCard_msg(3);
        }

        private void Button13_Click(object sender, EventArgs e)//玩家四確定
        {
            send_bit_msg(textBox5.Text);
            textBox5.Enabled = false;
            button13.Enabled = false;
            send_GiveCard_msg(4); send_GiveCard_msg(4);
        }

        private void Button14_Click(object sender, EventArgs e)//玩家五確定
        {
            send_bit_msg(textBox6.Text);
            textBox6.Enabled = false;
            button14.Enabled = false;
            send_GiveCard_msg(5); send_GiveCard_msg(5);
        }

        private void Button15_Click(object sender, EventArgs e)//玩家六確定
        {
            send_bit_msg(textBox7.Text);
            // Player_bit[6] = Convert.ToInt32(textBox7.Text);
            textBox7.Enabled = false;
            button15.Enabled = false;
            send_GiveCard_msg(6); send_GiveCard_msg(6);
        }

        private void Button1_Click_1(object sender, EventArgs e)//開牌
        {
            send_dealer_msg("Open," + Room_id + "," + User_id);//通知開牌
        }

        private void Button2_Click(object sender, EventArgs e)//玩家1補牌
        {
            send_GiveCard_msg(1);
            //GiveCard(1); Check_Out(1);
        }

        private void Button3_Click(object sender, EventArgs e)//玩家2補牌
        {
            send_GiveCard_msg(2);
            //GiveCard(2); Check_Out(2);
        }

        private void Button4_Click(object sender, EventArgs e)//玩家3補牌
        {
            send_GiveCard_msg(3);
            //GiveCard(3); Check_Out(3);
        }

        private void Button5_Click(object sender, EventArgs e)//玩家4補牌
        {
            send_GiveCard_msg(4);
            //GiveCard(4); Check_Out(4);
        }

        private void Button6_Click(object sender, EventArgs e)//玩家5補牌
        {
            send_GiveCard_msg(5);
            //GiveCard(5); Check_Out(5);
        }

        private void Button7_Click(object sender, EventArgs e)//玩家6補牌
        {
            send_GiveCard_msg(6);
            //GiveCard(6); Check_Out(6);
        }

        private void 說明ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("玩家點數高於莊家時獲勝，或者是莊家爆牌，其點數必須等於或低於21點；" +
                "\n超過21點的玩家稱為爆牌。2點至10點的牌以牌面的點數計算，J、Q、K 每張為10點。" +
                "\nA可記為1點或為11點，若玩家會因A而爆牌則A可算為1 點。");
        }
        private void 結束ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            udp.Close();
            this.Close();
        }

        private void Timer1_Tick(object sender, EventArgs e)//監聽
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);//
            if (udp.Available > 0)
            {
                String msg = Encoding.Unicode.GetString(udp.Receive(ref ep));//取得收到的字串
                //MessageBox.Show(msg);
                String[] ans = Center.Decrypt(Center.private_key,msg).Split(',');
                if(ans[0] != "Fasle")
                {
                    if (ans[0] == "Update")//更新遊戲室
                    {
                        update_room();
                    }
                    else if (ans[0] == "Update_Card")//更新遊戲室牌組
                    {
                        Update_Card(Convert.ToInt32(ans[1]), Convert.ToInt32(ans[2]));
                    }
                    else if (ans[0] == "Dealer_Open")//打開莊家牌
                    {
                        Card[0, 1].Image = Picture_Card[CardNumber[0, 1]];//莊家打開隱藏的牌
                    }
                    else if (ans[0] == "Dealer_Close" && Now_Position != "0")//蓋掉莊家牌
                    {
                        Card[0, 1].Image = Picture_Card[0];//莊家蓋一張牌
                    }
                    else if (ans[0] == "Update_Bit")//更新遊戲室賭金顯示
                    {
                        bit_box[Convert.ToInt32(ans[1])].Text = ans[2];
                    }
                    else if (ans[0] == "Result")//更新遊戲室牌組
                    {
                        Update_Result(ans);
                        enable_all();
                    }
                }
                
                //textBox1.Text += ep.Address.ToString() + "：" + DateTime.Now.ToString() + " " + Encoding.Unicode.GetString(msg) + Environment.NewLine;
            }
        }

        private void send_connect_msg()//傳送連線訊息
        {
            byte[] msg = Encoding.Unicode.GetBytes(Center.encryption(Room_Head_id, "Connect," + User_id + "," + User_ip)); // type = Connect連線
            udp1.Send(msg, msg.Length, ep_send);
        }

        private void send_GiveCard_msg(int now)//傳送補牌or發牌訊息
        {
            int fr = 0;
            while (CardNumber[now, fr] != 0)//計算呼叫者現在有幾張牌了
            {
                fr++;
                if (fr == 5) return;//若拿了五張牌了 不繼續發
            }
            byte[] msg = Encoding.Unicode.GetBytes(Center.encryption(Room_Head_id, "GiveCard," + Now_Position + "," + User_id + "," + Room_id)); // type = 
            udp1.Send(msg, msg.Length, ep_send);
        }

        private void send_dealer_msg(String temp)//傳送莊家蓋or翻牌訊息
        {
            byte[] msg = Encoding.Unicode.GetBytes(Center.encryption(Room_Head_id, "Dealer_" + temp)); // type = open or close 莊家覆蓋的那張牌要不要打開
            udp1.Send(msg, msg.Length, ep_send);
        }

        private void send_bit_msg(String temp)//傳送賭金資訊 temp = 賭金
        {
            byte[] msg = Encoding.Unicode.GetBytes(Center.encryption(Room_Head_id, "Bit," + temp + "," + User_id + "," + Now_Position)); // type 
            udp1.Send(msg, msg.Length, ep_send);
        }

        private void send_msg(String temp)//傳送訊息
        {
            byte[] msg = Encoding.Unicode.GetBytes(temp); // type = Connect連線
            udp1.Send(msg, msg.Length, ep_send);
        }

    }
}