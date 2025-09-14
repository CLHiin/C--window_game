using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace S12360740
{
    public partial class Form1 : Form
    {
        Point BXY, PXY;
        int Bmove = 20,click =0,max = 0;
        double bx,by=0.4;
        Random R = new Random();
        bool right = false, left = false;

        public Form1()
        {
            InitializeComponent();
            BXY = button1.Location;
        }

        private void 說明ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 aboutbox = new AboutBox1();
            aboutbox.StartPosition = FormStartPosition.CenterScreen;
            aboutbox.ShowDialog();
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A) left = false;
            if (e.KeyCode == Keys.D) right = false;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A) left = true;
            if (e.KeyCode == Keys.D) right = true;
        }
        /*
        private void From1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Convert.ToChar(e.KeyChar) == 'a')
            {
                BXY.X -= Bmove
                if (BXY.X < 0) BXY.X = 0;
            }
            if (Convert.ToChar(e.KeyChar) == 'd')
            {
                BXY.X += Bmove;
                if (BXY.X > ClientSize.Width - button1.Width) BXY.X = ClientSize.Width - button1.Width;
            }
            button1.Location = BXY;
        }
        */
        private void timer1_Tick(object sender, EventArgs e)
        {
            PXY.X += (int)(Bmove * bx);
            PXY.Y += (int)(Bmove * by);

            if (left ) BXY.X -= Bmove;
            if (right) BXY.X += Bmove;
            if (BXY.X < 0) BXY.X = 0;
            if (BXY.X > ClientSize.Width - button1.Width) BXY.X = ClientSize.Width - button1.Width;

            if (PXY.X < 0 || PXY.X > ClientSize.Width - pictureBox1.Width) bx *= -1;
            if (PXY.Y < 27) by *= -1;
            if (PXY.Y > BXY.Y - pictureBox1.Height)
            {
                by *= -1;
                if (PXY.X + pictureBox1.Width > BXY.X && PXY.X < BXY.X + button1.Width)
                {
                    click++;
                    toolStripTextBox2.Text = "當前分數：" + click.ToString();
                    button1.Width -= button1.Width / 25;
                    if (timer1.Interval > 1) timer1.Interval--;
                }
                else 
                {
                    toolStripTextBox1.Visible = true;
                    max = max > click ? max : click;
                    toolStripTextBox1.Text = "最高分："+(max).ToString();
                    timer1.Enabled = false;
                    MessageBox.Show("結束了啦！要玩自己再去按開始。");
                }
            }
            button1.Location = BXY;
            pictureBox1.Location = PXY;
        }
        private void 開始玩ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = true;
            
            PXY = new Point(R.Next(0, 750), 30);
            pictureBox1.Location = PXY;

            bx = (double)R.Next(4,7)/10;
            if (PXY.X % 2 == 0) bx *= -1;
            button1.Width = 500;

            timer1.Enabled = true;
            timer1.Interval = 30;

            click = 0;
            toolStripTextBox2.Text = "當前分數：" + click.ToString();
        }
    }
}
