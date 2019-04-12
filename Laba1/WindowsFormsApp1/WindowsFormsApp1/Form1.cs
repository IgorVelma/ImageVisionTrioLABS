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
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private string type;
        private SolidBrush brush;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            var watch1 = System.Diagnostics.Stopwatch.StartNew();
            generateImageWithPixels();
            watch1.Stop();
            var result1 = watch1.Elapsed;
            var watch2 = System.Diagnostics.Stopwatch.StartNew();
            generateImageWithPointer();
            watch2.Stop();
            var result2 = watch2.Elapsed;
            label1.Text = "Time:\t" + result1 + string.Empty;
            label2.Text = "Time:\t" + result2 + string.Empty;
        }
        private void generateImageWithPixels()
        {
            int i, j;
            int amplitude, gray;
            float period;
            Color rgb;
            Bitmap bitmap = new Bitmap(256, 256, PixelFormat.Format24bppRgb);
            for (i = 0; i < 256; ++i)
                for (j = 0; j < 256; ++j)
                {
                    amplitude = 64 * (255 - i) / 255;
                    period = (float)(100 * Math.Sqrt(1
                        / (1 + Math.Exp(0.013 * j)
                        * Math.Exp(0.027 * j) / 400)));
                    gray = (int)(amplitude * Math.Sin(2 * Math.PI / period * j) + 128);
                    rgb = Color.FromArgb(gray, gray, gray);
                    bitmap.SetPixel(j, i, rgb);
                }
            i = bitmap.Height;
            j = bitmap.Width;
            pictureBox1.Image = bitmap;

        }
        unsafe private void generateImageWithPointer()
        {
            Bitmap bitmap = new Bitmap(pictureBox1.Image);
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height)
                , ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            byte* imagePointer = (byte*)data.Scan0;
            for (int i = 0; i < bitmap.Height; ++i)
            {
                for (int j = 0; j < bitmap.Width; ++j)
                {
                    byte rCan, gCan, bCan, greyCan;
                    bCan = imagePointer[data.Stride * i + j * 4];
                    gCan = imagePointer[data.Stride * i + j * 4 + 1];
                    rCan = imagePointer[data.Stride * i + j * 4 + 2];
                    greyCan = (byte)((bCan + gCan + rCan) / 3);

                    imagePointer[data.Stride * i + j * 4] = gCan;
                    imagePointer[data.Stride * i + j * 4 + 1] = gCan;
                    imagePointer[data.Stride * i + j * 4 + 2] = gCan;

                }
            }
            bitmap.UnlockBits(data);
            pictureBox2.Image = bitmap;
        }


        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
                pictureBox4.Image = new Bitmap(dialog.FileName);
                pictureBox4.Tag = dialog.FileName;

            }
        }
        private Bitmap getImageGrey(string path)
        {
            Bitmap c = new Bitmap(path);
            for (int i = 0; i < c.Width; i++)
            {
                for (int x = 0; x < c.Height; x++)
                {
                    Color oc = c.GetPixel(i, x);
                    int grayScale = (int)((oc.R * 0.3) + (oc.G * 0.59) + (oc.B * 0.11));
                    Color color = Color.FromArgb(oc.A, grayScale, grayScale, grayScale);
                    c.SetPixel(i, x, color);
                }
            }
            return c;

        }
        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                pictureBox5.SizeMode = PictureBoxSizeMode.CenterImage;
                pictureBox5.Image = getImageGrey(pictureBox4.Tag + string.Empty);

            }
            catch (Exception ex) { }
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox6.Tag = dialog.FileName + string.Empty;
                pictureBox6.Image = getInvertation(pictureBox6.Tag + string.Empty);


            }

        }
        private Bitmap getInvertation(string path)
        {
            Bitmap pic = getImageGrey(path);
            for (int y = 0; (y <= (pic.Height - 1)); y++)
            {
                for (int x = 0; (x <= (pic.Width - 1)); x++)
                {
                    Color inv = pic.GetPixel(x, y);
                    inv = Color.FromArgb(255, (255 - inv.R), (255 - inv.G), (255 - inv.B));
                    pic.SetPixel(x, y, inv);
                }
            }
            return pic;


        }
        private void pictureBox6_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string path = dialog.FileName + string.Empty;
                if (textBox2.Text != "")
                {
                    int threshold = Int32.Parse(textBox2.Text);
                    pictureBox7.Image = getSegmentation(path, threshold);
                }

                else
                {
                    textBox2.Text = "!!!!Enter threshold!!!!!";

                }
            }

        }
        private Bitmap getSegmentation(string path, int threshold)
        {
            Bitmap bitmap = getImageGrey(path);
            Color white = Color.FromArgb(255, 255, 255, 255);
            Color black = Color.FromArgb(255, 0, 0, 0);
            int td = threshold;
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color c = bitmap.GetPixel(x, y);

                    int value = (int)(c.R + c.G + c.B) / 3;
                    if (value < td)
                    {
                        bitmap.SetPixel(x, y, black);
                    }
                    else
                    {
                        bitmap.SetPixel(x, y, white);
                    }

                }
            }
            return bitmap;
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            drawOnPanel();

            drawOnPictureBox();
        }
        private void drawOnPanel()
        {
            panel1.Refresh();
            if (textBox3.Text != "" | textBox4.Text != "" | textBox5.Text != "")
            {
                int r = Int32.Parse(textBox3.Text);
                int g = Int32.Parse(textBox4.Text);
                int b = Int32.Parse(textBox5.Text);

                type = "panel";
                brush = new SolidBrush(Color.FromArgb(255, r, g, b));
                panel1.Invalidate();

            }
        }

        private void drawOnPictureBox()
        {
            if (textBox3.Text != "" | textBox4.Text != "" | textBox5.Text != "")
            {
                int r = Int32.Parse(textBox3.Text);
                int g = Int32.Parse(textBox4.Text);
                int b = Int32.Parse(textBox5.Text);
                SolidBrush brush = new SolidBrush(Color.FromArgb(255, r, g, b));
                Bitmap bmp = new Bitmap(150, 150);
                pictureBox8.Image = bmp;
                Graphics gr = Graphics.FromImage(pictureBox8.Image);
                gr.Clear(Color.FromArgb(255, 255, 255, 255));
                gr.FillRectangle(brush, 0, 0, 200, 150);
                gr.Dispose();
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (type == "panel")
            {
                e.Graphics.FillRectangle(brush, 0, 0, 150, 150);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (dialog.ShowDialog() == DialogResult.OK) {
                Bitmap bitmap= new Bitmap(dialog.FileName);
                pictureBox9.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
                pictureBox9.Image = bitmap;
                pictureBox9.Tag = dialog.FileName + string.Empty;
                textBox6.Text = dialog.FileName;
                textBox8.Text = Convert.ToString(bitmap.Width);
                textBox7.Text = Convert.ToString(bitmap.Height);
                textBox9.Text = Convert.ToString(bitmap.PixelFormat);

            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            OpenFileDialog saveFileDialog1 = new OpenFileDialog();
            saveFileDialog1.Title = "Open file";
            saveFileDialog1.Filter = "bmp files =(*.bmp)|*.bmp|jpg files (*.jpg)|*.jpg|All files (*.*)|*.*";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox9.Image.Save(saveFileDialog1.FileName);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Bitmap bm=null;
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                bm = new Bitmap(getImageGrey(dialog.FileName));
            }
            
            Dictionary<int, int> histoG = new Dictionary<int, int>();

            for (int x = 0; x < bm.Width; x++)
            {
                for (int y = 0; y < bm.Height; y++)
                {
                    Color c = bm.GetPixel(x, y);
                    var grayscale = (int)(c.R + c.G + c.B) / 3;
                    if (histoG.ContainsKey(grayscale))
                        histoG[grayscale] = histoG[grayscale] + 1;
                    else
                        histoG.Add(grayscale, 1);

                }
            }

            Series graySeries = Grayscale.Series.FindByName("Grayscale");

            String histoValues = "";

            foreach (var h in histoG.OrderBy(key => key.Key))
            {
                graySeries.Points.AddXY(h.Key, h.Value);
                histoValues += ("Key: " + h.Key + "; Value " + h.Value + System.Environment.NewLine);
            }

            textBox1.Text = histoValues;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Bitmap bm = null;
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                bm = new Bitmap(getImageGrey(dialog.FileName));
            }
            Dictionary<byte, int> dictR = new Dictionary<byte, int>();
            Dictionary<byte, int> dictG = new Dictionary<byte, int>();
            Dictionary<byte, int> dictB = new Dictionary<byte, int>();
            for (int x = 0; x < bm.Width; x++)
            {
                for (int y = 0; y < bm.Height; y++)
                {
                    Color c = bm.GetPixel(x, y);
                    if (dictR.ContainsKey(c.R))
                        dictR[c.R] = dictR[c.R] + 1;
                    else
                        dictR.Add(c.R, 1);
                    if (dictG.ContainsKey(c.G))
                        dictG[c.G] = dictG[c.G] + 1;
                    else
                        dictG.Add(c.G, 1);
                    if (dictB.ContainsKey(c.B))
                        dictB[c.B] = dictB[c.B] + 1;
                    else
                        dictB.Add(c.B, 1);
                }
            }

            Series redSeries = chart1.Series.FindByName("Red");
            Series greenSeries = chart1.Series.FindByName("Green");
            Series blueSeries = chart1.Series.FindByName("Blue");
            foreach (var h in dictR)
            {
                redSeries.Points.AddXY(h.Key, h.Value);
            }
            foreach (var h in dictG)
            {
                greenSeries.Points.AddXY(h.Key, h.Value);
            }
            foreach (var h in dictB)
            {
                blueSeries.Points.AddXY(h.Key, h.Value);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

