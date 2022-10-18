using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Formats.Asn1.AsnWriter;
using static System.Net.Mime.MediaTypeNames;
using Color = System.Drawing.Color;
using Point = System.Windows.Point;

namespace Podejscie2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public Bitmap bitmap;

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {

            int value;

            string Type = "";

            int WidthSize = 0, HeightSize = 0, ColourSize = 0;


            int R = -2, G = -2, B = -2;


            BitmapImage bitmapImage = new();


            OpenFileDialog dialog = new()
            {
                Filter = "PPM File | *.ppm"
            };

            bool? result = dialog.ShowDialog();

            if (result is not true) return;
            string fileName = dialog.FileName;

            int CurrentX = 0, CurrentY = 0;

            int Counter = 0;

            FileStream fs = File.Open(fileName, FileMode.Open, FileAccess.Read);

            using (BinaryReader reader = new(fs))
            {
                StringBuilder stringBuilder = new StringBuilder();
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {

                    if (Type == "")
                    {
                        StringBuilder sb = new();
                        sb.Append(Convert.ToChar(reader.ReadByte()));
                        sb.Append(Convert.ToChar(reader.ReadByte()));
                        Type = sb.ToString();
                    }

                    //Trace.WriteLine(currentChar + " " +(int)currentChar);


                    if (Type == "P3")
                    {
                        char currentChar = Convert.ToChar(reader.ReadByte());

                        if (((int)currentChar >= 33 && ((int)currentChar <35)) || (((int)currentChar >= 36 && ((int)currentChar <= 47))) ||  ((int)currentChar >= 58))
                        {
                            continue;
                        }

                        //whitespaces 10, 32, 9

                        if (currentChar == '#')
                        {
                            while (currentChar != '\n')
                            {
                                currentChar = (char)reader.ReadByte();
                            }
                            continue;

                        }


                        //ARE NUMBERS
                        if ((int)currentChar >= 48 && ((int)currentChar <= 57))
                        {
                            stringBuilder.Append(currentChar);

                            //NEXT NOT NUMBER
                            if ((int)Convert.ToChar(reader.PeekChar()) == 10 || (int)Convert.ToChar(reader.PeekChar()) == 32 || (int)Convert.ToChar(reader.PeekChar()) == 9)
                            {
                                if (WidthSize == 0)
                                {
                                    WidthSize = Convert.ToInt32(stringBuilder.ToString());
                                    stringBuilder = new();
                                    continue;
                                }
                                if (HeightSize == 0)
                                {
                                    HeightSize = Convert.ToInt32(stringBuilder.ToString());
                                    stringBuilder = new();
                                    bitmap = new(WidthSize, HeightSize);
                                    continue;
                                }
                                if (ColourSize==0)
                                {
                                    ColourSize = Convert.ToInt32(stringBuilder.ToString());
                                    stringBuilder = new();
                                    continue;
                                }

                                if (R == -2)
                                {
                                    R = Convert.ToInt32(stringBuilder.ToString());
                                    stringBuilder = new();
                                    continue;
                                }
                                if (G == -2)
                                {
                                    G = Convert.ToInt32(stringBuilder.ToString());
                                    stringBuilder = new();
                                    continue;
                                }
                                if (B == -2)
                                {
                                    B = Convert.ToInt32(stringBuilder.ToString());
                                    stringBuilder = new();
                                    //continue;
                                }

                                if(ColourSize == 255)
                                    bitmap.SetPixel(CurrentX, CurrentY, Color.FromArgb(R, G, B));
                                else
                                    bitmap.SetPixel(CurrentX, CurrentY, Color.FromArgb(R>>8, G>>8, B >> 8));

                                CurrentX++;

                                R =G=B=-2;

                                if (CurrentX >= WidthSize)
                                {
                                    CurrentX = 0;
                                    CurrentY++;
                                }
                            }
                        }
                    }
                    else if (Type == "P6")
                    {
                        char currentChar = Convert.ToChar(reader.ReadByte());

                        if (((int)currentChar >= 33 && ((int)currentChar <35)) || (((int)currentChar >= 36 && ((int)currentChar <= 47))) ||  ((int)currentChar >= 58))
                        {
                            continue;
                        }

                        //whitespaces 10, 32, 9

                        if (currentChar == '#')
                        {
                            while (currentChar != '\n')
                            {
                                currentChar = (char)reader.ReadByte();
                            }
                            continue;

                        }

                        if ((int)currentChar >= 48 && ((int)currentChar <= 57))
                        {
                            stringBuilder.Append(currentChar);

                            //NEXT NOT NUMBER
                            
                            if ((int)Convert.ToChar(reader.PeekChar()) == 10 || (int)Convert.ToChar(reader.PeekChar()) == 32 || (int)Convert.ToChar(reader.PeekChar()) == 9)
                            {
                                if (WidthSize == 0)
                                {
                                    WidthSize = Convert.ToInt32(stringBuilder.ToString());
                                    stringBuilder = new();
                                    continue;
                                }
                                if (HeightSize == 0)
                                {
                                    HeightSize = Convert.ToInt32(stringBuilder.ToString());
                                    stringBuilder = new();
                                    bitmap = new(WidthSize, HeightSize);
                                    continue;
                                }
                                if (ColourSize==0)
                                {
                                    ColourSize = Convert.ToInt32(stringBuilder.ToString());
                                    stringBuilder = new();
                                    reader.ReadByte();
                                    for (int i = 0; i < HeightSize; i++)
                                    {
                                        for (int j = 0; j < WidthSize; j++)
                                        {
                                            int red = Convert.ToInt32(reader.ReadByte());

                                            int green = Convert.ToInt32(reader.ReadByte());

                                            int blue = Convert.ToInt32(reader.ReadByte());

                                            if (ColourSize == 255)
                                                bitmap.SetPixel(j, i, Color.FromArgb(red, green, blue));
                                            else
                                                bitmap.SetPixel(j, i, Color.FromArgb(red>>8, G>>green, B >> blue));
                                           // bitmap.SetPixel(j, i, Color.FromArgb(red, green, blue));
                                        }
                                    }
                                }
                            }
                        }

                    }

                }
            }

            bitmapImage = ToBitmapImage(bitmap);
            MyImage.Source = bitmapImage;
        }

        public BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }
        double ScaleX = 1.1;
        double ScaleY = 1.1;
        private void ZoomInButtonClick(object sender, MouseButtonEventArgs e)
        {
             ScaleX += 0.5;
             ScaleY += 0.5;
             MyStackPanel1.LayoutTransform = new ScaleTransform(ScaleX, ScaleY);
        }

        private void ZoomOutButtonClick(object sender, MouseButtonEventArgs e)
        {
            ScaleX -= 0.5;
            ScaleY -= 0.5;
            if (ScaleX <=0)
                ScaleX = .1;

            if (ScaleY <=0)
                ScaleY = .1;

            MyStackPanel1.LayoutTransform = new ScaleTransform(ScaleX, ScaleY);
        }

        private void LoadJpeg_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new()
            {
                Filter = "JPEG File | *.jpg"
            };

            bool? result = dialog.ShowDialog();

            if (result is not true) return;
            string fileName = dialog.FileName;

            ImageSource imageSource = new BitmapImage(new Uri(fileName));
            MyImage.Source = imageSource;
        }

        private void SaveJpeg_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog fileDialog = new()
            {
                Filter = "JPEG File | *.jpg"
            };

            bool? result = fileDialog.ShowDialog();

            if (result is not true)
                return;

            var fileName = fileDialog.FileName;

            const double dpi = 96d;
            var bounds = VisualTreeHelper.GetDescendantBounds(MyImage);
            //Trace.WriteLine(MainWindow1.Width);
            

            RenderTargetBitmap bitmap = new(
                (int)bounds.Width,
                (int)bounds.Height,
                dpi,
                dpi,
                PixelFormats.Default);

            bitmap.Render(MyImage);

            JpegBitmapEncoder encoder = new();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.QualityLevel = int.Parse(Compression.Value.ToString());

            using var fileStream = File.Create(fileName);
            encoder.Save(fileStream);
        }

        private void ImageMouseMove(object sender, MouseEventArgs e)
        {
            Point point = Mouse.GetPosition(MyImage);

            var renderTargetBitmap = new RenderTargetBitmap((int)MyImage.ActualWidth,
                (int)MyImage.ActualHeight,
                96, 96, PixelFormats.Default);
            renderTargetBitmap.Render(MyImage);

            if ((point.X <= renderTargetBitmap.PixelWidth) && (point.Y <= renderTargetBitmap.PixelHeight))
            {
                CroppedBitmap croppedBitmap;
                if ((int)point.X >1 && (int)point.Y >1)
                {
                    croppedBitmap = new CroppedBitmap(renderTargetBitmap,
                    new Int32Rect((int)point.X, (int)point.Y, 1, 1));
                    var pixels = new byte[4];
                    croppedBitmap.CopyPixels(pixels, 4, 0);
                    ValueBlock.Text = "Red Value: " + pixels[2] + "\tGreen Value: " + pixels[1] + " \tBlue Value: " + pixels[0];
                }
            }
        }
    }
}