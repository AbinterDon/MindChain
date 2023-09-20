namespace MindChain
{
    partial class Center
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Center));
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.Reload = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.user_coin_txt = new System.Windows.Forms.Label();
            this.coin = new System.Windows.Forms.Label();
            this.IP = new System.Windows.Forms.Label();
            this.username_txt = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.user_IP_txt = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(415, 137);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(110, 38);
            this.button1.TabIndex = 9;
            this.button1.Text = "建立遊戲室";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(415, 210);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(110, 38);
            this.button2.TabIndex = 10;
            this.button2.Text = "加入遊戲室";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.Button2_Click);
            // 
            // Reload
            // 
            this.Reload.Location = new System.Drawing.Point(415, 283);
            this.Reload.Margin = new System.Windows.Forms.Padding(4);
            this.Reload.Name = "Reload";
            this.Reload.Size = new System.Drawing.Size(110, 38);
            this.Reload.TabIndex = 18;
            this.Reload.Text = "重新整理";
            this.Reload.UseVisualStyleBackColor = true;
            this.Reload.Click += new System.EventHandler(this.Reload_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 15;
            this.listBox1.Location = new System.Drawing.Point(23, 135);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(334, 259);
            this.listBox1.TabIndex = 17;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button4);
            this.groupBox1.Controls.Add(this.user_IP_txt);
            this.groupBox1.Controls.Add(this.user_coin_txt);
            this.groupBox1.Controls.Add(this.coin);
            this.groupBox1.Controls.Add(this.IP);
            this.groupBox1.Controls.Add(this.username_txt);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Location = new System.Drawing.Point(23, 35);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(565, 72);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "會員資料";
            // 
            // user_coin_txt
            // 
            this.user_coin_txt.AutoSize = true;
            this.user_coin_txt.Location = new System.Drawing.Point(510, 37);
            this.user_coin_txt.Name = "user_coin_txt";
            this.user_coin_txt.Size = new System.Drawing.Size(14, 15);
            this.user_coin_txt.TabIndex = 8;
            this.user_coin_txt.Text = "0";
            // 
            // coin
            // 
            this.coin.AutoSize = true;
            this.coin.Location = new System.Drawing.Point(461, 37);
            this.coin.Name = "coin";
            this.coin.Size = new System.Drawing.Size(49, 15);
            this.coin.TabIndex = 7;
            this.coin.Text = "Coin：";
            // 
            // IP
            // 
            this.IP.AutoSize = true;
            this.IP.Location = new System.Drawing.Point(193, 38);
            this.IP.Name = "IP";
            this.IP.Size = new System.Drawing.Size(35, 15);
            this.IP.TabIndex = 5;
            this.IP.Text = "IP：";
            // 
            // username_txt
            // 
            this.username_txt.AutoSize = true;
            this.username_txt.Location = new System.Drawing.Point(124, 37);
            this.username_txt.Name = "username_txt";
            this.username_txt.Size = new System.Drawing.Size(0, 15);
            this.username_txt.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 38);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(97, 15);
            this.label5.TabIndex = 3;
            this.label5.Text = "使用者名稱：";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(415, 356);
            this.button3.Margin = new System.Windows.Forms.Padding(4);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(110, 38);
            this.button3.TabIndex = 19;
            this.button3.Text = "登出";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click_1);
            // 
            // user_IP_txt
            // 
            this.user_IP_txt.Location = new System.Drawing.Point(234, 31);
            this.user_IP_txt.Name = "user_IP_txt";
            this.user_IP_txt.Size = new System.Drawing.Size(121, 25);
            this.user_IP_txt.TabIndex = 9;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(365, 33);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(58, 23);
            this.button4.TabIndex = 20;
            this.button4.Text = "設定";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // Center
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 426);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.Reload);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Center";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Center";
            this.Load += new System.EventHandler(this.Center_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button Reload;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label user_coin_txt;
        private System.Windows.Forms.Label coin;
        private System.Windows.Forms.Label IP;
        private System.Windows.Forms.Label username_txt;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox user_IP_txt;
        private System.Windows.Forms.Button button4;
    }
}