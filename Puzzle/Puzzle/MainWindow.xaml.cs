using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Puzzle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        String imageSource;
        BitmapImage bitmapSource;
        MediaPlayer player = new MediaPlayer();
        Uri uriCurrenMusic;
        DispatcherTimer timer = new DispatcherTimer();
        List<Tuple<int, int>> undo = new List<Tuple<int, int>>();
        List<Tuple<int, int>> redo = new List<Tuple<int, int>>();
        TimeSpan durationGame;
        TimeSpan remainTime;
        int sizeBoard = 0;
        bool isPlaying = false;
        int rowLastImage, colLastImage;
        int widthDisplayedImage, heightDisplayedImage;
        int widthCroppedImage, heightCroppedImage;
        Image[,] images;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer.Tick += tick_Handle;
            timer.Interval = TimeSpan.FromSeconds(1);
            uriCurrenMusic = new Uri("./Musics/ChinaX.mp3", UriKind.Relative);
            player.Open(uriCurrenMusic);
            player.MediaEnded += Player_MediaEnded;
            toggleMusicButton.IsChecked = true;
            stopGame_Click(null, null);

        }

        private void Player_MediaEnded(object sender, EventArgs e)
        {
            player.Stop();
            player.Play();
        }

        private void tick_Handle(object sender, EventArgs e)
        {
            if (remainTime.Minutes != 0 || remainTime.Seconds != 0)
            {
                remainTime = remainTime.Subtract(TimeSpan.FromSeconds(1));
                stopWatchTextBlock.Text = String.Format($"{remainTime.Minutes:00}:{remainTime.Seconds:00}");
            }
            else
            {
                stopGame_Click(null, null);
                MessageBox.Show("Game over!", "Time Up", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }



        private void newGame_Click(object sender, RoutedEventArgs e)
        {
            GameConfigDialog dialog = new GameConfigDialog();

            if (dialog.ShowDialog() == true)
            {
                imageSource = dialog.ImagePath;
                sizeBoard = dialog.Size;
                durationGame = dialog.DurationGame;
                remainTime = dialog.DurationGame;
            }
            else
            {
                return;
            }


            bitmapSource = new BitmapImage(new Uri(imageSource, UriKind.Absolute));
            sampleImage.Source = bitmapSource;
            images = new Image[sizeBoard, sizeBoard];

            widthDisplayedImage = (int)canvas.ActualWidth / sizeBoard;
            heightDisplayedImage = (int)canvas.ActualHeight / sizeBoard;
            widthCroppedImage = (int)bitmapSource.Width / sizeBoard;
            heightCroppedImage = (int)bitmapSource.Height / sizeBoard;

            try
            {
                cropImage();
            }
            catch (Exception)
            {
                MessageBox.Show("Sorry!", "Invalid Image", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            shuffleImage();
            displayImage();
            isPlaying = true;
            stopWatchTextBlock.Text = String.Format($"{remainTime.Minutes:00}:{remainTime.Seconds:00}");

            saveGameButton.IsEnabled = true;
            playButton.IsEnabled = false;
            pauseuButton.IsEnabled = true;
            stopButton.IsEnabled = true;
            undoButton.IsEnabled = true;
            redoButton.IsEnabled = true;

            timer.Start();



        }


        Image lastImage = new Image();



        private void canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isPlaying == false) return;

            Point lastPoint = e.GetPosition(canvas);


            if (lastPoint.X > canvas.ActualWidth ||
                lastPoint.Y > canvas.ActualHeight ||
                lastPoint.X <= 0 || lastPoint.Y <= 0) return;

            int newRow, newCol;
            newCol = (int)lastPoint.X / widthDisplayedImage;
            newRow = (int)lastPoint.Y / heightDisplayedImage;

            if (Math.Abs(rowLastImage - newRow) + Math.Abs(colLastImage - newCol) != 1) return;
            Tuple<int, int> changedPosition
                = new Tuple<int, int>(newCol - colLastImage, newRow - rowLastImage);

            moveImage(ref rowLastImage, ref colLastImage, changedPosition.Item1, changedPosition.Item2);

            if (undo.Count == 10)
            {
                undo.RemoveAt(0);
            }

            redo.Clear();
            undo.Add((changedPosition));

            if (checkWin() == true)
            {
                stopGame_Click(null, null);
                MessageBox.Show("You won!");
            }



        }




        private void moveImage(ref int selectedRow, ref int selectedCol, int hor, int ver)
        {
            int newRow = selectedRow + ver;
            int newCol = selectedCol + hor;

            if (newRow < 0 || newRow >= sizeBoard || newCol < 0 || newCol >= sizeBoard) return;

            swapImage(ref selectedRow, ref selectedCol, ref newRow, ref newCol);

            lastImage = images[selectedRow, selectedCol];
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (isPlaying == false) return;

            int oldRow = rowLastImage;
            int oldCol = colLastImage;

            Tuple<int, int> changedPosition;

            switch (e.Key)
            {
                case Key.Up:
                    moveImage(ref rowLastImage, ref colLastImage, 0, -1);
                    changedPosition = new Tuple<int, int>(0, -1);
                    break;
                case Key.Down:
                    moveImage(ref rowLastImage, ref colLastImage, 0, 1);
                    changedPosition = new Tuple<int, int>(0, 1);
                    break;
                case Key.Left:
                    moveImage(ref rowLastImage, ref colLastImage, -1, 0);
                    changedPosition = new Tuple<int, int>(-1, 0);
                    break;
                case Key.Right:
                    moveImage(ref rowLastImage, ref colLastImage, 1, 0);
                    changedPosition = new Tuple<int, int>(1, 0);
                    break;
                default:
                    return;
            }

            if (oldRow != rowLastImage || oldCol != colLastImage)
            {
                if (undo.Count == 10)
                {
                    undo.RemoveAt(0);
                }

                redo.Clear();
                undo.Add((changedPosition));


                if (checkWin() == true)
                {
                    winGame();
                }
            }




        }


        private void saveGame_Click(object sender, RoutedEventArgs e)
        {
            pauseGame_Click(null, null);

            StreamWriter writer = new StreamWriter("dataGame.txt");
            writer.WriteLine($"{imageSource}");
            writer.WriteLine($"{sizeBoard}");
            writer.WriteLine($"{remainTime.Minutes}:{remainTime.Seconds}");

            for (int i = 0; i < sizeBoard; i++)
            {
                for (int j = 0; j < sizeBoard; j++)
                {
                    writer.Write($"{images[i, j].Tag} ");
                }

                writer.WriteLine();
            }

            writer.Close();

            MessageBox.Show("Game is saved!", "Save Game", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void playGame_Click(object sender, RoutedEventArgs e)
        {
            isPlaying = true;
            timer.Start();
            pauseuButton.IsEnabled = true;
            playButton.IsEnabled = false;

        }

        private void pauseGame_Click(object sender, RoutedEventArgs e)
        {
            isPlaying = false;
            timer.Stop();
            playButton.IsEnabled = true;
            pauseuButton.IsEnabled = false;
            stopButton.IsEnabled = true;
        }

        private void stopGame_Click(object sender, RoutedEventArgs e)
        {
            isPlaying = false;
            timer.Stop();

            saveGameButton.IsEnabled = false;
            playButton.IsEnabled = false;
            pauseuButton.IsEnabled = false;
            stopButton.IsEnabled = false;
            undoButton.IsEnabled = false;
            redoButton.IsEnabled = false;
        }

        private void loadGame_Click(object sender, RoutedEventArgs e)
        {
            StreamReader reader = new StreamReader("dataGame.txt");

            imageSource = reader.ReadLine();
            if (!File.Exists(imageSource))
            {
                MessageBox.Show("Oh no! Not Found Image!", "Invalid Direction", MessageBoxButton.OK, MessageBoxImage.Error);
                reader.Close();
                return;
            }

            bitmapSource = new BitmapImage(new Uri(imageSource, UriKind.Absolute));

            string[] tokens;
            try
            {
                sizeBoard = int.Parse(reader.ReadLine());
                tokens = reader.ReadLine().Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                remainTime = new TimeSpan(0, int.Parse(tokens[0]), int.Parse(tokens[1]));
            }
            catch (Exception)
            {
                MessageBox.Show("Load game fault!", "Data Error", MessageBoxButton.OK, MessageBoxImage.Error);
                reader.Close();
                return;
            }


            sampleImage.Source = bitmapSource;
            images = new Image[sizeBoard, sizeBoard];
            int[,] posImage = new int[sizeBoard, sizeBoard];

            for (int i = 0; i < sizeBoard; i++)
            {
                tokens = reader.ReadLine().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                for (int j = 0; j < sizeBoard; j++)
                {
                    try
                    {
                        posImage[i, j] = int.Parse(tokens[j]);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Load game fault!", "Data Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        reader.Close();
                        return;
                    }

                }
            }

            reader.Close();

            widthDisplayedImage = (int)canvas.ActualWidth / sizeBoard;
            heightDisplayedImage = (int)canvas.ActualHeight / sizeBoard;
            widthCroppedImage = (int)bitmapSource.Width / sizeBoard;
            heightCroppedImage = (int)bitmapSource.Height / sizeBoard;

            for (int i = 0; i < sizeBoard; i++)
            {
                for (int j = 0; j < sizeBoard; j++)
                {
                    int rowCropImage = posImage[i, j] / sizeBoard;
                    int colCropImage = posImage[i, j] % sizeBoard;

                    Int32Rect rect = new Int32Rect(colCropImage * widthCroppedImage, rowCropImage * heightCroppedImage, widthCroppedImage, heightCroppedImage);
                    CroppedBitmap cropBitmap = new CroppedBitmap(bitmapSource, rect);

                    Image croppedImage = new Image();
                    int margin = 2;
                    croppedImage.Stretch = Stretch.Fill;
                    croppedImage.Width = widthDisplayedImage - 2 * margin;
                    croppedImage.Height = heightDisplayedImage - 2 * margin;
                    croppedImage.Margin = new Thickness(margin);
                    croppedImage.Tag = posImage[i, j];

                    croppedImage.Source = cropBitmap;
                    images[i, j] = croppedImage;

                    if (posImage[i, j] == 8)
                    {
                        lastImage = images[i, j];
                        rowLastImage = i;
                        colLastImage = j;
                    }

                }
            }


            displayImage();
            stopWatchTextBlock.Text = String.Format($"{remainTime.Minutes:00}:{remainTime.Seconds:00}");

            pauseGame_Click(null, null);
            MessageBox.Show("Game is loaded!", "Load Game", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void undoButton_Click(object sender, RoutedEventArgs e)
        {
            if (undo.Count == 0) return;

            var (i, j) = undo[undo.Count - 1];
            redo.Add(undo[undo.Count - 1]);
            undo.RemoveAt(undo.Count - 1);
            moveImage(ref rowLastImage, ref colLastImage, 0 - i, 0 - j);


        }

        private void redoButton_Click(object sender, RoutedEventArgs e)
        {
            if (redo.Count == 0) return;

            var (i, j) = redo[redo.Count - 1];
            undo.Add(redo[redo.Count - 1]);
            redo.RemoveAt(redo.Count - 1);
            moveImage(ref rowLastImage, ref colLastImage, i, j);
        }

        private void displayImage()
        {
            canvas.Children.Clear();

            for (int i = 0; i < sizeBoard; i++)
            {
                for (int j = 0; j < sizeBoard; j++)
                {
                    canvas.Children.Add(images[i, j]);

                    Canvas.SetLeft(images[i, j], j * (widthDisplayedImage));
                    Canvas.SetTop(images[i, j], i * (heightDisplayedImage));
                }
            }
        }

        private void cropImage()
        {

            for (int i = 0; i < sizeBoard; i++)
            {
                for (int j = 0; j < sizeBoard; j++)
                {
                    Int32Rect rect
                        = new Int32Rect(j * widthCroppedImage, i * heightCroppedImage, widthCroppedImage, heightCroppedImage);
                    CroppedBitmap croppedBitmap = new CroppedBitmap(bitmapSource, rect);

                    Image croppedImage = new Image();
                    int margin = 2;
                    croppedImage.Stretch = Stretch.Fill;
                    croppedImage.Width = widthDisplayedImage - 2 * margin;
                    croppedImage.Height = heightDisplayedImage - 2 * margin;
                    croppedImage.Margin = new Thickness(margin);
                    croppedImage.Tag = (int)(i * sizeBoard + j);

                    croppedImage.Source = croppedBitmap;
                    images[i, j] = croppedImage;

                }
            }

            images[sizeBoard - 1, sizeBoard - 1].Opacity = 0.25;
        }

        private bool checkWin()
        {
            for (int i = 0; i < sizeBoard; i++)
            {
                for (int j = 0; j < sizeBoard; j++)
                {
                    int tag = (int)images[i, j].Tag;
                    if (tag != (i * sizeBoard + j)) return false;
                }
            }

            return true;
        }


        private void watchScore_Click(object sender, RoutedEventArgs e)
        {
            HighScoreDialog dialog = new HighScoreDialog(0, 0, 0);

            dialog.Show();
        }

        private void music1_Checked(object sender, RoutedEventArgs e)
        {
            toggleMusicButton.IsChecked = false;
            player.Open(new Uri("./Musics/ChinaX.mp3", UriKind.Relative));
            toggleMusicButton.IsChecked = true;
        }

        private void music2_Checked(object sender, RoutedEventArgs e)
        {
            toggleMusicButton.IsChecked = false;
            player.Open(new Uri("./Musics/Cloud 9.mp3", UriKind.Relative));
            toggleMusicButton.IsChecked = true;
        }
        private void music3_Checked(object sender, RoutedEventArgs e)
        {
            toggleMusicButton.IsChecked = false;
            player.Open(new Uri("./Musics/Spring.mp3", UriKind.Relative));
            toggleMusicButton.IsChecked = true;
        }

        private void toggleMusicButton_Unchecked(object sender, RoutedEventArgs e)
        {
            toggleMusicButton.Header = "Off";
            player.Stop();
        }

        private void toggleMusicButton_Checked(object sender, RoutedEventArgs e)
        {
            toggleMusicButton.Header = "On";
            player.Play();
        }


        public void winGame()
        {
            stopGame_Click(null, null);
            MessageBox.Show("You won!");
            HighScoreDialog dialog
                = new HighScoreDialog(durationGame.Minutes, (int)remainTime.TotalSeconds, sizeBoard);
            dialog.ShowDialog();

        }

        private void swapImage(ref int rowImage1, ref int colImage1, ref int rowImage2, ref int colImage2)
        {
            DoubleAnimation widthAnimation1
                = new DoubleAnimation(colImage2 * widthDisplayedImage, TimeSpan.FromSeconds(0.75));
            DoubleAnimation heightAnimation1
                = new DoubleAnimation(rowImage2 * heightDisplayedImage, TimeSpan.FromSeconds(0.75));

            images[rowImage1, colImage1].BeginAnimation(Canvas.LeftProperty, widthAnimation1);
            images[rowImage1, colImage1].BeginAnimation(Canvas.TopProperty, heightAnimation1);

            DoubleAnimation widthAnimation2
                = new DoubleAnimation(colImage1 * widthDisplayedImage, TimeSpan.FromSeconds(0.75));
            DoubleAnimation heightAnimation2
                = new DoubleAnimation(rowImage1 * heightDisplayedImage, TimeSpan.FromSeconds(0.75));

            images[rowImage2, colImage2].BeginAnimation(Canvas.LeftProperty, widthAnimation2);
            images[rowImage2, colImage2].BeginAnimation(Canvas.TopProperty, heightAnimation2);

            Image tempImage = images[rowImage1, colImage1];
            images[rowImage1, colImage1] = images[rowImage2, colImage2];
            images[rowImage2, colImage2] = tempImage;

            int temp = rowImage1;
            rowImage1 = rowImage2;
            rowImage2 = temp;

            temp = colImage1;
            colImage1 = colImage2;
            colImage2 = temp;

        }

        private void shuffleImage()
        {
            Random random = new Random();

            rowLastImage = sizeBoard - 1;
            colLastImage = sizeBoard - 1;
            lastImage = images[rowLastImage, colLastImage];
            int times = 20 * sizeBoard;

            for (int i = 0; i < times; i++)
            {
                int rngRow = random.Next(-1, 2);
                int rngCol = random.Next(-1, 2);

                int newRow = rowLastImage + rngRow;
                int newCol = colLastImage + rngCol;

                if (newRow >= 0 && newRow < sizeBoard)
                {
                    images[rowLastImage, colLastImage] = images[newRow, colLastImage];
                    images[newRow, colLastImage] = lastImage;

                    rowLastImage = newRow;
                    //lastImage = images[rowLastImage, colLastImage];
                }

                if (newCol >= 0 && newCol < sizeBoard)
                {
                    images[rowLastImage, colLastImage] = images[rowLastImage, newCol];
                    images[rowLastImage, newCol] = lastImage;

                    colLastImage = newCol;
                    //lastImage = images[rowLastImage, colLastImage];
                }

            }
        }

    }

}
