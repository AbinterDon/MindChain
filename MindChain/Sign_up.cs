using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace MindChain
{
    public partial class Sign_up : Form
    {
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

        SqlConnection cnn = new SqlConnection("Data Source=" + Center.DataBase_ip +
            ";Initial Catalog=" + Center.DataBase_Name +
            ";User ID=" + Center.DataBase_id +
            ";Password=" + Center.DataBase_Password + "");//資料庫參數

        public Sign_up()
        {
            InitializeComponent();
        }

        //註冊按鈕
        private void button1_Click(object sender, EventArgs e)
        {
            string account = textBox1.Text;
            string username = textBox2.Text;
            string password = textBox3.Text;
            string repeat_password = textBox4.Text;

            Sign_in sign_in = new Sign_in();

            try
            {
                if (password == repeat_password && account != "" && username != "" && password != "")
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO member_table(member_account,member_username,member_password) VALUES('" + account + "','" + username + "','" + password + "')", cnn);
                    cnn.Open();
                    cmd.Connection = cnn;
                    cmd.ExecuteNonQuery();
                    cnn.Close();

                    //新增錢
                    cmd = new SqlCommand("INSERT INTO dbo.member_coin(member_coin) VALUES('1000')", cnn);
                    cnn.Open();
                    cmd.Connection = cnn;
                    cmd.ExecuteNonQuery();
                    cnn.Close();

                    //新增 user 的公鑰
                    cnn.Open();
                    cmd = new SqlCommand("INSERT INTO dbo.member_publickey_table(member_publickey) VALUES('" + rsa.ToXmlString(true) + "')", cnn);
                    cmd.Connection = cnn;
                    cmd.ExecuteNonQuery();
                    cnn.Close();

                    MessageBox.Show("註冊成功");
                    this.Hide();
                    sign_in.Show();
                }
                else
                {
                    MessageBox.Show("註冊失敗");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("資料庫連接失敗");
            }
        }
    }
}
