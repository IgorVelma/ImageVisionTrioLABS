using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;


namespace SecondLabaApp
{
    public partial class Form1 : Form
    {
        Image img;
        int height;
        int width;
        int tmpWidth;
        int tmpHeight;
        public Form1()
        {
            InitializeComponent();
            dataGridView1.Columns[0].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (dialog.ShowDialog() == DialogResult.OK)
            {

                Image image = null;
                Bitmap bitmap = new Bitmap(dialog.FileName);
                pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
                pictureBox1.Image = bitmap;
                Emgu.CV.Image<Gray, Byte> imageCV = new Emgu.CV.Image<Gray, byte>(bitmap);
                MCvMoments moments = imageCV.GetMoments(false);
                dataGridView1.Rows.Add(bitmap, getMoments(moments).ToString());
            }
        }
        private StringBuilder getMoments(MCvMoments moments)
        {
            var map = new Dictionary<string, double>();
            map.Add("\nm02-> ", moments.M02);
            map.Add("m11-> ", moments.M11);
            map.Add("m20-> ", moments.M20);
            map.Add("m30-> ", moments.M30);
            map.Add("m21-> ", moments.M21);
            map.Add("m03-> ", moments.M03);
            StringBuilder mmnts = new StringBuilder();
            foreach (KeyValuePair<string, double> pair in map)
            {
                mmnts.AppendLine(pair.Key + string.Format("{0:N3}", pair.Value));
                mmnts.Append(Environment.NewLine);
            }
            return mmnts;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                List<Bitmap> bitmaps = new List<Bitmap>();
                Bitmap etalon = new Bitmap(pictureBox2.Image);
                System.Diagnostics.Debug.Write(dataGridView1.RowCount);
                for (int i = 0; i < dataGridView1.RowCount - 1; ++i)
                {
                    bitmaps.Add((Bitmap)dataGridView1.Rows[i].Cells[0].Value);
                }
                bitmaps.Add(new Bitmap(etalon.Width, etalon.Height));
                StringBuilder builder = new StringBuilder();
                Console.Write(bitmaps.ToArray().Length);
                float[] simMas = CalculateEvklid(bitmaps.ToArray(), etalon);
                for(int i=0; i<simMas.Length-1; ++i)
                {
                    
                        builder.Append("p(0," + (i+1) + ") " + simMas[i] + Environment.NewLine);
                        builder.Append('\n');
                    
                }
                //int count = 0;
                //foreach (float f in simMas)
                //{
                //    builder.Append("p(0, " + (count + 1) + ") = " + f + Environment.NewLine);
                //    count++;
                //}
                simMas[simMas.Length - 1] = 0;
                textBox2.Text = builder.ToString();
                textBox3.Text = simMas.Max() + "(" + (simMas.ToList().IndexOf(simMas.Max()) + 1) + ")";
            }
            catch (Exception ex)
            {

            }

        }
        static float[] CalculateEvklid(Bitmap[] bitmaps, Bitmap etalon)
        {
            //for (int i = 0; i < bitmaps.Length; ++i)
            //{
            //    if (etalon.Size != bitmaps[i].Size)
            //    {
            //        return new float[] { 0 };
            //    }
            //}
            var rectangle = new Rectangle(0, 0, etalon.Width, etalon.Height);
            BitmapData[] bitmapDatas = new BitmapData[bitmaps.Length];
            BitmapData etalonData = etalon.LockBits(rectangle, ImageLockMode.ReadOnly, etalon.PixelFormat);
            for (int i = 0; i < bitmaps.Length; ++i)
            {
                bitmapDatas[i] = bitmaps[i].LockBits(rectangle, ImageLockMode.ReadOnly, bitmaps[i].PixelFormat);

            }
            float diff = 0;
            var byteCount = rectangle.Width * rectangle.Height * 3;
            float[] diffMass = new float[bitmaps.Length];
            unsafe
            {
                for (int i = 0; i < bitmapDatas.Length; ++i)
                {
                    byte* pointer1 = (byte*)etalonData.Scan0.ToPointer();
                    byte* pointer2 = (byte*)bitmapDatas[i].Scan0.ToPointer();

                    for (int x = 0; x < byteCount; x++)
                    {
                        diff += (float)Math.Abs(*pointer1 - *pointer2) / 255;
                        pointer1++;
                        pointer2++;
                    }
                    pointer1 = (byte*)0;
                    pointer2 = (byte*)0;
                    diffMass[i] = diff;
                    diff = 0;
                }
            }

            etalon.UnlockBits(etalonData);
            for (int i = 0; i < bitmaps.Length; ++i)
                bitmaps[i].UnlockBits(bitmapDatas[i]);

            return diffMass;
        }
        

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            try
            {
                Bitmap bitmap = new Bitmap(pictureBox2.Image, 50, 145);
                pictureBox3.Image = bitmap;
            }
            catch (Exception ex) { }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox4.Clear();
            StringBuilder builder = new StringBuilder();
            float[] simMas = null;
            float sims = 0;
            List<Bitmap> bitmaps = new List<Bitmap>();
            //Bitmap etalon = new Bitmap(pictureBox2.Image);
            System.Diagnostics.Debug.Write(dataGridView1.RowCount);
            for (int i = 0; i < dataGridView1.RowCount - 1; ++i)
            {
                bitmaps.Add((Bitmap)dataGridView1.Rows[i].Cells[0].Value);
            }
            for (int i = 1; i < bitmaps.Count; ++i)
            {
                    simMas = CalculateEvklid(bitmaps.ToArray(), new Bitmap(bitmaps.ElementAt(i).Width, bitmaps.ElementAt(i).Height));
            }
            //Bitmap defected = new Bitmap((Bitmap)pictureBox2.Image, etalon.Width, etalon.Height);

            //bitmaps.Add(defected);
            Console.Write(bitmaps.ToArray().Length);
            for (int i=1; i<simMas.Length; ++i)
            {
                if (simMas[0] == simMas[i])
                {
                    builder.Append("p(1," + (i+1) + ") " + "equals" + Environment.NewLine);
                    builder.Append('\n');
                }
                else
                {
                    builder.Append("p(1," + (i+1) + ") " + simMas[i] + Environment.NewLine);
                    builder.Append('\n');
                }
            }
            textBox4.Text = builder.ToString();
        }
        Image tmpIm;
        int counter = 0;
        private void добавитьЭталонToolStripMenuItem_Click(object sender, EventArgs e)
        {           
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                img = MakeGrayscale3(new Bitmap(dialog.FileName));
                height = img.Height;
                width = img.Width;
                
                try
                {
                    if (counter > 0)
                    {
                        if (width != ((Bitmap)dataGridView1.Rows[0].Cells[0].Value).Width || height != ((Bitmap)dataGridView1.Rows[0].Cells[0].Value).Height)
                        {
                            throw new Exception("Неверный размер");
                        }
                    }
                    pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
                    pictureBox1.Image = img;
                    Emgu.CV.Image<Gray, Byte> imageCV = new Emgu.CV.Image<Gray, byte>((Bitmap)img);
                    MCvMoments moments = imageCV.GetMoments(false);
                    dataGridView1.Rows.Add(img, getMoments(moments).ToString());
                    counter++;
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                
            }
        }

        private void загрузитьИзображениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                img = MakeGrayscale3(new Bitmap(dialog.FileName));
                try
                {
                    if (counter == 1)
                    {
                        if (img.Width != ((Bitmap)dataGridView1.Rows[0].Cells[0].Value).Width || img.Height!= ((Bitmap)dataGridView1.Rows[0].Cells[0].Value).Height)
                        {
                            throw new Exception("Неверный размер");
                        }
                    }
                    
                    pictureBox2.SizeMode = PictureBoxSizeMode.CenterImage;
                    pictureBox2.Image = img;
                    Emgu.CV.Image<Gray, Byte> imageCV = new Emgu.CV.Image<Gray, byte>((Bitmap)img);
                    MCvMoments moments = imageCV.GetMoments(false);
                    textBox1.Text = getMoments(moments).ToString();
                }catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        int angle;
        private void trB_Scroll(object sender, EventArgs e)
        {
            pictureBox2.Image = img;
            //pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.Image = Rotate(img);
            angle = trB.Value;
            Invalidate();
        }
        private Image Rotate(Image img)
        {
            pictureBox2.Image = null;
            pictureBox2.Invalidate();
            Bitmap bmp = new Bitmap(img.Width, img.Height);
            Graphics gfx = Graphics.FromImage(bmp);
            gfx.TranslateTransform(img.Width / 2, img.Height / 2);
            gfx.RotateTransform(angle);
            gfx.TranslateTransform(-img.Width / 2, -img.Height / 2);
            gfx.DrawImage(img, new Point(0, 0));
            gfx.Dispose();
            CalculateMoment(bmp);
            return bmp;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            double scale = 0;
            try
            {
                scale = double.Parse(textBox5.Text.Replace(',', '.'),
                 System.Globalization.CultureInfo.InvariantCulture);
                if (scale < 0.0)
                    MessageBox.Show("Введите корректно");
                else pictureBox2.Image = ImgResize(img, scale);
                pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox2.Image = img;
                pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                textBox2.Clear();
            }
            catch (Exception exp)
            { MessageBox.Show("Ошибка: " + exp.Message); }
            CalculateMoment(ImgResize(img, scale));
        }
        private Bitmap ImgResize(Image img, double percent)
        {
            Bitmap bmp = null;
            Size size = new Size(Convert.ToInt32(pictureBox2.Image.Width * percent),
            Convert.ToInt32(pictureBox2.Image.Height * percent));
            try
            {
                bmp = new Bitmap(size.Width, size.Height);
            }
            catch (Exception exp)
            { MessageBox.Show("Ошибка: " + exp.Message); }
            Graphics g = Graphics.FromImage(bmp);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(pictureBox2.Image, new Rectangle(new Point(0, 0), size),
            new Rectangle(new Point(0, 0), pictureBox2.Image.Size), GraphicsUnit.Pixel);
            g.Dispose();
            pictureBox2.Size = bmp.Size;
            return bmp;
        }

        private void CalculateMoment(Bitmap image)
        {
            Emgu.CV.Image<Gray, Byte> imageCV = new Emgu.CV.Image<Gray, byte>(image);
            MCvMoments moments = imageCV.GetMoments(false);
            textBox1.Text = getMoments(moments).ToString();
        }
        private void button1_Click_1(object sender, EventArgs e)
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

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }
    }
}
