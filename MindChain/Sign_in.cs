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
    public partial class Sign_in : Form
    {

        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        SqlConnection cnn;
        SqlCommand cmd;

        public Sign_in()
        {
            InitializeComponent();
        }

        private void 系統設定ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Config config = new Config();
            config.Show();
        }

        private void 離開ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void sign_in_btn_Click(object sender, EventArgs e)
        {
            
            try
            {
                bool ck = false;
                cnn = new SqlConnection("Data Source=" + Center.DataBase_ip +
                ";Initial Catalog=" + Center.DataBase_Name +
                ";User ID=" + Center.DataBase_id +
                ";Password=" + Center.DataBase_Password + "");//資料庫參數

                cmd = new SqlCommand("Select * from dbo.member_table where member_account='" + username_txt.Text + "' and member_password='" + password_txt.Text + "'", cnn);
                cnn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    if (!dr[0].Equals(DBNull.Value))
                    {
                        string User_id = dr.GetValue(1).ToString();
                        string username = dr.GetValue(4).ToString();
                        Center center = new Center();
                        Center.User_id = User_id;
                        Center.username = username;
                        ck = true;
                        //cnn.Open();
                        center.Show();
                        this.Hide();
                    }
                }
                cnn.Close();
                if(ck==true)get_key(Center.User_id, cnn, cmd);
            }
            catch(Exception ex)
            {
                MessageBox.Show("登入失敗");
            }

            

        }

        private void sign_up_btn_Click(object sender, EventArgs e)
        {
            Sign_up sign_up = new Sign_up();
            sign_up.Show();
            this.Hide();
        }

        //取得公私鑰，並把公鑰存進資料庫，私鑰自己保存
        public void get_key(string User_id, SqlConnection cnn, SqlCommand cmd)
        {

            Center.private_key = rsa.ToXmlString(true); //取得私鑰
            //cnn.Close();
            cnn.Open();
            cmd = new SqlCommand("UPDATE dbo.member_publickey_table set member_publickey = '" + rsa.ToXmlString(false) + "' where member_id_add = '" + User_id + "'", cnn);
            cmd.Connection = cnn;
            cmd.ExecuteNonQuery();
            cnn.Close();
            //MessageBox.Show(Center.private_key);
            //MessageBox.Show(rsa.ToXmlString(false));
        }

    }
}
