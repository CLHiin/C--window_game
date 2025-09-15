using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PixelProcessing;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.CvEnum;

namespace S12360740
{
    public partial class Form1 : Form
    {
        int iPanelRBLy, iFH_PRTH, iFH_PLH, ipanel1w_picW;
        double percent;
        PixelProcess f = new PixelProcess();
        static Bitmap originalbmp;
        static int currentImage = 0;
        static CreateTabPage SelectedTabPage;

        public Form1()
        {
            InitializeComponent();
            LetEnabled(false);
            ToolStaus(false);
            iPanelRBLy = panel1.Location.Y;
            iFH_PRTH = this.Height - panel1.Size.Height;
            iFH_PLH = this.Height - panel2.Height;
            ipanel1w_picW = this.Width - panel2.Width;
        }
        public partial class CreateTabPage : TabPage
        {
            public PictureBox pictureBox1;
            public Bitmap Image;
            public string path = "";
            public CreateTabPage(Bitmap Image, string filename)
            {
                this.AutoScroll = false;
                this.Text = filename.Remove(0, filename.LastIndexOf("\\") + 1);
                path = filename; ;
                pictureBox1 = new PictureBox();
                this.pictureBox1.Location = new System.Drawing.Point(1, 1);
                this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox1.Width = Image.Width;
                pictureBox1.Height = Image.Height;
                this.pictureBox1.BorderStyle = BorderStyle.FixedSingle;
                pictureBox1.Image = Image;
                this.Controls.Add(pictureBox1);
                currentImage++;
            }
        }
        private void Newpage(Bitmap newMap)
        {
            tabCtrlImage.Visible = true;
            SelectedTabPage = (CreateTabPage)tabCtrlImage.SelectedTab;
            string title = SelectedTabPage.path;
            CreateTabPage NewTabPage = new CreateTabPage(newMap, title);
            tabCtrlImage.TabPages.Add(NewTabPage);
            tabCtrlImage.SelectTab(NewTabPage);
        }        
        private void 說明ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 aboutbox = new AboutBox1();
            aboutbox.StartPosition = FormStartPosition.CenterScreen;
            aboutbox.ShowDialog();
        }
        private void LetEnabled(bool t) 
        {
            存檔toolStripButton.Enabled =
            關檔toolStripButton.Enabled = 
            存檔ToolStripMenuItem.Enabled = 
            關檔ToolStripMenuItem.Enabled = 
            處理ToolStripMenuItem.Enabled =
            Emgu處理ToolStripMenuItem.Enabled =
            FitImage.Enabled = 
            Larger.Enabled = 
            Smaller.Enabled = 
            Full.Enabled = t;
        }
        private void ToolStaus(bool b)
        {
            toolStripStatusLabel1.Text = "文件夾：";
            toolStripStatusLabel2.Text = "圖片解析度：";
            toolStripStatusLabel3.Text = "倍率：";
            toolStripStatusLabel4.Text = "PixelFormat：";
            if (b)
            {
                SelectedTabPage = (CreateTabPage)tabCtrlImage.SelectedTab;
                toolStripStatusLabel1.Text += SelectedTabPage.path;
                toolStripStatusLabel2.Text += SelectedTabPage.pictureBox1.Image.Width + "x" + SelectedTabPage.pictureBox1.Image.Height;
                toolStripStatusLabel3.Text += (int)(double)SelectedTabPage.pictureBox1.Width / (double)SelectedTabPage.pictureBox1.Image.Width * 100 + "%";
                toolStripStatusLabel4.Text += SelectedTabPage.pictureBox1.Image.PixelFormat.ToString();
            }
        }
        private void buttonOpen_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "PNG(*.PNG)|*.PNG|JPEG(*.JPG)|*.JPG|BNP(*.BMP)|*.BNP";
            openFileDialog1.Title = "開啟影像檔";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                tabCtrlImage.Visible = true;
                Bitmap Image = new Bitmap(openFileDialog1.FileName);
                string title = openFileDialog1.FileName;
                CreateTabPage newTabPage = new CreateTabPage(Image, title);
                tabCtrlImage.TabPages.Add(newTabPage);
                tabCtrlImage.SelectTab(newTabPage);
                openFileDialog1.Dispose();
                SelectedTabPage = (CreateTabPage)tabCtrlImage.SelectedTab;
                originalbmp = new Bitmap(SelectedTabPage.pictureBox1.Image);
                LetEnabled(true);
                ToolStaus(true);
            }
        }
        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (currentImage == 0)
            {
                MessageBox.Show("尚未開啟檔案");
            }
            else
            {
                saveFileDialog1.FileName = "";
                saveFileDialog1.Filter = "PNG(*.PNG)|*.PNG|JPEG(*.JPG)|*.JPG|BNP(*.BMP)|*.BNP";
                saveFileDialog1.Title = "儲存影像檔";
                SelectedTabPage = (CreateTabPage)tabCtrlImage.SelectedTab;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    SelectedTabPage.pictureBox1.Image.Save(saveFileDialog1.FileName);
                }
            }
        }
        private void buttonClose_Click(object sender, EventArgs e)
        {
            currentImage--;
            tabCtrlImage.TabPages.Remove(tabCtrlImage.SelectedTab);
            if (tabCtrlImage.SelectedTab == null)
            {
                tabCtrlImage.Visible = false;
                LetEnabled(false);
                ToolStaus(false);
            }
        }
        private void buttonOut_Click(object sender, EventArgs e) {Close();}
        private void 平均法ToolStripMenuItem_Click(object sender, EventArgs e) 
        {
            SelectedTabPage = (CreateTabPage)tabCtrlImage.SelectedTab;
            SelectedTabPage.path = "灰階（平均法）";
            Newpage(f.rgb2gray(new Bitmap(SelectedTabPage.pictureBox1.Image), 1));
            Adjust_ImageShow(0);
        }
        private void 亮度法ToolStripMenuItem_Click(object sender, EventArgs e) 
        {
            SelectedTabPage = (CreateTabPage)tabCtrlImage.SelectedTab;
            SelectedTabPage.path = "灰階(亮度法）";
            Newpage(f.rgb2gray(new Bitmap(SelectedTabPage.pictureBox1.Image), 2));
            Adjust_ImageShow(0);
        }
        private void 紅色濾鏡ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectedTabPage = (CreateTabPage)tabCtrlImage.SelectedTab;
            SelectedTabPage.path = "紅色濾鏡";
            Newpage(f.rgb2ColorFilter(new Bitmap(SelectedTabPage.pictureBox1.Image), 1));
            Adjust_ImageShow(0);
        }
        private void 綠色濾鏡ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectedTabPage = (CreateTabPage)tabCtrlImage.SelectedTab;
            SelectedTabPage.path = "綠色濾鏡";
            Newpage(f.rgb2ColorFilter(new Bitmap(SelectedTabPage.pictureBox1.Image), 2));
            Adjust_ImageShow(0);
        }
        private void 藍色濾鏡ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectedTabPage = (CreateTabPage)tabCtrlImage.SelectedTab;
            SelectedTabPage.path = "藍色濾鏡";
            Newpage(f.rgb2ColorFilter(new Bitmap(SelectedTabPage.pictureBox1.Image), 3));
            Adjust_ImageShow(0);
        }
        private void 補色ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectedTabPage = (CreateTabPage)tabCtrlImage.SelectedTab;
            SelectedTabPage.path = "補色";
            Newpage(f.rgb2Complement(new Bitmap(SelectedTabPage.pictureBox1.Image)));
            Adjust_ImageShow(0);
        }
        private void 轉黑白ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectedTabPage = (CreateTabPage)tabCtrlImage.SelectedTab;
            SelectedTabPage.path = "轉黑白";
            Newpage(f.gray2binarization(new Bitmap(SelectedTabPage.pictureBox1.Image), 128));
            Adjust_ImageShow(0);
        }
        private void 加入文字ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectedTabPage = (CreateTabPage)tabCtrlImage.SelectedTab;
            SelectedTabPage.path = "加入文字";
            Newpage(new Bitmap(SelectedTabPage.pictureBox1.Image));
            SelectedTabPage = (CreateTabPage)tabCtrlImage.SelectedTab;
            SolidBrush brush = new SolidBrush(Color.Red);
            f.ColorPlane((Bitmap)SelectedTabPage.pictureBox1.Image);
            Bitmap tempbmp = f.ColorPlane2Gray(f.G);
            Graphics graphics = Graphics.FromImage(SelectedTabPage.pictureBox1.Image);
            graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            string str = "Hello World";
            PointF P = new PointF(10, 20);
            Font font = new Font(this.Font.Name, 25);
            graphics.DrawString(str, font, brush, P);
            Adjust_ImageShow(0);
        }
        Mat InMat;//global Image Matrix for EmguCV
        public enum EcolorType
        {
            RedChannel = 0,GreenChannel,BlueChannel
        } //列舉
        private Emgu.CV.Image<Bgr, Byte> ChannelConvert(Emgu.CV.Image<Bgr, Byte> bitmap, EcolorType type)
        {
            Emgu.CV.Image<Bgr, Byte> ConvertImage;
            // 顏色轉換
            switch (type)
            {
                case EcolorType.RedChannel:
                    ConvertImage = bitmap.Sub(new Bgr(255, 255, 0)).Clone();
                    break;
                case EcolorType.GreenChannel:
                    ConvertImage = bitmap.Sub(new Bgr(255, 0, 255)).Clone();
                    break;
                case EcolorType.BlueChannel:
                    ConvertImage = bitmap.Sub(new Bgr(0, 255, 255)).Clone();
                    break;
                default: throw new NullReferenceException("其他類型");
            }
            return ConvertImage;
        }
        private void bit8ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectedTabPage = (CreateTabPage)tabCtrlImage.SelectedTab;
            SelectedTabPage.path = "灰階處理 - 8bits(Emgu.CV.Mat)";
            Newpage(new Bitmap(SelectedTabPage.pictureBox1.Image));
            SelectedTabPage = (CreateTabPage)tabCtrlImage.SelectedTab;

            InMat = Emgu.CV.BitmapExtension.ToMat((Bitmap)SelectedTabPage.pictureBox1.Image);
            Mat myMat = new Mat();
            CvInvoke.CvtColor(InMat, myMat, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
            SelectedTabPage.pictureBox1.Image = myMat.ToBitmap();   //顯示處理過後的圖檔
            myMat.Dispose();
            InMat.Dispose();
            Adjust_ImageShow(0);
        }
        private void bit24ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SelectedTabPage = (CreateTabPage)tabCtrlImage.SelectedTab;
            SelectedTabPage.path = "灰階處理 - 24bits(Emgu.CV.Mat)";
            Newpage(new Bitmap(SelectedTabPage.pictureBox1.Image));
            SelectedTabPage = (CreateTabPage)tabCtrlImage.SelectedTab;

            InMat = Emgu.CV.BitmapExtension.ToMat((Bitmap)SelectedTabPage.pictureBox1.Image);
            VectorOfMat vectorOfMat = new VectorOfMat();
            Emgu.CV.Image<Bgr, Byte> myImage = new Bitmap(InMat.ToBitmap()).ToImage<Bgr, Byte>();
            Emgu.CV.Image<Gray, Byte> tempImage = myImage.Convert<Gray, Byte>().Clone();
            vectorOfMat.Push(tempImage);
            vectorOfMat.Push(tempImage);
            vectorOfMat.Push(tempImage);
            CvInvoke.Merge(vectorOfMat, InMat);
            SelectedTabPage.pictureBox1.Image = InMat.ToBitmap();   //顯示處理過後的圖檔
            InMat.Dispose();
            Adjust_ImageShow(0);
        }
        private void 紅色濾鏡ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SelectedTabPage = (CreateTabPage)tabCtrlImage.SelectedTab;
            SelectedTabPage.path = "紅色濾鏡(Emgu.CV.Image)_enum";
            Newpage(new Bitmap(SelectedTabPage.pictureBox1.Image));
            SelectedTabPage = (CreateTabPage)tabCtrlImage.SelectedTab;

            Emgu.CV.Image<Bgr, Byte> myImage = new Bitmap(SelectedTabPage.pictureBox1.Image).ToImage<Bgr, Byte>();
            SelectedTabPage.pictureBox1.Image = (Bitmap)ChannelConvert(myImage, EcolorType.RedChannel).ToBitmap().Clone();
            myImage.Dispose();
            Adjust_ImageShow(0);
        }
        private void 綠色濾鏡ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SelectedTabPage = (CreateTabPage)tabCtrlImage.SelectedTab;
            SelectedTabPage.path = "綠色濾鏡(Emgu.CV.Image)_enum";
            Newpage(new Bitmap(SelectedTabPage.pictureBox1.Image));
            SelectedTabPage = (CreateTabPage)tabCtrlImage.SelectedTab;

            Emgu.CV.Image<Bgr, Byte> myImage = new Bitmap(SelectedTabPage.pictureBox1.Image).ToImage<Bgr, Byte>();
            SelectedTabPage.pictureBox1.Image = (Bitmap)ChannelConvert(myImage, EcolorType.GreenChannel).ToBitmap().Clone();
            myImage.Dispose();
            Adjust_ImageShow(0);
        }
        private void 藍色濾鏡ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SelectedTabPage = (CreateTabPage)tabCtrlImage.SelectedTab;
            SelectedTabPage.path = "藍色濾鏡(Emgu.CV.Image)_enum";
            Newpage(new Bitmap(SelectedTabPage.pictureBox1.Image));
            SelectedTabPage = (CreateTabPage)tabCtrlImage.SelectedTab;
            
            Emgu.CV.Image<Bgr, Byte> myImage = new Bitmap(SelectedTabPage.pictureBox1.Image).ToImage<Bgr, Byte>();
            SelectedTabPage.pictureBox1.Image = (Bitmap)ChannelConvert(myImage, EcolorType.BlueChannel).ToBitmap().Clone();         
            myImage.Dispose();
            Adjust_ImageShow(0);
        }
        private void 補色ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SelectedTabPage = (CreateTabPage)tabCtrlImage.SelectedTab;
            SelectedTabPage.path = "補色處理(Emgu.CV.Image)";
            Newpage(new Bitmap(SelectedTabPage.pictureBox1.Image));
            SelectedTabPage = (CreateTabPage)tabCtrlImage.SelectedTab;
            
            Emgu.CV.Image<Bgr, Byte> myImage = new Bitmap(SelectedTabPage.pictureBox1.Image).ToImage<Bgr, Byte>();
            SelectedTabPage.pictureBox1.Image = (~myImage).ToBitmap();
            myImage.Dispose();
            Adjust_ImageShow(0);
        }
        private void 轉黑白ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SelectedTabPage = (CreateTabPage)tabCtrlImage.SelectedTab;
            SelectedTabPage.path = "轉黑白處理(Emgu.CV.Mat)";
            Newpage(new Bitmap(SelectedTabPage.pictureBox1.Image));
            SelectedTabPage = (CreateTabPage)tabCtrlImage.SelectedTab;

            InMat = Emgu.CV.BitmapExtension.ToMat((Bitmap)SelectedTabPage.pictureBox1.Image);
            Mat[] channels = InMat.Split();
            VectorOfMat vectorOfMat = new VectorOfMat();
            CvInvoke.Threshold(channels[1], channels[1], 128, 255, ThresholdType.Binary);
            for (int i = 0; i < 3; i++) vectorOfMat.Push(channels[1]);
            CvInvoke.Merge(vectorOfMat, InMat);
            SelectedTabPage.pictureBox1.Image = InMat.ToBitmap();
            InMat.Dispose();
            Adjust_ImageShow(0);
        }
        private void 加入文字ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SelectedTabPage = (CreateTabPage)tabCtrlImage.SelectedTab;
            SelectedTabPage.path = "圖中放文字(Emgu.CV.Mat)";
            Newpage(new Bitmap(SelectedTabPage.pictureBox1.Image));
            SelectedTabPage = (CreateTabPage)tabCtrlImage.SelectedTab;
            
            InMat = Emgu.CV.BitmapExtension.ToMat((Bitmap)SelectedTabPage.pictureBox1.Image);
            Emgu.CV.CvInvoke.PutText(
                    InMat,
                    "Hello, World",
                    new System.Drawing.Point(10, InMat.Height / 2), //(x,y)座標
                    Emgu.CV.CvEnum.FontFace.HersheyScriptComplex, //設定字型
                    4.0,  //大小
                    new Emgu.CV.Structure.Bgr(255, 0, 0).MCvScalar); //表示使用藍色
            SelectedTabPage.pictureBox1.Image = InMat.ToBitmap();   //顯示處理過後的圖檔
            InMat.Dispose();
            Adjust_ImageShow(0);
        }
        
        
         /*
        public formImageProcessing()
        {
            InitializeComponent();
            LetEnabled(true);
            ToolStaus(true);
            iPanelRBLy = panel1.Location.Y;
            iFH_PRTH = this.Height - panel1.Size.Height;
            iFH_PLH = this.Height - panel2.Height;
            ipanel1w_picW = this.Width - panel2.Width;
        }*/
        private void TbtnFitImage_Click(object sender, EventArgs e)
        {
            Adjust_ImageShow(0);
        }
        private void TbtnLarger_Click(object sender, EventArgs e)
        {
            Adjust_ImageShow(1);
        }
        private void TbtnSmaller_Click(object sender, EventArgs e)
        {
            Adjust_ImageShow(2);
        }
        private void TbtnFull_Click(object sender, EventArgs e)
        {
            Adjust_ImageShow(3);
        }
        private void Adjust_ImageShow(int adjustflags)
        {
            SelectedTabPage = (CreateTabPage)tabCtrlImage.SelectedTab;
            Bitmap tempImage = new Bitmap(SelectedTabPage.pictureBox1.Image);
            double ratiol = (double)tempImage.Height / (double)tempImage.Width;
            double ratioP = (double)(tabCtrlImage.Height - 30) / (double)(tabCtrlImage.Width - 12);
            switch (adjustflags)
            {
                case 0:
                    if (ratiol > ratioP)
                    {
                        SelectedTabPage.pictureBox1.Height = (int)tabCtrlImage.Height - 30;
                        SelectedTabPage.pictureBox1.Width = (int)(SelectedTabPage.pictureBox1.Height / ratiol);
                    }
                    else
                    {
                        SelectedTabPage.pictureBox1.Width = (int)(tabCtrlImage.Width - 12);
                        SelectedTabPage.pictureBox1.Height = (int)(SelectedTabPage.pictureBox1.Width * ratiol);
                    }
                    SelectedTabPage.AutoScroll = false;
                    percent = (double)(SelectedTabPage.pictureBox1.Width) / (double)(tempImage.Width);
                    break;
                case 1:
                    if (percent < 8)
                    {
                        percent = (double)(SelectedTabPage.pictureBox1.Width) / (double)(tempImage.Width) * 1.5;
                        if (percent > 8) percent = 8;
                        SelectedTabPage.pictureBox1.Width = (int)((tempImage.Width) * percent);
                        SelectedTabPage.pictureBox1.Height = (int)((double)(SelectedTabPage.pictureBox1.Width) * ratiol);
                    }
                    SelectedTabPage.AutoScroll = true;
                    break;
                case 2:
                    {
                        if (percent > 0.08)
                        {
                            percent = (double)(SelectedTabPage.pictureBox1.Width) / (double)(tempImage.Width) / 1.5;
                            if (percent < 0.08) percent = 0.08;
                            SelectedTabPage.pictureBox1.Width = (int)((tempImage.Width) * percent);
                            SelectedTabPage.pictureBox1.Height = (int)((double)(SelectedTabPage.pictureBox1.Width) * ratiol);
                        }
                        break;
                    }
                case 3:
                    percent = 1;
                    SelectedTabPage.pictureBox1.Width = tempImage.Width;
                    SelectedTabPage.pictureBox1.Height = tempImage.Height;
                    SelectedTabPage.AutoScroll = true;
                    break;
            }
            ToolStaus(true);
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            panel2.Location = new Point(this.Width - 18 - panel2.Size.Width, panel2.Location.Y);
            panel2.Height = this.Height - iFH_PRTH;
            tabCtrlImage.Height = this.Height - iFH_PLH;
            tabCtrlImage.Width = panel2.Location.X - 7;
            if (SelectedTabPage.pictureBox1 != null) TbtnFitImage_Click(sender, e);
            panel3.Location = new System.Drawing.Point(panel2.Location.X, panel2.Height + iPanelRBLy + 6);
        }
    }
}