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
        DispatcherTimer timer = new DispatcherTimer();
        List<Tuple<int, int>> undo = new List<Tuple<int, int>>();
        List<Tuple<int, int>> redo = new List<Tuple<int, int>>();
        TimeSpan gameTime;
        int sizeBoard = 0;
        bool isPlaying = false;
        int rowSelectedImage, colSelectedImage;
        int widthPieceImage, heightPieceImage;
        int widthCropImage, heightCropImage;
        Image[,] images;
        public MainWindow()
        {
            InitializeComponent();


        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer.Tick += tick_Handle;
            timer.Interval = TimeSpan.FromSeconds(1);

            stopGame();

        }

        private void tick_Handle(object sender, EventArgs e)
        {
            if (gameTime.Minutes != 0 || gameTime.Seconds != 0)
            {
                gameTime = gameTime.Subtract(TimeSpan.FromSeconds(1));
                stopWatchTextBlock.Text = String.Format($"{gameTime.Minutes:00}:{gameTime.Seconds:00}");
            }
            else
            {
                stopGame();
                MessageBox.Show("Game over!", "Time Up", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        BitmapImage bitmap;

        private void newGame_Click(object sender, RoutedEventArgs e)
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

            widthPieceImage = (int)canvas.ActualWidth / sizeBoard;
            heightPieceImage = (int)canvas.ActualHeight / sizeBoard;
            widthCropImage = (int)bitmap.Width / sizeBoard;
            heightCropImage = (int)bitmap.Height / sizeBoard;

            cropImage();
            shuffleImage();
            displayImage();
            isPlaying = true;
            stopWatchTextBlock.Text = String.Format($"{gameTime.Minutes:00}:{gameTime.Seconds:00}");

            saveGameButton.IsEnabled = true;
            playButton.IsEnabled = false;
            pauseuButton.IsEnabled = true;
            stopButton.IsEnabled = true;
            undoButton.IsEnabled = true;
            redoButton.IsEnabled = true;

            timer.Start();


        }

        bool isDragging = false;
        Image lastImage = new Image();
        int startI, startJ;
        Point startPoint, lastPoint;
        private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isPlaying == false) return;

            startPoint = e.GetPosition(canvas);
            isDragging = true;
            startI = (int)startPoint.X / widthPieceImage;
            startJ = (int)startPoint.Y / heightPieceImage;

            lastImage = images[(int)startPoint.X / widthPieceImage, (int)startPoint.Y / heightPieceImage];
        }

        private void canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isPlaying == false) return;

            lastPoint = e.GetPosition(canvas);
            isDragging = false;

            if (lastPoint.X > canvas.ActualWidth || lastPoint.Y > canvas.ActualHeight || lastPoint.X <= 0 || lastPoint.Y <= 0) return;

            int endI, endJ;


            endI = (int)lastPoint.X / widthPieceImage;
            endJ = (int)lastPoint.Y / heightPieceImage;

            if (Math.Abs(startI - endI) + Math.Abs(startJ - endJ) != 1)
            {
                Canvas.SetLeft(lastImage, startI * widthPieceImage);
                Canvas.SetTop(lastImage, startJ * heightPieceImage);
                return;
            }

            swapImage(ref startI, ref startJ, ref endI, ref endJ);


            if (checkWin() == true)
            {
                stopGame();
                MessageBox.Show("You won!");
            }



        }

       /* private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            //if (isPlaying == false) return;

            Point position = e.GetPosition(canvas);

            this.Title = $"{position.X} - {position.Y}";

            if (isDragging == true)
            {
                Canvas.SetLeft(lastImage, e.GetPosition(canvas).X);
                Canvas.SetTop(lastImage, e.GetPosition(canvas).Y);
                Canvas.SetZIndex(lastImage, 1);
                if (position.X > canvas.ActualWidth || position.Y > canvas.ActualHeight || position.X <= 0 || position.Y <= 0 || isPlaying == false)
                {
                    isDragging = false;
                    Canvas.SetLeft(lastImage, startI * widthPieceImage);
                    Canvas.SetTop(lastImage, startJ * heightPieceImage);
                    return;
                }
            }
        }*/


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

            int oldRow = rowSelectedImage;
            int oldCol = colSelectedImage;

            Tuple<int, int> changedPosition;

            switch (e.Key)
            {
                case Key.Up:
                    moveImage(ref rowSelectedImage, ref colSelectedImage, 0, -1);
                    changedPosition = new Tuple<int, int>(0, -1);
                    break;
                case Key.Down:
                    moveImage(ref rowSelectedImage, ref colSelectedImage, 0, 1);
                    changedPosition = new Tuple<int, int>(0, 1);
                    break;
                case Key.Left:
                    moveImage(ref rowSelectedImage, ref colSelectedImage, -1, 0);
                    changedPosition = new Tuple<int, int>(-1, 0);
                    break;
                case Key.Right:
                    moveImage(ref rowSelectedImage, ref colSelectedImage, 1, 0);
                    changedPosition = new Tuple<int, int>(1, 0);
                    break;
                default:
                    return;
            }

            if (oldRow != rowSelectedImage || oldCol != colSelectedImage)
            {
                if (undo.Count == 3)
                {
                    undo.RemoveAt(0);
                }

                redo.Clear();
                undo.Add((changedPosition));


                if (checkWin() == true)
                {
                    isPlaying = false;
                    timer.Stop();
                    playButton.IsEnabled = false;
                    pauseuButton.IsEnabled = false;
                    stopButton.IsEnabled = false;
                    MessageBox.Show("You won!");
                }
            }




        }


        private void saveGame_Click(object sender, RoutedEventArgs e)
        {
            pauseGame_Click(null, null);

            StreamWriter writer = new StreamWriter("dataGame.txt");
            writer.WriteLine($"{imageSource}");
            writer.WriteLine($"{sizeBoard}");
            writer.WriteLine($"{gameTime.Minutes}:{gameTime.Seconds}");

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
            playButton.IsEnabled = false;
            pauseuButton.IsEnabled = false;
        }

        private void loadGame_Click(object sender, RoutedEventArgs e)
        {
            StreamReader reader = new StreamReader("dataGame.txt");

            imageSource = reader.ReadLine();
            string[] tokens;
            try
            {
                sizeBoard = int.Parse(reader.ReadLine());
                tokens = reader.ReadLine().Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                gameTime = new TimeSpan(0, int.Parse(tokens[0]), int.Parse(tokens[1]));
            }
            catch (Exception)
            {
                MessageBox.Show("Load game fault!", "Data Error", MessageBoxButton.OK, MessageBoxImage.Error);
                reader.Close();
                return;
            }

            try
            {
                bitmap = new BitmapImage(new Uri(imageSource, UriKind.Absolute));
            }
            catch (Exception)
            {
                MessageBox.Show("No file found!", "Error Loading Game", MessageBoxButton.OK, MessageBoxImage.Error);
                reader.Close();
                return;
            }

            sampleImage.Source = bitmap;
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

            widthPieceImage = (int)canvas.ActualWidth / sizeBoard;
            heightPieceImage = (int)canvas.ActualHeight / sizeBoard;
            widthCropImage = (int)bitmap.Width / sizeBoard;
            heightCropImage = (int)bitmap.Height / sizeBoard;

            for (int i = 0; i < sizeBoard; i++)
            {
                for (int j = 0; j < sizeBoard; j++)
                {
                    int rowCropImage = posImage[i, j] / sizeBoard;
                    int colCropImage = posImage[i, j] % sizeBoard;

                    Int32Rect rect = new Int32Rect(colCropImage * widthCropImage, rowCropImage * heightCropImage, widthCropImage, heightCropImage);
                    CroppedBitmap cropBitmap = new CroppedBitmap(bitmap, rect);

                    var cropImage = new Image();
                    int margin = 2;
                    cropImage.Stretch = Stretch.Fill;
                    cropImage.Width = widthPieceImage - 2 * margin;
                    cropImage.Height = heightPieceImage - 2 * margin;
                    cropImage.Margin = new Thickness(margin);
                    cropImage.Tag = posImage[i, j];

                    cropImage.Source = cropBitmap;
                    images[i, j] = cropImage;

                    if (posImage[i, j] == 8)
                    {
                        lastImage = images[i, j];
                        rowSelectedImage = i;
                        colSelectedImage = j;
                    }

                }
            }


            displayImage();
            stopWatchTextBlock.Text = String.Format($"{gameTime.Minutes:00}:{gameTime.Seconds:00}");

            pauseGame_Click(null, null);
            MessageBox.Show("Game is loaded!", "Load Game", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void undoButton_Click(object sender, RoutedEventArgs e)
        {
            if (undo.Count == 0) return;

            var (i, j) = undo[undo.Count - 1];
            redo.Add(undo[undo.Count - 1]);
            undo.RemoveAt(undo.Count - 1);
            moveImage(ref rowSelectedImage, ref colSelectedImage, 0 - i, 0 - j);


        }

        private void redoButton_Click(object sender, RoutedEventArgs e)
        {
            if (redo.Count == 0) return;

            var (i, j) = redo[redo.Count - 1];
            undo.Add(redo[redo.Count - 1]);
            redo.RemoveAt(redo.Count - 1);
            moveImage(ref rowSelectedImage, ref colSelectedImage, i, j);
        }

        private void displayImage()
        {
            canvas.Children.Clear();

            for (int i = 0; i < sizeBoard; i++)
            {
                for (int j = 0; j < sizeBoard; j++)
                {
                    canvas.Children.Add(images[i, j]);

                    Canvas.SetLeft(images[i, j], j * (widthPieceImage));
                    Canvas.SetTop(images[i, j], i * (heightPieceImage));
                }
            }
        }

        private void cropImage()
        {

            for (int i = 0; i < sizeBoard; i++)
            {
                for (int j = 0; j < sizeBoard; j++)
                {
                    Int32Rect rect = new Int32Rect(j * widthCropImage, i * heightCropImage, widthCropImage, heightCropImage);
                    CroppedBitmap cropBitmap = new CroppedBitmap(bitmap, rect);

                    var cropImage = new Image();
                    int margin = 2;
                    cropImage.Stretch = Stretch.Fill;
                    cropImage.Width = widthPieceImage - 2 * margin;
                    cropImage.Height = heightPieceImage - 2 * margin;
                    cropImage.Margin = new Thickness(margin);
                    cropImage.Tag = (int)(i * sizeBoard + j);

                    cropImage.Source = cropBitmap;
                    images[i, j] = cropImage;

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

        private void swapImage(ref int rowImage1, ref int colImage1, ref int rowImage2, ref int colImage2)
        {
            Canvas.SetLeft(images[rowImage1, colImage1], colImage2 * widthPieceImage);
            Canvas.SetTop(images[rowImage1, colImage1], rowImage2 * heightPieceImage);
            Canvas.SetLeft(images[rowImage2, colImage2], colImage1 * widthPieceImage);
            Canvas.SetTop(images[rowImage2, colImage2], rowImage1 * heightPieceImage);

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

            rowSelectedImage = sizeBoard - 1;
            colSelectedImage = sizeBoard - 1;
            lastImage = images[rowSelectedImage, colSelectedImage];
            int times = 2 * sizeBoard;

            for (int i = 0; i < times; i++)
            {
                int rngRow = random.Next(-1, 1);
                int rngCol = random.Next(-1, 1);

                int newRow = rowSelectedImage + rngRow;
                int newCol = colSelectedImage + rngCol;

                if (newRow >= 0 && newRow < sizeBoard)
                {
                    images[rowSelectedImage, colSelectedImage] = images[newRow, colSelectedImage];
                    images[newRow, colSelectedImage] = lastImage;

                    rowSelectedImage = newRow;
                    lastImage = images[rowSelectedImage, colSelectedImage];
                }

                if (newCol >= 0 && newCol < sizeBoard)
                {
                    images[rowSelectedImage, colSelectedImage] = images[rowSelectedImage, newCol];
                    images[rowSelectedImage, newCol] = lastImage;

                    colSelectedImage = newCol;
                    lastImage = images[rowSelectedImage, colSelectedImage];
                }

            }
        }

        private void stopGame()
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

    }

}
