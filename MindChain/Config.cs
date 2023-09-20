using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MindChain
{
    public partial class Config : Form
    {
        public Config()
        {
            InitializeComponent();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Button1_Click(object sender, EventArgs e)//儲存
        {
            Center.DataBase_ip = textBox1.Text;
            Center.DataBase_Name = textBox5.Text;
            Center.DataBase_id = textBox6.Text;
            Center.DataBase_Password = textBox7.Text;
            MessageBox.Show("設定完成");
            this.Close();
        }
    }
}
