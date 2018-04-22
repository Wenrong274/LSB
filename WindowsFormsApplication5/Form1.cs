using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApplication5
{
    public partial class Form1 : Form
    {
        public Form1()
        { InitializeComponent(); }

        Bitmap myBitmap1;//原圖
        Bitmap myBitmap2;//Logo
        Bitmap LOGO_RE = new Bitmap(64, 64);//Logo 64 * 64
        string PW;//設置密碼變數
        int[, ,] Logo_Image1 = new int[64, 64, 3];//LOGO
        int[, ,] Logo_Image2 = new int[64, 64, 3];//LOGO
        int[, ,] Ran = new int[256, 256, 3];//LOGO亂數陣列
        int[] bk_x = new int[16384];//亂數迴圈x
        int[] bk_y = new int[16384];//亂數迴圈y
        int[, ,] True_Image1 = new int[256, 256, 3];//原圖
        int[, ,] True_Image2 = new int[256, 256, 3];//原圖

        private void button1_Click(object sender, EventArgs e)//開啟原圖
        {
            this.openFileDialog1.Filter = "所有檔案|*.*|BMP File| *.bmp|JPEG File|*.jpg| GIF File|*.gif";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)   ////由對話框選取圖檔
            {
                myBitmap1 = new Bitmap(openFileDialog1.FileName);

                if (myBitmap1.Height > True_Image1.GetLength(0) || myBitmap1.Width > True_Image1.GetLength(1))
                {
                    myBitmap1 = null;
                    MessageBox.Show("您的圖片過大請輸入256*256的圖片！", "錯誤");
                }
                else
                {
                    pictureBox1.Image = myBitmap1;
                    for (int y = 0; y < myBitmap1.Height; y++) //取得圖像資料
                    {
                        for (int x = 0; x < myBitmap1.Width; x++)
                        {
                            Color c1 = myBitmap1.GetPixel(x, y); // 得到 原始像素 的 Color
                            True_Image1[x, y, 0] = (int)(c1.R);
                            True_Image1[x, y, 1] = (int)(c1.G);
                            True_Image1[x, y, 2] = (int)(c1.B);
                        }
                    }
                }
            }
        }///////////////////////////////////
        private void button2_Click(object sender, EventArgs e)//開啟Logo
        {
            this.openFileDialog2.Filter = "所有檔案|*.*|BMP File| *.bmp|JPEG File|*.jpg| GIF File|*.gif";

            if (openFileDialog2.ShowDialog() == DialogResult.OK)   ////由對話框選取圖檔
            {
                myBitmap2 = new Bitmap(openFileDialog2.FileName);

                if (myBitmap2.Height > Logo_Image1.GetLength(0) || myBitmap2.Width > Logo_Image1.GetLength(1))
                {
                    MessageBox.Show("您的圖片過大請輸入64*64的圖片！", "錯誤");
                    myBitmap2 = null;
                }
                else
                {
                    pictureBox2.Image = myBitmap2;
                    for (int y = 0; y < myBitmap2.Height; y++) //取得圖像資料
                    {
                        for (int x = 0; x < myBitmap2.Width; x++)
                        {
                            Color c1 = myBitmap2.GetPixel(x, y); // 得到 原始像素 的 Color
                            Logo_Image1[x, y, 0] = (int)(c1.R);
                            Logo_Image1[x, y, 1] = (int)(c1.G);
                            Logo_Image1[x, y, 2] = (int)(c1.B);
                        }
                    }
                }
            }
        }///////////////////////////////////
        private void button3_Click(object sender, EventArgs e)//圖片轉換
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("您沒有載入原圖！", "錯誤");
            }
            else if (pictureBox2.Image == null)
            {
                MessageBox.Show("您沒有載入浮水印！", "錯誤");
            }
            else if (textBox1.Text == "請輸入金鑰" || textBox1.Text.Contains(" ") || textBox1.Text == null)
            {
                MessageBox.Show("您沒有輸入金鑰", "錯誤");
            }
            else if (pictureBox1.Image != null & pictureBox2.Image != null & textBox1.Text != null)
            {
                if (!System.IO.File.Exists(@"Text\" + textBox1.Text + "_bky.txt"))
                {
                    wavelet_transformation(myBitmap1);
                    Random_LOGO(myBitmap2);
                    pictureBox3.Image = wavelet_transformation_re(myBitmap1);
                }
                else if (System.IO.File.Exists(@"Text\" + PW + "_bky.txt") && (textBox1.Text != "請輸入金鑰" || !textBox1.Text.Contains(" ") || textBox1.Text != null))
                {
                    MessageBox.Show("密碼重複，請更換密碼", "警告");
                }
            }

        }///////////////////////////////////
        private void button4_Click(object sender, EventArgs e)//圖片輸出
        {
            if ((Bitmap)pictureBox3.Image == null) //防呆
            {
                MessageBox.Show("您沒有原圖(加密)圖片！", "錯誤");
            }
            else
            {
                Output_Image(myBitmap1);
            }
        }///////////////////////////////////
        private void button5_Click(object sender, EventArgs e)//取出logo
        {
            if ((Bitmap)pictureBox3.Image == null)//防呆
            {
                MessageBox.Show("您沒有載入原圖(加密)！", "錯誤");
            }
            else if (!System.IO.File.Exists(@"Text\" + textBox1.Text + "_bky.txt"))
            {
                MessageBox.Show("查無金鑰或未輸入金鑰！", "錯誤");
            }
            else if (System.IO.File.Exists(@"Text\" + textBox1.Text + "_bky.txt"))
            {
                wavelet_transformation(myBitmap1);
                pictureBox4.Image = RE_RadLogo(myBitmap1);
            }
        }///////////////////////////////////
        private void button6_Click(object sender, EventArgs e)//開啟原圖(加密)
        {
            if ((Bitmap)pictureBox1.Image == null)
            {
                MessageBox.Show("您沒有載入原圖！", "錯誤");
            }
            else
            {
                this.openFileDialog1.Filter = "所有檔案|*.*|BMP File| *.bmp|JPEG File|*.jpg| GIF File|*.gif";

                if (openFileDialog1.ShowDialog() == DialogResult.OK)   ////由對話框選取圖檔
                {
                    myBitmap1 = new Bitmap(openFileDialog1.FileName);
                    if (myBitmap1.Height > True_Image1.GetLength(0) || myBitmap1.Width > True_Image1.GetLength(1))
                    {
                        myBitmap1 = null;
                        MessageBox.Show("您的圖片過大請輸入256*256的圖片！", "錯誤");
                    }
                    else
                    {
                        pictureBox3.Image = myBitmap1;
                        for (int y = 0; y < myBitmap1.Height; y += 1) //取得圖像資料
                        {
                            for (int x = 0; x < myBitmap1.Width; x += 1)
                            {
                                Color c1 = myBitmap1.GetPixel(x, y); // 得到 原始像素 的 Color
                                True_Image1[x, y, 0] = (int)(c1.R);
                                True_Image1[x, y, 1] = (int)(c1.G);
                                True_Image1[x, y, 2] = (int)(c1.B);
                            }
                        }
                    }

                }
            }
        }/////////////////////////////
        private void button7_Click(object sender, EventArgs e)//PSNR
        {
            if ((Bitmap)pictureBox1.Image == null || (Bitmap)pictureBox3.Image == null)
            {
                MessageBox.Show("您沒有載入原圖或原圖(加密)！", "錯誤");
            }
            else
            {
                Bitmap bmp = (Bitmap)pictureBox1.Image;
                Bitmap bmp_re = (Bitmap)pictureBox3.Image;
                label4.Text = Convert.ToString(Math.Round(FindPSNR(bmp, bmp_re), 2)+"db");
            }
        }///////////////////////////////////////
        private void button8_Click(object sender, EventArgs e)//BCR
        {
            if ((Bitmap)pictureBox2.Image == null || (Bitmap)pictureBox4.Image == null)
            {
                MessageBox.Show("您沒有Logo圖片或logo(取出)圖片！", "錯誤");
            }
            else
            {
                Bitmap bmp = (Bitmap)pictureBox2.Image;
                Bitmap bmp_re = (Bitmap)pictureBox4.Image;
                label9.Text = Convert.ToString(Math.Round(FindBCR(bmp, bmp_re), 2)+"%");
            }
        }////////////////////////////////////////

        public Bitmap wavelet_transformation(Bitmap Source)//小波轉換
        {
            Bitmap WT = new Bitmap(Source.Width, Source.Height); //建立小波圖的寬高

            /////////////////////一維垂直濾波/////////////////////
            for (int y = 0; y < Source.Height; y++)
            {
                for (int x = 0; x < Source.Width; x += 2)
                {
                    True_Image2[Gous(x), y, 0] = (True_Image1[x, y, 0] + True_Image1[x + 1, y, 0]) / 2;
                    True_Image2[Gous(x) + (Source.Width / 2), y, 0] = (True_Image1[x, y, 0] - True_Image1[x + 1, y, 0]) / 2;
                }
            }
            /////////////////////一維垂直濾波/////////////////////

            /////////////////////一維水平濾波/////////////////////
            for (int x = 0; x < Source.Width; x++)
            {
                for (int y = 0; y < Source.Height; y += 2)
                {
                    True_Image1[x, Gous(y), 0] = (True_Image2[x, y, 0] + True_Image2[x, y + 1, 0]) / 2;
                    True_Image1[x, Gous(y) + (Source.Height / 2), 0] = (True_Image2[x, y, 0] - True_Image2[x, y + 1, 0]) / 2;
                }
            }
            /////////////////////一維水平濾波/////////////////////

            /* 
             /////////////////////二維水平濾波/////////////////////
               for (int y = 0; y < Source.Height / 2; y++)
               {
                   for (int x = 0; x < Source.Width / 2; x += 2)
                   {
                       True_Image2[Gous(x), y, 0] = (True_Image1[x, y, 0] + True_Image1[x + 1, y, 0]) / 2;
                       True_Image2[Gous(x) + (Source.Width / 4), y, 0] = (True_Image1[x, y, 0] - True_Image1[x + 1, y, 0]) / 2;
                   }
               }/////////////////////二維水平濾波/////////////////////

                     /////////////////////二維水平濾波/////////////////////
                     for (int x = 0; x < Source.Width / 2; x++)
                     {
                         for (int y = 0; y < Source.Height / 2; y += 2)
                         {
                             True_Image1[x, Gous(y), 0] = (True_Image2[x, y, 0] + True_Image2[x, y + 1, 0]) / 2;
                             True_Image1[x, Gous(y) + (Source.Height / 4), 0] = (True_Image2[x, y, 0] - True_Image2[x, y + 1, 0]) / 2;
                         }
                     }/////////////////////二維水平濾波/////////////////////

                     /////////////////////三維垂直濾波/////////////////////
                     for (int y = 0; y < Source.Height / 4; y++)
                     {
                         for (int x = 0; x < Source.Width / 4; x += 2)
                         {
                             True_Image2[Gous(x), y, 0] = (True_Image1[x, y, 0] + True_Image1[x + 1, y, 0]) / 2;
                             True_Image2[Gous(x) + (Source.Width / 8), y, 0] = (True_Image1[x, y, 0] - True_Image1[x + 1, y, 0]) / 2;
                         }
                     }/////////////////////三維垂直濾波/////////////////////

                     /////////////////////三維水平濾波/////////////////////
                     for (int x = 0; x < Source.Width / 4; x++)
                     {
                         for (int y = 0; y < Source.Height / 4; y += 2)
                         {
                             True_Image1[x, Gous(y), 0] = (True_Image2[x, y, 0] + True_Image2[x, y + 1, 0]) / 2;
                             True_Image1[x, Gous(y) + (Source.Height / 8), 0] = (True_Image2[x, y, 0] - True_Image2[x, y + 1, 0]) / 2;
                         }
                     }/////////////////////三維水平濾波/////////////////////
                */



            /////////////////////圖檔象素數更新/////////////////////
            for (int x = 0; x < Source.Width; x++)
            {
                for (int y = 0; y < Source.Height; y++)
                {
                    WT.SetPixel(x, y, Color.FromArgb(number_line((int)True_Image1[x, y, 0]), number_line((int)True_Image1[x, y, 0]), number_line((int)True_Image1[x, y, 0])));
                }
            }/////////////////////圖檔象素數更新/////////////////////
            return WT;
        }//////////////////////////////////////
        public Bitmap wavelet_transformation_re(Bitmap Source)//小波還原
        {
            Bitmap wt_re = new Bitmap(Source.Width, Source.Height);

            /* 
             /////////////////////三維垂直還原/////////////////////
                  for (int y = 0; y < Source.Height / 4; y++)
                  {
                      for (int x = 0; x < Source.Width / 4; x += 2)
                      {
                          True_Image2[x, y, 0] = (True_Image1[Gous(x), y, 0] + True_Image1[Gous(x) + (Source.Width / 8), y, 0]);
                          True_Image2[x + 1, y, 0] = (True_Image1[Gous(x), y, 0] - True_Image1[Gous(x) + (Source.Width / 8), y, 0]);
                      }
                  }/////////////////////三維垂直還原/////////////////////

                  /////////////////////三維水平還原/////////////////////
                  for (int x = 0; x < Source.Width / 4; x++)
                  {
                      for (int y = 0; y < Source.Height / 4; y += 2)
                      {
                          True_Image1[x, y, 0] = (True_Image2[x, Gous(y), 0] + True_Image2[x, Gous(y) + (Source.Height / 8), 0]);
                          True_Image1[x, y + 1, 0] = (True_Image2[x, Gous(y), 0] - True_Image2[x, Gous(y) + (Source.Height / 8), 0]);
                      }
                  }/////////////////////三維水平還原/////////////////////

                  /////////////////////二維垂直還原/////////////////////
                  for (int y = 0; y < Source.Height / 2; y++)
                  {
                      for (int x = 0; x < Source.Width / 2; x += 2)
                      {
                          True_Image2[x, y, 0] = (True_Image1[Gous(x), y, 0] + True_Image1[Gous(x) + (Source.Width / 4), y, 0]);
                          True_Image2[x + 1, y, 0] = (True_Image1[Gous(x), y, 0] - True_Image1[Gous(x) + (Source.Width / 4), y, 0]);
                      }
                  }/////////////////////二維垂直還原/////////////////////

                  /////////////////////二維水平還原/////////////////////
                  for (int x = 0; x < Source.Width / 2; x++)
                  {
                      for (int y = 0; y < Source.Height / 2; y += 2)
                      {
                          True_Image1[x, y, 0] = (True_Image2[x, Gous(y), 0] + True_Image2[x, Gous(y) + (Source.Height / 4), 0]);
                          True_Image1[x, y + 1, 0] = (True_Image2[x, Gous(y), 0] - True_Image2[x, Gous(y) + +(Source.Height / 4), 0]);
                      }
                  }/////////////////////二維水平還原/////////////////////
                  */
            /////////////////////一維垂直還原/////////////////////
            for (int y = 0; y < Source.Height; y++)
            {
                for (int x = 0; x < Source.Width; x += 2)
                {
                    True_Image2[x, y, 0] = (True_Image1[Gous(x), y, 0] + True_Image1[Gous(x) + (Source.Width / 2), y, 0]);
                    True_Image2[x + 1, y, 0] = (True_Image1[Gous(x), y, 0] - True_Image1[Gous(x) + (Source.Width / 2), y, 0]);
                }
            }
            /////////////////////一維垂直還原/////////////////////

            /////////////////////一維水平還原/////////////////////
            for (int x = 0; x < Source.Width; x++)
            {
                for (int y = 0; y < Source.Height; y += 2)
                {
                    True_Image1[x, y, 0] = (True_Image2[x, Gous(y), 0] + True_Image2[x, Gous(y) + (Source.Height / 2), 0]);
                    True_Image1[x, y + 1, 0] = (True_Image2[x, Gous(y), 0] - True_Image2[x, Gous(y) + (Source.Height / 2), 0]);
                }
            }
            /////////////////////一維水平還原/////////////////////

            /////////////////////載入還原陣列/////////////////////
            for (int y = 0; y < Source.Height; y++)
            {
                for (int x = 0; x < Source.Width; x++)
                {
                    wt_re.SetPixel(x, y, Color.FromArgb(number_line((int)True_Image1[x, y, 0]), number_line((int)True_Image1[x, y, 0]), number_line((int)True_Image1[x, y, 0])));
                }
            }
            /////////////////////載入還原陣列/////////////////////
            return wt_re;
        }///////////////////////////////////
        public Bitmap Random_LOGO(Bitmap LOGO)//logo亂數
        {
            Bitmap Logo_Image_2 = new Bitmap(LOGO.Width, LOGO.Height); //寫入亂數至Logo_Image_2
            int i = 0;
            int x = 0;
            int y = 0;

            string PW = textBox1.Text;//讀取密碼

            for (int b = 0; b < Ran.GetLength(1); b++)//把亂數正列載入999 用來判斷
            {
                for (int a = 0; a < Ran.GetLength(0); a++)
                {
                    Ran[a, b, 0] = 999;
                }
            }

            while (y < LOGO.Height)
            {
                while (x < LOGO.Width)
                {
                    Random ran1 = new Random(DateTime.Now.Millisecond);
                    Random ran2 = new Random();
                    if (i == LOGO.Height * LOGO.Width)  //i跑完時，強制跳出回圈
                    { break; }
                    while (i < LOGO.Height * LOGO.Width)
                    {
                        bk_x[i] = (int)ran1.Next(0, 64);    //亂數產生0 ~ 127
                        bk_y[i] = (int)ran2.Next(0, 64);
                        if (Ran[bk_x[i], bk_y[i], 0] == 999)  //亂數正列==999  就是未載入亂數
                        {
                            Ran[bk_x[i], bk_y[i], 0] = (int)Logo_Image1[x, y, 0]; //logo亂數打散
                            i++;
                            x++;
                            if (x == 64)
                            { x = 0; y++; }
                        }
                    }
                }
            }

            for (int h = 0; h < myBitmap1.Height; h++)
            {
                for (int w = 0; w < myBitmap1.Width; w++)
                {
                    True_Image2[w, h, 0] = True_Image1[w, h, 0];
                }
            }

            for (int h = 0; h < LOGO.Height; h++)  //將亂數logo 用lsb寫入
            {
                for (int w = 0; w < LOGO.Width; w++)
                {
                    True_Image1[w + 128, h + 128, 0] = ((Convert.ToInt32(Ran[w, h, 0]) >> 6) & 3) | Convert.ToInt32(True_Image2[w + 128, h + 128, 0]);
                    True_Image1[w + 128, h + 192, 0] = ((Convert.ToInt32(Ran[w, h, 0]) >> 4) & 3) | Convert.ToInt32(True_Image2[w + 128, h + 192, 0]);
                    True_Image1[w + 192, h + 128, 0] = ((Convert.ToInt32(Ran[w, h, 0]) >> 2) & 3) | Convert.ToInt32(True_Image2[w + 192, h + 128, 0]);
                    True_Image1[w + 192, h + 192, 0] = ((Convert.ToInt32(Ran[w, h, 0]) & 3)) | Convert.ToInt32(True_Image2[w + 192, h + 192, 0]);
                }
            }

            for (int txt_x = 0; txt_x < LOGO.Height * LOGO.Width; txt_x++)//存為文字檔
            {
                FileStream objFileStream_bkx = new FileStream(@"Text\" + PW + "_bkx.txt", FileMode.Append, FileAccess.Write);
                StreamWriter objStreamWriter_bkx = new StreamWriter(objFileStream_bkx);
                FileStream objFileStream_bky = new FileStream(@"Text\" + PW + "_bky.txt", FileMode.Append, FileAccess.Write);
                StreamWriter objStreamWriter_bky = new StreamWriter(objFileStream_bky);

                if (txt_x < LOGO.Height * LOGO.Width)
                {
                    objStreamWriter_bkx.Write(bk_x[txt_x] + ",");//將字串寫入到文字中
                    objStreamWriter_bky.Write(bk_y[txt_x] + ",");
                }
                objStreamWriter_bkx.Close();
                objStreamWriter_bky.Close();
            }
            return Logo_Image_2;
        }///////////////////////////////////////////////////
        public Bitmap RE_RadLogo(Bitmap Rad_LOGO)//logo亂數還原
        {
            Bitmap T_logo = new Bitmap(LOGO_RE.Width, LOGO_RE.Height);
            int LSB_w, LSB_x, LSB_y, LSB_z;
            PW = textBox1.Text;
            int i = 0;
            string input_bkx = "";
            string input_bky = "";
            string[] str_bkx;
            string[] str_bky;
            StreamReader sr_bkx = new StreamReader(@"Text\" + PW + "_bkx.txt", Encoding.UTF8);
            StreamReader sr_bky = new StreamReader(@"Text\" + PW + "_bky.txt", Encoding.UTF8);
            input_bkx = sr_bkx.ReadToEnd();
            input_bky = sr_bky.ReadToEnd();
            sr_bkx.Close();
            sr_bky.Close();
            str_bkx = input_bkx.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            str_bky = input_bky.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            for (int T_x = 0; T_x < T_logo.Width * T_logo.Height; T_x++)
            {
                bk_x[T_x] = Convert.ToInt32(str_bkx[T_x]);
                bk_y[T_x] = Convert.ToInt32(str_bky[T_x]);
            }

            for (int h = 0; h < myBitmap1.Height / 4; h++)  //亂數logo掃入原圖
            {
                for (int w = 0; w < myBitmap1.Width / 4; w++)
                {
                    LSB_w = ((Convert.ToInt32(True_Image1[w + 128, h + 128, 0]) & 3) << 6);
                    LSB_x = ((Convert.ToInt32(True_Image1[w + 128, h + 192, 0]) & 3) << 4);
                    LSB_y = ((Convert.ToInt32(True_Image1[w + 192, h + 128, 0]) & 3) << 2);
                    LSB_z = ((Convert.ToInt32(True_Image1[w + 192, h + 192, 0]) & 3));
                    Logo_Image2[w, h, 0] = LSB_w + LSB_x + LSB_y + LSB_z;
                }
            }

            for (int y = 0; y < LOGO_RE.Height; y++)
            {
                for (int x = 0; x < LOGO_RE.Height; x++)
                {
                    T_logo.SetPixel(x, y, Color.FromArgb(Logo_Image2[bk_x[i], bk_y[i], 0], Logo_Image2[bk_x[i], bk_y[i], 0], Logo_Image2[bk_x[i], bk_y[i], 0]));
                    i++;
                }
            }

            return T_logo;
        }////////////////////////////////////////////
        public Bitmap Output_Image(Bitmap Source)//輸出圖片
        {
            Bitmap O_Image = new Bitmap(Source.Width, Source.Height);
            Directory.CreateDirectory(@"Text");//建立文字暫存資料夾
            try
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "Bitmap Image|*.bmp";
                saveFileDialog1.Title = "儲存圖片";
                saveFileDialog1.ShowDialog();

                if (saveFileDialog1.FileName != "")
                {
                    System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();

                    switch (saveFileDialog1.FilterIndex)
                    {
                        case 1:
                            this.pictureBox3.Image.Save(fs, System.Drawing.Imaging.ImageFormat.Bmp);
                            break;
                    }
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return O_Image;
        }////////////////////////////////////////////////
        private double FindPSNR(Bitmap Souce_Image, Bitmap Souce_Image_re)//PSNR
        {
            long dist, tmp;
            double mse, psnr;

            long[, ,] image = new long[256, 256, 3];//原圖陣列
            long[, ,] re_image = new long[256, 256, 3];//原圖陣列

            for (int x = 0; x < Souce_Image.Width; x += 1) //取得圖像資料
            {
                for (int y = 0; y < Souce_Image.Height; y += 1)
                {
                    Color c1 = Souce_Image.GetPixel(x, y); // 得到 原始像素 的 Color
                    image[x, y, 0] = (int)(c1.R); image[x, y, 1] = (int)(c1.G); image[x, y, 2] = (int)(c1.B);
                    Color c2 = Souce_Image_re.GetPixel(x, y);
                    re_image[x, y, 0] = (int)(c2.R); re_image[x, y, 1] = (int)(c2.G); re_image[x, y, 2] = (int)(c2.B);
                }
            }

            dist = 0;
            for (int y = 0; y < Souce_Image.Width; y++)
            {
                for (int x = 0; x < Souce_Image.Height; x++)
                {
                    tmp = image[x, y, 0] - re_image[x, y, 0];
                    dist += tmp * tmp; // 注意這是，不是；
                }
            }

            mse = (double)dist / (Souce_Image.Height * Souce_Image.Width);
            psnr = Math.Log10(255 * 255 / mse) * 10;

            return psnr;

        }///////////////////////////
        private double FindBCR(Bitmap Logo_Image, Bitmap Logo_Image_re)
        {
            int[, ,] image = new int[256, 256, 3];//原圖陣列
            int[, ,] re_image = new int[256, 256, 3];//原圖陣列
            double BCR = 0;

            for (int x = 0; x < Logo_Image.Width; x += 1) //取得圖像資料
            {
                for (int y = 0; y < Logo_Image.Height; y += 1)
                {
                    Color c1 = Logo_Image.GetPixel(x, y); // 得到 原始像素 的 Color
                    image[x, y, 0] = (int)(c1.R); image[x, y, 1] = (int)(c1.G); image[x, y, 2] = (int)(c1.B);
                    Color c2 = Logo_Image_re.GetPixel(x, y);
                    re_image[x, y, 0] = (int)(c2.R); re_image[x, y, 1] = (int)(c2.G); re_image[x, y, 2] = (int)(c2.B);
                }
            }

            for (int y = 0; y < Logo_Image.Height; y++)
            {
                for (int x = 0; x < Logo_Image.Width; x++)
                {
                    BCR += (image[x, y, 0] ^ re_image[x, y, 0]);
                }
            }
            BCR = 100 - ((BCR / (Logo_Image.Width * Logo_Image.Height)));

            return BCR;
        }
        public int number_line(int x) //GOD
        {
            if (x < 0)
            {
                x = 0;
            }
            if (x > 255)
            {
                x = 255;
            }
            return x;
        }////////////////////////////////////////////////////////////////
        public int Gous(int x) //高斯
        {
            if (x % 2 == 0)
            {
                x = x / 2;
            }
            else
            {
                x = x / 2 - 1 / 2;
            }
            return x;
        }


    }
}
