using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Drawing;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Diagnostics;
using System.Xml.Linq;
using IronOcr;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Threading;
using BitMiracle.LibTiff.Classic;
using System.IO;

namespace ZangTFT
{
    public partial class Main : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        IntPtr LOLWindowHandle;
        Dictionary<string, Mat> champs = new Dictionary<string, Mat>();
        Mat empty = new Mat();

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                Application.Exit();
                return true;
            }
            else
                return base.ProcessDialogKey(keyData);
        }
        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            LOLWindowHandle = GetLOLWindow();
            if (LOLWindowHandle == IntPtr.Zero)
            {
                MessageBox.Show("Cannot find LOL Windows, exit now");
                Application.Exit();
            }


            Mat templ = new Mat();
            templ = CvInvoke.Imread(@"Champs\Kalista.png", ImreadModes.Unchanged);
            champs["Kalista"] = templ;
            templ = CvInvoke.Imread(@"Champs\Irelia.png", ImreadModes.Unchanged);
            champs["Irelia"] = templ;
            templ = CvInvoke.Imread(@"Champs\KaiSa.png", ImreadModes.Unchanged);
            champs["KaiSa"] = templ;
            templ = CvInvoke.Imread(@"Champs\Maokai.png", ImreadModes.Unchanged);
            champs["Maokai"] = templ;
            templ = CvInvoke.Imread(@"Champs\Samira.png", ImreadModes.Unchanged);
            champs["Samira"] = templ;
            templ = CvInvoke.Imread(@"Champs\Shen.png", ImreadModes.Unchanged);
            champs["Shen"] = templ;
            templ = CvInvoke.Imread(@"Champs\Yasuo.png", ImreadModes.Unchanged);
            champs["Yasuo"] = templ;
            templ = CvInvoke.Imread(@"Champs\Warwick.png", ImreadModes.Unchanged);
            champs["Warwick"] = templ;

            empty = CvInvoke.Imread("empty.png", ImreadModes.Unchanged);
        }

        public bool LOLIsActivated()
        {
            var activatedHandle = GetForegroundWindow();
            if (activatedHandle == IntPtr.Zero)
            {
                return false;       // No window is currently activated
            }

            return activatedHandle == LOLWindowHandle;
        }

        public bool IsEqualImage(Mat image, Mat templ)
        {
            // Compare 2 images
            Mat result = new Mat();
            CvInvoke.MatchTemplate(image, templ, result, TemplateMatchingType.CcoeffNormed);
            double[] minValues, maxValues;
            System.Drawing.Point[] minLocations, maxLocations;
            result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);

            // If the match is good enough
            return maxValues[0] > 0.9;
        }

        public void OpenCVClick()
        {
            string imageName = "Screenshot.png";
            int w = 1015;
            int h = 35;
            int left = 472;
            int top = 1042;
            do
            {
                Thread.Sleep(20);

                // Check if slot is avaiable
                var slot = new Bitmap(160, 160, PixelFormat.Format32bppArgb);
                using (Graphics graphicsSlot = Graphics.FromImage(slot))
                {
                    graphicsSlot.CopyFromScreen(1305, 705, 0, 0, slot.Size, CopyPixelOperation.SourceCopy);
                    slot.Save("slot.png", ImageFormat.Png);
                    Mat slotMat = CvInvoke.Imread("slot.png", ImreadModes.Unchanged);
                    if (!IsEqualImage(slotMat, empty))
                    {
                        continue;
                    }
                }


                var bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb);
                using (Graphics graphics = Graphics.FromImage(bmp))
                {
                    graphics.CopyFromScreen(left, top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);
                    bmp.Save(imageName, ImageFormat.Png);
                    Mat img = CvInvoke.Imread(imageName, ImreadModes.Unchanged);

                    foreach (var champ in champs)
                    {
                        FindAndClick(champ.Key, img, top, left);
                    }
                }
            } while (true);
        }

        public void FindAndClick(string templName, Mat img, int topStartPosition, int leftStartPosition)
        {
            Mat templ = champs[templName];
            Random rnd = new Random();

            Mat result = new Mat();

            // Find the match
            CvInvoke.MatchTemplate(img, templ, result, TemplateMatchingType.CcoeffNormed);
            double[] minValues, maxValues;
            System.Drawing.Point[] minLocations, maxLocations;
            result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);

            // If the match is good enough
            if (maxValues[0] > 0.9)
            {
                // Click on the match
                int x = maxLocations[0].X + templ.Width / 2 + leftStartPosition + rnd.Next(-templ.Width / 2, templ.Width / 2);
                int y = maxLocations[0].Y + templ.Height / 2 + topStartPosition - rnd.Next(0, 50);
                KeyMouseHelper.ClickOnPoint(new System.Drawing.Point(x, y));
                Thread.Sleep(50);
            }
        }

        public void AutoClick()
        {
            // List of champions
            List<string> champions = new List<string>();
            /*
            champions.Add("Kalista");
            champions.Add("Irelia");
            champions.Add("Maokai");
            champions.Add("Samira");
            champions.Add("Warwick");
            champions.Add("Kaisa");
            champions.Add("Shen");
            champions.Add("Yasuo");
            champions.Add("Aatrox");
            */
            champions.Add("Poppy");
            champions.Add("Peppy");
            champions.Add("Tristana");
            champions.Add("Mackai");
            champions.Add("Maokai");
            champions.Add("Viego");
            champions.Add("Vieeo");
            champions.Add("Jinx");
            champions.Add("Jayce");
            champions.Add("Zeri");
            champions.Add("Heimerdinger");

            Random rnd = new Random();
            string imageName = "Screenshot\\LOL_Screenshot"+rnd.Next()+".png";
            IronTesseract ocr = new IronTesseract();
            ocr.Configuration.BlackListCharacters = "~`$#^*_}{][|\\";
            ocr.Configuration.PageSegmentationMode = TesseractPageSegmentationMode.SingleWord;

            int w = 1015;
            int h = 35;
            int left = 472;
            int top = 1042;
            var bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb);

            do
            {
                imageName = "Screenshot\\LOL_Screenshot" + rnd.Next() + ".png";
                using (Graphics graphics = Graphics.FromImage(bmp))
                {
                    graphics.CopyFromScreen(left, top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);
                }

                bmp.Save(imageName, ImageFormat.Png);

                for (int i = 0; i < 5; i++)
                {
                    using (OcrInput input = new OcrInput())
                    {
                        // Dimensions are in pixel
                        var contentArea = new System.Drawing.Rectangle() { X = w / 5 * i, Y = 0, Height = h, Width = w / 5 };
                        input.Add(imageName, contentArea);
                        OcrResult result = ocr.Read(input);
                        Console.WriteLine(result.Text);

                        // If text in result contains any champion name, click on that champion
                        foreach (string champ in champions)
                        {
                            if (result.Text.Contains(champ))
                            {
                                int rdX = rnd.Next(20, w / 5 - 20);
                                int rdY = rnd.Next(10, 40);
                                KeyMouseHelper.ClickOnPoint(new System.Drawing.Point(left + w / 5 * i + rdX, top - rdY));
                                Thread.Sleep(50);
                            }
                        }
                    }
                }
                Thread.Sleep(1);
            } while (true);
        }

        public void GenerateChamps()
        {
            // Read all image in folder named "Screenshot"
            string[] files = Directory.GetFiles("Screenshot");
            foreach (string file in files)
            {
                // Crop image into 5 parts then crop first 50px
                Bitmap bmp = new Bitmap(file);
                int w = bmp.Width / 5;
                int h = bmp.Height;
                int left = 0;
                int top = 0;

                for (int i = 0; i < 5; i++)
                {
                    Bitmap crop = bmp.Clone(new System.Drawing.Rectangle(left + 9, top + 3, 80, 17), bmp.PixelFormat);
                    // Save crop to temp file
                    crop.Save("temp.png", ImageFormat.Png);

                    // Read text on image
                    IronTesseract ocr = new IronTesseract();
                    ocr.Configuration.BlackListCharacters = "~`$#^*_}{][|\\";
                    ocr.Configuration.PageSegmentationMode = TesseractPageSegmentationMode.SingleWord;
                    OcrResult result = ocr.Read("temp.png");
                    Console.WriteLine(result.Text);

                    // Save image
                    crop.Save("Champs\\" + RemoveSpecialCharacters(result.Text) + ".png", ImageFormat.Png);
                    left += w;
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            // KeyMouseHelper.ClickOnPoint(LOLWindowHandle, new Point(900, 670));
            //string Text = new IronTesseract().Read(@"FFF.png").Text;
            //MessageBox.Show(Text);
            
            // Create new thread run AutoClick
            Thread thread = new Thread(OpenCVClick);
            thread.Start();
        }

        private IntPtr GetLOLWindow()
        {
            IntPtr hWnd = IntPtr.Zero;
            foreach (Process pList in Process.GetProcesses())
            {
                if (pList.MainWindowTitle.Contains("League of Legends"))
                {
                    hWnd = pList.MainWindowHandle;
                }
            }
            return hWnd;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Read image named LOL_Screenshot1041739629
            Mat img = CvInvoke.Imread("Screenshot\\LOL_Screenshot1041739629.png", ImreadModes.Unchanged);
            FindAndClick("KaiSa", img, 0, 0);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            GenerateChamps();
        }

        public string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}
