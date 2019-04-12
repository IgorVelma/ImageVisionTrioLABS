using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab2
{
    public partial class Form1 : Form
    {
        Image file;
        Image file2;
        static string fname1, fname2;
        Bitmap img1, img2;
        int count1 = 0, count2 = 0;
        bool flag = true;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            progressBar1.Visible = false;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void распознатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            progressBar1.Visible = true;

            string img1_ref, img2_ref;
            img1 = (Bitmap)file;
            img2 = (Bitmap)file2;

            progressBar1.Maximum = img1.Width;
            if (img1.Width == img2.Width && img1.Height == img2.Height)
            {
                for (int i = 0; i < img1.Width; i++)
                {
                    for (int j = 0; j < img1.Height; j++)
                    {
                        img1_ref = img1.GetPixel(i, j).ToString();
                        img2_ref = img2.GetPixel(i, j).ToString();
                        if (img1_ref != img2_ref)
                        {
                            count2++;
                            
                        }
                        count1++;
                    }
                    progressBar1.Value++;
                }
                if (count2 == 0)
                {
                    MessageBox.Show("Изображения похожи на 100%");
                }
                else
                {
                    MessageBox.Show( count1 +" "+ count2 + "Изображения похожи на " + ((double)((count1 -count2) / (double)count1)) * 100 + "%");
                }
            }

            else
                MessageBox.Show("Не возможно сравнить");
            progressBar1.Visible = false;
            progressBar1.Value = 0;
            Application.Restart();
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog();
            f.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (f.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                file = Image.FromFile(f.FileName);
                pictureBox1.Image = file;
                fname1 = f.FileName.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int newWidth = 0;
            int newHeight = 0;
            Bitmap bmp = new Bitmap(file.Width, file.Height);
            textBox1.Text = (bmp.Height / 2).ToString();
            textBox2.Text = (bmp.Width / 2).ToString();
            groupBox2.Enabled = true;
        }

        private void открытьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog f1 = new OpenFileDialog();
            f1.Filter = "JPG(.*JPG)|*.jpg";
            if (f1.ShowDialog() == DialogResult.OK)
            {
                pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
                file2 = Image.FromFile(f1.FileName);
                pictureBox2.Image = file2;
                fname2 = f1.FileName.ToString();
            }

        }

        private void нормализироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            file = ContrastStretch((Bitmap)file, int.Parse(textBox1.Text), int.Parse(textBox2.Text));
            pictureBox3.Image = file;
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "JPG(.*JPG)|*.jpg";
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                file = MakeGrayscale3((Bitmap)file);
                file.Save(saveFile.FileName);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Image tmpImage = null;
            if (radioButton1.Checked)
            {
                tmpImage = ContrastStretch((Bitmap)file, int.Parse(textBox1.Text), int.Parse(textBox2.Text));
                pictureBox3.Image = tmpImage;
            }
            if (radioButton2.Checked)
            {
                tmpImage = ContrastStretch((Bitmap)file, int.Parse(textBox3.Text), int.Parse(textBox4.Text));
                pictureBox3.Image = tmpImage;
            }
            if (radioButton3.Checked)
            {
                tmpImage = ContrastStretch((Bitmap)file, int.Parse(textBox5.Text), int.Parse(textBox6.Text));
                pictureBox3.Image = tmpImage;
            }

            file = tmpImage;
          
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                file.Save(saveFile.FileName);
            }
        }

        private void рабочееИзображениеToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        public static Bitmap MakeGrayscale3(Bitmap original)
        {
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            Graphics g = Graphics.FromImage(newBitmap);

            //create the grayscale ColorMatrix
            ColorMatrix colorMatrix = new ColorMatrix(
               new float[][]
               {
         new float[] {.3f, .3f, .3f, 0, 0},
         new float[] {.59f, .59f, .59f, 0, 0},
         new float[] {.11f, .11f, .11f, 0, 0},
         new float[] {0, 0, 0, 1, 0},
         new float[] {0, 0, 0, 0, 1}
               });

            //create some image attributes
            ImageAttributes attributes = new ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);

            //draw the original image on the new image
            //using the grayscale color matrix
            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
               0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

            //dispose the Graphics object
            g.Dispose();
            return newBitmap;
        }
        public static Bitmap ContrastStretch(Bitmap srcImage, int height, int width, double blackPointPercent = 0.02, double whitePointPercent = 0.01)
        {
            BitmapData srcData = srcImage.LockBits(new Rectangle(0, 0, srcImage.Width, srcImage.Height), ImageLockMode.ReadWrite,
                PixelFormat.Format32bppArgb);
            Bitmap destImage = new Bitmap(srcImage.Width, srcImage.Height);
            BitmapData destData = destImage.LockBits(new Rectangle(0, 0, destImage.Width, destImage.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            int stride = srcData.Stride;
            IntPtr srcScan0 = srcData.Scan0;
            IntPtr destScan0 = destData.Scan0;
            var freq = new int[256];

            unsafe
            {
                byte* src = (byte*)srcScan0;
                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        ++freq[src[y * stride + x * 4]];
                    }
                }

                int numPixels = srcImage.Width * srcImage.Height;
                int minI = 0;
                var blackPixels = numPixels * blackPointPercent;
                int accum = 0;

                while (minI < 255)
                {
                    accum += freq[minI];
                    if (accum > blackPixels) break;
                    ++minI;
                }

                int maxI = 255;
                var whitePixels = numPixels * whitePointPercent;
                accum = 0;

                while (maxI > 0)
                {
                    accum += freq[maxI];
                    if (accum > whitePixels) break;
                    --maxI;
                }
                double spread = 255d / (maxI - minI);
                byte* dst = (byte*)destScan0;
                for (int y = 0; y < srcImage.Height; ++y)
                {
                    for (int x = 0; x < srcImage.Width; ++x)
                    {
                        int i = y * stride + x * 4;

                        byte val = (byte)Clamp(Math.Round((src[i] - minI) * spread), 0, 255);
                        dst[i] = val;
                        dst[i + 1] = val;
                        dst[i + 2] = val;
                        dst[i + 3] = 255;
                    }
                }
            }

            srcImage.UnlockBits(srcData);
            destImage.UnlockBits(destData);

            return destImage;
        }

        static double Clamp(double val, double min, double max)
        {
            return Math.Min(Math.Max(val, min), max);
        }
        /*
public static Bitmap Normalization(this Bitmap img)
        {
            int w = img.Width;
            int h = img.Height;

            BitmapData sd = img.LockBits(new Rectangle(0, 0, w, h),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int bytes = sd.Stride * sd.Height;
            byte[] buffer = new byte[bytes];
            byte[] result = new byte[bytes];
            Marshal.Copy(sd.Scan0, buffer, 0, bytes);
            img.UnlockBits(sd);
            int current = 0;
            byte max = 0;
            byte min = 255;
            for (int i = 0; i &lt; buffer.Length; i++)
            {
                max = Math.Max(max, buffer[i]);
                min = Math.Min(min, buffer[i]);
            }
            for (int y = 0; y &lt; h; y++)
            {
                for (int x = 0; x &lt; w; x++)
                {
                    current = y * sd.Stride + x * 4;
                    for (int i = 0; i &lt; 3; i++)
                    {
                        result[current + i] = (byte)((buffer[current + i] - min) * 100 / (max - min));
                    }
                    result[current + 3] = 255;
                }
            }
            Bitmap resimg = new Bitmap(w, h);
            BitmapData rd = resimg.LockBits(new Rectangle(0, 0, w, h),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(result, 0, rd.Scan0, bytes);
            resimg.UnlockBits(rd);
            return resimg;
        }*/
    }
}
