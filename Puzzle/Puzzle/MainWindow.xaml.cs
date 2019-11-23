using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Puzzle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        String imageSource;
        DispatcherTimer dt = new DispatcherTimer();
        Stopwatch sw = new Stopwatch();
        string strCurTime = string.Empty;
        TimeSpan gameTime = new TimeSpan(0, 3, 0);
        int sizeBoard = 3;
        bool isPlaying = false;
        int widthImage, heightImage;
        int widthCropImage, heightCropImage;
        Image[,] images;
        public MainWindow()
        {
            InitializeComponent();


        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            dt.Tick += tick_Handle;
            dt.Interval = TimeSpan.FromSeconds(1);

        }

        private void tick_Handle(object sender, EventArgs e)
        {
            if (sw.IsRunning)
            {
                TimeSpan curTime = sw.Elapsed;
                strCurTime = String.Format($"{curTime.Minutes:00}:{curTime.Seconds:00}");
                stopWatchTextBlock.Text = strCurTime;
                if (curTime.Minutes == gameTime.Minutes && curTime.Seconds == gameTime.Seconds)
                {
                    sw.Stop();
                    dt.Stop();
                    isPlaying = false;
                    MessageBox.Show("Game over!");
                }
            }
        }

        BitmapImage bitmap = new BitmapImage(new Uri("sample.jpg", UriKind.Relative));

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            GameConfigDialog dialog = new GameConfigDialog();

            if (dialog.ShowDialog() == true)
            {
                imageSource = dialog.ImagePath;
                sizeBoard = dialog.Size;
                gameTime = dialog.GameTime;

            }
            else
            {
                return;
            }
            bitmap = new BitmapImage(new Uri(imageSource, UriKind.Absolute));
            sampleImage.Source = bitmap;
            images = new Image[sizeBoard, sizeBoard];
            widthImage = (int)canvas.ActualWidth / sizeBoard;
            heightImage = (int)canvas.ActualHeight / sizeBoard;
            widthCropImage = (int)bitmap.Width / sizeBoard;
            heightCropImage = (int)bitmap.Height / sizeBoard;
            stopWatchTextBlock.Text = "00:00";
            sw.Reset();
            canvas.Children.Clear();
            cropImage();
            shuffleImage();
            isPlaying = true;
            dt.Start();
            sw.Start();

        }

        bool isDragging = false;
        Image selectedImage = new Image();
        int startI, startJ;
        Point startPoint, lastPoint;
        private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isPlaying == false) return;

            startPoint = e.GetPosition(canvas);
            isDragging = true;
            startI = (int)startPoint.X / widthImage;
            startJ = (int)startPoint.Y / heightImage;

            selectedImage = images[(int)startPoint.X / widthImage, (int)startPoint.Y / heightImage];
        }

        private void canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isPlaying == false) return;

            lastPoint = e.GetPosition(canvas);
            isDragging = false;

            if (lastPoint.X > canvas.ActualWidth || lastPoint.Y > canvas.ActualHeight || lastPoint.X <= 0 || lastPoint.Y <= 0) return;

            int endI, endJ;


            endI = (int)lastPoint.X / widthImage;
            endJ = (int)lastPoint.Y / heightImage;

            if (Math.Abs(startI - endI) + Math.Abs(startJ - endJ) != 1)
            {
                Canvas.SetLeft(selectedImage, startI * widthImage);
                Canvas.SetTop(selectedImage, startJ * heightImage);
                return;
            }

            swapImage(startI, startJ, endI, endJ);

            images[startI, startJ] = images[endI, endJ];
            images[endI, endJ] = selectedImage;
            if (checkWin() == true)
            {
                isPlaying = false;
                sw.Stop();
                dt.Stop();
                MessageBox.Show("You won!");
            }



        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            //if (isPlaying == false) return;

            Point position = e.GetPosition(canvas);

            this.Title = $"{position.X} - {position.Y}";

            if (isDragging == true)
            {
                Canvas.SetLeft(selectedImage, e.GetPosition(canvas).X);
                Canvas.SetTop(selectedImage, e.GetPosition(canvas).Y);
                Canvas.SetZIndex(selectedImage, 1);
                if (position.X > canvas.ActualWidth || position.Y > canvas.ActualHeight || position.X <= 0 || position.Y <= 0 || isPlaying == false)
                {
                    isDragging = false;
                    Canvas.SetLeft(selectedImage, startI * widthImage);
                    Canvas.SetTop(selectedImage, startJ * heightImage);
                    return;
                }
            }
        }



        Image image = new Image();
        private void cropImage()
        {
            Canvas.SetZIndex(canvas, -1);
            for (int i = 0; i < sizeBoard; i++)
            {
                for (int j = 0; j < sizeBoard; j++)
                {
                    Int32Rect rect = new Int32Rect(i * widthCropImage, j * heightCropImage, widthCropImage, heightCropImage);
                    CroppedBitmap cropBitmap = new CroppedBitmap(bitmap, rect);

                    var cropImage = new Image();
                    int margin = 5;
                    cropImage.Stretch = Stretch.Fill;
                    cropImage.Width = widthImage - 2 * margin;
                    cropImage.Height = heightImage - 2 * margin;
                    cropImage.Margin = new Thickness(margin);
                    cropImage.Tag = (int)(i * sizeBoard + j);

                    cropImage.Source = cropBitmap;
                    images[i, j] = cropImage;
                    canvas.Children.Add(cropImage);

                    Canvas.SetLeft(cropImage, i * (widthImage));
                    Canvas.SetTop(cropImage, j * (heightImage));
                }
            }
        }

        private bool checkWin()
        {
            for (int i = 0; i < sizeBoard; i++)
            {
                for (int j = 0; j < sizeBoard; j++)
                {
                    int tag = (int)images[i, j].Tag;
                    if (tag != (i * sizeBoard + j))
                    { return false; }
                }
            }

            return true;
        }

        private void swapImage(int rowImage1, int colImage1, int rowImage2, int colImage2)
        {
            Canvas.SetLeft(images[rowImage1, colImage1], rowImage2 * widthImage);
            Canvas.SetTop(images[rowImage1, colImage1], colImage2 * heightImage);
            Canvas.SetLeft(images[rowImage2, colImage2], rowImage1 * widthImage);
            Canvas.SetTop(images[rowImage2, colImage2], colImage1 * heightImage);
        }

        public void shuffleImage()
        {
            Random random = new Random();

            int selectedRow = 2, selectedCol = 2;
            selectedImage = images[selectedRow, selectedCol];

            for (int i = 0; i < 120; i++)
            {
                int rngRow = random.Next(-1, 1);
                int rngCol = random.Next(-1, 1);

                if (Math.Abs(rngRow) != Math.Abs(rngCol))
                {

                    int newRow = selectedRow + rngRow;
                    int newCol = selectedCol + rngCol;

                    if (newRow < 0 || newRow > 2 || newCol < 0 || newCol > 2)
                    {
                        continue;
                    }

                    swapImage(selectedRow, selectedCol, newRow, newCol);

                    images[selectedRow, selectedCol] = images[newRow, newCol];
                    images[newRow, newCol] = selectedImage;

                    selectedRow = newRow;
                    selectedCol = newCol;
                    selectedImage = images[selectedRow, selectedCol];

                }

            }
        }

    }

}
