using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ZIKS_Lab4_Test_
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void buttonOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Image Files (*.png, *.jpg) | *.png; *.jpg";
            openDialog.InitialDirectory = @"D:\Зображення";

            if (openDialog.ShowDialog() == true)
            {
                textBoxFilePath.Text = openDialog.FileName.ToString();
                pictureBox1.Source = new BitmapImage(new Uri(openDialog.FileName));
            }
        }

        private void buttonEncode_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage img = new BitmapImage(new Uri(textBoxFilePath.Text));

            int msgLength = textBoxMessage.Text.Length;
            byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(textBoxMessage.Text);

            int stride = (img.PixelWidth * img.Format.BitsPerPixel + 7) / 8;
            byte[] pixels = new byte[img.PixelHeight * stride];
            img.CopyPixels(pixels, stride, 0);

            int lastPixelIndex = (img.PixelWidth * img.PixelHeight - 1);

            for (int i = 0; i < messageBytes.Length; i++)
            {
                pixels[i * 4 + 2] = messageBytes[i];
            }

            pixels[lastPixelIndex * 4 + 2] = (byte)msgLength;

            WriteableBitmap writeableBitmap = new WriteableBitmap(img);
            writeableBitmap.WritePixels(new Int32Rect(0, 0, img.PixelWidth, img.PixelHeight), pixels, stride, 0);

            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Image Files (*.png, *.jpg) | *.png; *.jpg";
            saveFile.InitialDirectory = @"D:\Зображення";

            if (saveFile.ShowDialog() == true)
            {
                using (FileStream fs = new FileStream(saveFile.FileName, FileMode.Create))
                {
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(writeableBitmap));
                    encoder.Save(fs);
                }
            }
        }

        private void buttonDecode_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage img = new BitmapImage(new Uri(textBoxFilePath.Text));

            int stride = (img.PixelWidth * img.Format.BitsPerPixel + 7) / 8;
            byte[] pixels = new byte[img.PixelHeight * stride];
            img.CopyPixels(pixels, stride, 0);

            int lastPixelIndex = (img.PixelWidth * img.PixelHeight - 1);
            int msgLength = pixels[lastPixelIndex * 4 + 2];

            byte[] messageBytes = new byte[msgLength];
            for (int i = 0; i < msgLength; i++)
            {
                messageBytes[i] = pixels[i * 4 + 2];
            }

            string message = System.Text.Encoding.UTF8.GetString(messageBytes);
            textBoxMessage.Text = message;
        }
    }
}
