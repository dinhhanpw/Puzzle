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
        // đường dẫn hình ảnh được chọn bởi người chơi
        String imageSource;

        // bitmap của hình ảnh được chọn
        BitmapImage bitmapSource;

        // bộ phát nhạc nên trong game
        MediaPlayer player = new MediaPlayer();

        // đồng hồ đếm thời gian chơi
        DispatcherTimer timer = new DispatcherTimer();

        // danh sách các lần di chuyển trước đó, bước di chuyển tiếp theo
        List<Tuple<int, int>> undo = new List<Tuple<int, int>>();
        List<Tuple<int, int>> redo = new List<Tuple<int, int>>();

        // thời gian tối đa, thời gian còn lại của lần chơi hiên tại (phụ thuộc vào độ khó đã chọn)
        TimeSpan durationGame;
        TimeSpan remainTime;

        // số lượng hàng( số lượng hàng cột bằng nhau) của các mảnh ghép
        int sizeBoard = 0;

        // xác định người chơi đang chơi game hay không
        bool isPlaying = false;

        // xác định vị trí (hàng, cột) hiện tại của hình ảnh cuối cùng
        int rowLastImage, colLastImage;

        // kích thước của những mảnh ghép được hiển thị trên màn hình người chơi
        int widthDisplayedImage, heightDisplayedImage;

        // kích thước mảnh ghép được cắt từ hình gốc (hình người chơi đã chọn)
        int widthCroppedImage, heightCroppedImage;

        // mảng 2 chiều, lưu vị trí những mảnh ghép
        Image[,] images;

        // lưu thông tin mảnh ghép cuối cùng của hình được chọn ( mảnh ghép này được dùng để di chuyển)
        Image lastImage = new Image();
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// chuẩn bị những thứ cần thiết
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // đặt sự kiện xảy ra sau mỗi nhịp
            timer.Tick += tick_Handle;
            // khoảng thời gian giữa các nhịp
            timer.Interval = TimeSpan.FromSeconds(1);

            // đặt nhạc nền và phát lại khi đã chơi hết
            player.Open(new Uri("./Musics/ChinaX.mp3", UriKind.Relative));
            player.MediaEnded += Player_MediaEnded;

            // đặt nút bật tắt nhạc nền ở trạng thái bặt
            toggleMusicButton.IsChecked = true;

            // đặt trò chơi ở trạng thái dừng
            stopGame_Click(null, null);
        }

        /// <summary>
        /// đặt sự kiện khi chơi hết một bài hát
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Player_MediaEnded(object sender, EventArgs e)
        {
            // dừng bài hát hiện tại
            player.Stop();
            // chơi lại bài hát đó
            player.Play();
        }

        /// <summary>
        /// đặt sự kiện sau mỗi nhịp đồng hồ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tick_Handle(object sender, EventArgs e)
        {
            // kiểm tra đã hết thời gian chưa
            if (remainTime.Minutes != 0 || remainTime.Seconds != 0)
            {
                // trừ đi 1 giây và hiển thị nếu chưa hết thời gian
                remainTime = remainTime.Subtract(TimeSpan.FromSeconds(1));
                stopWatchTextBlock.Text = String.Format($"{remainTime.Minutes:00}:{remainTime.Seconds:00}");
            }
            else
            {
                // nếu hêt thời gian thì ngừng trò chơi và hiển thị thông báo
                stopGame_Click(null, null);
                MessageBox.Show("Game over!", "Time Up", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        /// <summary>
        /// sự kiện khi nhấn New - tạo một trò chơi mới
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newGame_Click(object sender, RoutedEventArgs e)
        {
            GameConfigDialog dialog = new GameConfigDialog();

            // hiển thị hộp thoại cấu hình game
            if (dialog.ShowDialog() == true)
            {
                // nếu kêt quả trả về OK
                // lấy các thông tin: đường dẫn hình ảnh, kích cỡ bàn chơi, thời lượng trò chơi
                imageSource = dialog.ImagePath;
                sizeBoard = dialog.Size;
                durationGame = dialog.DurationGame;
                remainTime = dialog.DurationGame;
            }
            else
            {
                // ngược lại, hủy tạo trò chơi mới
                return;
            }

            // tạo bitmap từ đường dẫn nhận được
            bitmapSource = new BitmapImage(new Uri(imageSource, UriKind.Absolute));
            // đặt hình mẫu
            sampleImage.Source = bitmapSource;
            // khởi tạo mảng 2 chiều các hình ảnh
            images = new Image[sizeBoard, sizeBoard];

            // tính toán kích thước
            getDimension();

            // cắt hình ảnh gốc thành các mảnh tùy theo kích cỡ đã chọn
            for (int i = 0; i < sizeBoard; i++)
            {
                for (int j = 0; j < sizeBoard; j++)
                {
                    Int32Rect rect
                        = new Int32Rect(j * widthCroppedImage, i * heightCroppedImage, widthCroppedImage, heightCroppedImage);
                    CroppedBitmap croppedBitmap = new CroppedBitmap(bitmapSource, rect);

                    images[i, j] = createNewImage(i * sizeBoard + j, croppedBitmap);
                }
            }

            // đặt mảnh ghép cuối cùng trong suốt hơn các mảnh ghép khác
            images[sizeBoard - 1, sizeBoard - 1].Opacity = 0.4;

            // xáo trộn các mảnh ghép
            shuffleImage();
            // hiển thị các mảnh ghép lên màn hình
            displayImage();
            // đặt trò chơi ở trạng thái đang chơi, đặt thời gian đếm ngược
            isPlaying = true;
            stopWatchTextBlock.Text = String.Format($"{remainTime.Minutes:00}:{remainTime.Seconds:00}");

            // đặt một số nút bấm ở trạng thái có thể bấm
            saveGameButton.IsEnabled = true;
            playButton.IsEnabled = false;
            pauseuButton.IsEnabled = true;
            stopButton.IsEnabled = true;
            undoButton.IsEnabled = true;
            redoButton.IsEnabled = true;

            // khởi động bộ đếm thời gian
            timer.Start();
        }

        /// <summary>
        /// lấy, tính toán kích thước của phần hình ảnh được cắt và kích thước của mảnh ghép được hiển thị
        /// </summary>
        private void getDimension()
        {
            // tính toán kích thước của mảnh ghép được hiển thi
            widthDisplayedImage = (int)canvas.ActualWidth / sizeBoard;
            heightDisplayedImage = (int)canvas.ActualHeight / sizeBoard;
            // tính toán kích thước của phần hình ảnh được cắt
            widthCroppedImage = (int)bitmapSource.PixelWidth / sizeBoard;
            heightCroppedImage = (int)bitmapSource.PixelHeight / sizeBoard;
        }

        /// <summary>
        /// tạo một mảnh ghép mới
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="croppedBitmap"></param>
        /// <returns></returns>
        private Image createNewImage(int tag, CroppedBitmap croppedBitmap)
        {
            Image croppedImage = new Image();
            int margin = 2;
            croppedImage.Stretch = Stretch.Fill;
            croppedImage.Width = widthDisplayedImage - 2 * margin;
            croppedImage.Height = heightDisplayedImage - 2 * margin;
            // đặt lề cho mảnh ghép
            croppedImage.Margin = new Thickness(margin);
            croppedImage.Tag = tag;
            croppedImage.Source = croppedBitmap;

            return croppedImage;
        }

        /// <summary>
        /// đặt sự kiện di chuyển hình ảnh khi nhấn chuột trái
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // nếu không ở trạng thái đang chơi thì không thể di chuyển
            if (isPlaying == false) return;

            // lấy tọa độ được chọn trên bàn chơi
            Point lastPoint = e.GetPosition(canvas);
            // nếu ngoài phạm vi bàn chơi thì không di chuyển
            if (lastPoint.X > canvas.ActualWidth ||
                lastPoint.Y > canvas.ActualHeight ||
                lastPoint.X <= 0 || lastPoint.Y <= 0) return;

            // tính toán hàng, cột tương ứng với tọa độ được chọn
            int newRow, newCol;
            newCol = (int)lastPoint.X / widthDisplayedImage;
            newRow = (int)lastPoint.Y / heightDisplayedImage;
            // kiểm tra vị trí mới có hợp lệ
            if (Math.Abs(rowLastImage - newRow) + Math.Abs(colLastImage - newCol) != 1) return;

            // thay đổi vị trí, di chuyển hình ảnh đến vị trí mới
            Tuple<int, int> changedPosition
                = new Tuple<int, int>(newCol - colLastImage, newRow - rowLastImage);
            moveImage(ref rowLastImage, ref colLastImage, changedPosition.Item1, changedPosition.Item2);

            //tối đa lưu trữ 10 lần di chuyển gần nhất
            if (undo.Count == 10)
            {
                undo.RemoveAt(0);
            }

            // làm mới danh sách redo
            redo.Clear();
            // thêm lần di chuyển vừa rồi vào danh sách redo
            undo.Add((changedPosition));

            // kiểm tra đã chiến thắng
            if (checkWin() == true)
            {
                winGame();
            }
        }

        /// <summary>
        /// di chuyển mảnh ghép
        /// </summary>
        /// <param name="selectedRow">hàng của mảnh ghép được chọn</param>
        /// <param name="selectedCol">cột của mảnh ghép được chọn</param>
        /// <param name="hor">di chuyển theo hướng ngang</param>
        /// <param name="ver">di chuyển theo hướng dọc</param>
        private void moveImage(ref int selectedRow, ref int selectedCol, int hor, int ver)
        {
            // tính toán hàng, cột mới cho mảnh ghép
            int newRow = selectedRow + ver;
            int newCol = selectedCol + hor;

            // nếu vượt ra ngoài bàn chơi thì dừng
            if (newRow < 0 || newRow >= sizeBoard || newCol < 0 || newCol >= sizeBoard) return;

            // hoán đổi vị trí 2 mảnh ghép
            swapImage(ref selectedRow, ref selectedCol, ref newRow, ref newCol);
        }

        /// <summary>
        /// đăt sự kiên khi cho các phím di chuyển
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            // kiểm tra trạng thái đang chơi
            if (isPlaying == false) return;

            // lưu vị trí cũ của mảnh ghép trước khi di chuyển
            int oldRow = rowLastImage;
            int oldCol = colLastImage;
            Tuple<int, int> changedPosition;

            switch (e.Key)
            {
                // đặt sự kiện cho phím đi lên
                case Key.Up:
                    moveImage(ref rowLastImage, ref colLastImage, 0, -1);
                    changedPosition = new Tuple<int, int>(0, -1);
                    break;
                // đặt sự kiện cho phím đi xuống
                case Key.Down:
                    moveImage(ref rowLastImage, ref colLastImage, 0, 1);
                    changedPosition = new Tuple<int, int>(0, 1);
                    break;
                // đặt sự kiện cho phím sang trái
                case Key.Left:
                    moveImage(ref rowLastImage, ref colLastImage, -1, 0);
                    changedPosition = new Tuple<int, int>(-1, 0);
                    break;
                // đặt sự kiện cho phím sang phải
                case Key.Right:
                    moveImage(ref rowLastImage, ref colLastImage, 1, 0);
                    changedPosition = new Tuple<int, int>(1, 0);
                    break;
                // dừng khi không phải 4 phím di chuyển
                default:
                    return;
            }

            // kiểm tra hình ảnh có thực sự di chuyển để thêm làm danh sách undo
            if (oldRow != rowLastImage || oldCol != colLastImage)
            {
                // lưu trữ tối đa 10 lần di chuyển gần nhất
                if (undo.Count == 10)
                {
                    undo.RemoveAt(0);
                }

                // làm mới danh sách redo
                redo.Clear();
                // thêm hướng di chuyển vào danh sách undo
                undo.Add((changedPosition));

                // kiểm tra đã thắng chưa
                if (checkWin() == true)
                {
                    winGame();
                }
            }
        }

        /// <summary>
        /// đặt sự kiện cho nút Save - lưu lại trò chơi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveGame_Click(object sender, RoutedEventArgs e)
        {
            // tạm dừng trò chơi
            pauseGame_Click(null, null);

            StreamWriter writer = new StreamWriter("./data/dataGame.txt");
            // lưu đương dẫn hình ảnh
            writer.WriteLine($"{imageSource}");
            // lưu kích cỡ bàn chơi
            writer.WriteLine($"{sizeBoard}");
            // lưu thời gian còn lại
            writer.WriteLine($"{remainTime.Minutes}:{remainTime.Seconds}");
            // lưu thời gian tối đa của trò chơi
            writer.WriteLine($"{durationGame.Minutes}:{durationGame.Seconds}");

            // lưu mảng 2 chiều các hình ảnh (chỉ lưu các Tag của hình ảnh)
            for (int i = 0; i < sizeBoard; i++)
            {
                for (int j = 0; j < sizeBoard; j++)
                {
                    writer.Write($"{images[i, j].Tag} ");
                }

                writer.WriteLine();
            }

            writer.Close();
            // thông báo nếu lưu thành công
            MessageBox.Show("Game is saved!", "Save Game", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// đặt sự kiện cho nút Play - tiếp tục trò chơi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void playGame_Click(object sender, RoutedEventArgs e)
        {
            // đặt trò chơi ở trạng thái đang chơi
            isPlaying = true;
            // chạy lại đồng hồ
            timer.Start();
            // nút Pause có thể bấm
            pauseuButton.IsEnabled = true;
            // nút Play không thể bấm
            playButton.IsEnabled = false;
        }

        /// <summary>
        /// đặt sự kiện cho nút Pause - tạm dừng trò chơi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pauseGame_Click(object sender, RoutedEventArgs e)
        {
            // đặt trò chơi ở trạng thái tạm dừng
            isPlaying = false;
            // dừng đồng hồ
            timer.Stop();
            // nút Play ở trạng thái có thể bấm
            playButton.IsEnabled = true;
            // nút Pause ở trạng thái không thể bấm
            pauseuButton.IsEnabled = false;
            stopButton.IsEnabled = true;
        }

        /// <summary>
        /// đặt sự kiện cho nút Stop - dừng trò chơi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopGame_Click(object sender, RoutedEventArgs e)
        {
            // đặt trò chơi ở trạng thái dừng
            isPlaying = false;
            // dừng dồng hồ
            timer.Stop();
            // đặt một số phím ở trạng thái không thể bấm
            saveGameButton.IsEnabled = false;
            playButton.IsEnabled = false;
            pauseuButton.IsEnabled = false;
            stopButton.IsEnabled = false;
            undoButton.IsEnabled = false;
            redoButton.IsEnabled = false;
        }

        /// <summary>
        /// đặt sự kiện cho nút Load - tải lại một trò chơi đã lưu lần cuối cùng
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadGame_Click(object sender, RoutedEventArgs e)
        {
            StreamReader reader;
            try
            {
                reader = new StreamReader("./data/dataGame.txt");
            }
            catch (Exception)
            {
                MessageBox.Show("Load game fault!", "Data Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // đọc đường dẫn hình ảnh
            imageSource = reader.ReadLine();
            // kiêm tra lỗi khi tạo một bitmap
            try
            {
                bitmapSource = new BitmapImage(new Uri(imageSource, UriKind.Absolute));
            }
            catch (Exception)
            {
                MessageBox.Show("Oh no! Fatal error!", "Invalid Image", MessageBoxButton.OK, MessageBoxImage.Error);
                reader.Close();
                return;
            }

            string[] tokens;
            try
            {
                // đọc kích cỡ bàn chơi
                sizeBoard = int.Parse(reader.ReadLine());
                // đọc thời gian còn lại
                tokens = reader.ReadLine().Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                remainTime = new TimeSpan(0, int.Parse(tokens[0]), int.Parse(tokens[1]));
                // đọc thời gian tối đa
                tokens = reader.ReadLine().Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                durationGame = new TimeSpan(0, int.Parse(tokens[0]), int.Parse(tokens[1]));
            }
            catch (Exception)
            {
                // hiển thị thông báo nếu định dạng của file không đúng
                MessageBox.Show("Load game fault!", "Data Error", MessageBoxButton.OK, MessageBoxImage.Error);
                reader.Close();
                return;
            }

            // đặt hình ảnh mẫu
            sampleImage.Source = bitmapSource;
            // tạo mới mảng 2 chiều các mảnh ghép theo kích cỡ
            images = new Image[sizeBoard, sizeBoard];
            int[,] posImage = new int[sizeBoard, sizeBoard];

            // đọc, lưu vị trí của các mảnh ghép vào mảng 2 chiều các số nguyên
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
            // tính toán kích thước
            getDimension();

            // đặt các mảnh ghép cho mảng 2 chiều
            for (int i = 0; i < sizeBoard; i++)
            {
                for (int j = 0; j < sizeBoard; j++)
                {
                    // tính toán vị trí của hình vị trí của mảng ghép trên hình ảnh gốc
                    int rowCropImage = posImage[i, j] / sizeBoard;
                    int colCropImage = posImage[i, j] % sizeBoard;

                    Int32Rect rect
                        = new Int32Rect(colCropImage * widthCroppedImage,
                        rowCropImage * heightCroppedImage, widthCroppedImage, heightCroppedImage);
                    CroppedBitmap croppedBitmap = new CroppedBitmap(bitmapSource, rect);
                    // tạo mới một mảng ghép
                    images[i, j] = createNewImage(posImage[i, j], croppedBitmap);

                    // lưu vị trí mảnh ghép cuối cùng
                    if (posImage[i, j] == sizeBoard * sizeBoard - 1)
                    {
                        lastImage = images[i, j];
                        rowLastImage = i;
                        colLastImage = j;
                        images[i, j].Opacity = 0.4;
                    }
                }
            }

            // hiển thị các mảnh ghép
            displayImage();
            // hiển thị thời gian còn lại
            stopWatchTextBlock.Text = String.Format($"{remainTime.Minutes:00}:{remainTime.Seconds:00}");
            // tạm dừng trò chơi
            pauseGame_Click(null, null);
            // thông báo tải lại trò chơi thành công
            MessageBox.Show("Game is loaded!", "Load Game", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// đặt sự kiên cho nút Undo - quay lại trạng thái sắp xếp các mảnh ghép trước đó
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void undoButton_Click(object sender, RoutedEventArgs e)
        {
            // kiểm tra danh sách những lần di chuyển trước đó
            if (undo.Count == 0) return;

            // lấy hướng di chuyển trước đó
            var (i, j) = undo[undo.Count - 1];
            // thêm vào danh sách redo
            redo.Add(undo[undo.Count - 1]);
            // loại bỏ bước di chuyển vừa quay lại
            undo.RemoveAt(undo.Count - 1);
            // di chuyển mảnh ghép cuối theo hướng ngược lại
            moveImage(ref rowLastImage, ref colLastImage, 0 - i, 0 - j);
        }

        /// <summary>
        /// đặt sự kiên cho nút redo - tiến tới trạng thái sắp xếp các mảnh ghép trong những lần quay lại (undo)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void redoButton_Click(object sender, RoutedEventArgs e)
        {
            // kiểm tra danh sách những lần quay lại trước
            if (redo.Count == 0) return;

            // lấy phương hướng di chuyển của lần quay lại
            var (i, j) = redo[redo.Count - 1];
            // thêm vào danh sách undo
            undo.Add(redo[redo.Count - 1]);
            // loại bỏ bước di chuyển
            redo.RemoveAt(redo.Count - 1);
            // di chuyển mảnh ghép cuối theo hướng nhận được
            moveImage(ref rowLastImage, ref colLastImage, i, j);
        }

        /// <summary>
        /// hiển thị các mảnh ghép của trò chơi lên màn hình
        /// </summary>
        private void displayImage()
        {
            // dọn dẹp mảnh ghép cũ
            canvas.Children.Clear();

            // hiển thị các mảnh ghép lên bàn chơi tùy theo vị trí
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

        /// <summary>
        /// kiểm tra chiến thắng
        /// </summary>
        /// <returns></returns>
        private bool checkWin()
        {
            // kiểm tra tất cả các mảnh ghép đã vào đúng vị trí hay chưa
            for (int i = 0; i < sizeBoard; i++)
            {
                for (int j = 0; j < sizeBoard; j++)
                {
                    // mảnh ghép đúng vị trí khi Tag = hàng * kích cỡ + cột
                    int tag = (int)images[i, j].Tag;
                    if (tag != (i * sizeBoard + j)) return false;
                }
            }

            return true;
        }

        /// <summary>
        /// đặt sự kiện cho nút HighScore - hiển thị bảng điểm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void watchScore_Click(object sender, RoutedEventArgs e)
        {
            HighScoreDialog dialog = new HighScoreDialog(0, 0, 0);

            dialog.Show();
        }

        /// <summary>
        /// đặt sự kiện cho Music 1 - thay đổi sang phát nhạc nền 1 (China X)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void music1_Checked(object sender, RoutedEventArgs e)
        {
            // tắt nhạc nền
            toggleMusicButton.IsChecked = false;
            // thay đổi đường dẫn bài hát
            player.Open(new Uri("./Musics/ChinaX.mp3", UriKind.Relative));
            // bật nhạc nền
            toggleMusicButton.IsChecked = true;
        }

        /// <summary>
        /// đặt sự kiện cho Music 2 - thay đổi sang phát nhạc nền 2 (Cloud 9)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void music2_Checked(object sender, RoutedEventArgs e)
        {
            // tắt nhạc nền
            toggleMusicButton.IsChecked = false;
            // thay đổi đường dẫn bài hát
            player.Open(new Uri("./Musics/Cloud 9.mp3", UriKind.Relative));
            // bật nhạc nền
            toggleMusicButton.IsChecked = true;
        }

        /// <summary>
        /// đặt sự kiện cho Music 3 - thay đổi sang phát nhạc nền 3 (Spring)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void music3_Checked(object sender, RoutedEventArgs e)
        {
            // tắt nhạc nền
            toggleMusicButton.IsChecked = false;
            // thay đổi đường dẫn bài hát
            player.Open(new Uri("./Musics/Spring.mp3", UriKind.Relative));
            // bật nhạc nền
            toggleMusicButton.IsChecked = true;
        }

        /// <summary>
        /// đăt sự kiện khi tắt nhạc
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toggleMusicButton_Unchecked(object sender, RoutedEventArgs e)
        {
            // thay đổi (Header) sang trạng thái Off
            toggleMusicButton.Header = "Off";
            // dừng phát nhạc
            player.Stop();
        }

        /// <summary>
        /// đặt sự kiện khi bật nhạc
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toggleMusicButton_Checked(object sender, RoutedEventArgs e)
        {
            // thay đổi (Header) sang trạng thái On
            toggleMusicButton.Header = "On";
            // phát nhạc
            player.Play();
        }

        /// <summary>
        /// đặt sự kiện khi chiến thắng trò chơi
        /// </summary>
        public void winGame()
        {
            // dừng trò chơi
            stopGame_Click(null, null);
            // hiển thị thông báo chiến thắng
            MessageBox.Show("You won!");
            // nhập tên để lưu lại điểm số
            HighScoreDialog dialog
                = new HighScoreDialog(durationGame.Minutes, (int)remainTime.TotalSeconds, sizeBoard);
            dialog.ShowDialog();

        }

        /// <summary>
        /// hoán đổi vị trí 2 mảnh ghép
        /// </summary>
        /// <param name="rowImage1">hàng mảnh ghép thứ 1</param>
        /// <param name="colImage1">cột mảnh ghép thứ 1</param>
        /// <param name="rowImage2">hàng mảnh ghép thứ 2</param>
        /// <param name="colImage2">cột mảnh ghép thứ 2</param>
        private void swapImage(ref int rowImage1, ref int colImage1, ref int rowImage2, ref int colImage2)
        {
            // hiệu ứng hoạt hình khi di chuyển mảnh ghép 1 đến vị trí của mảnh ghép 2
            DoubleAnimation widthAnimation1
                = new DoubleAnimation(colImage2 * widthDisplayedImage, TimeSpan.FromSeconds(0.75));
            DoubleAnimation heightAnimation1
                = new DoubleAnimation(rowImage2 * heightDisplayedImage, TimeSpan.FromSeconds(0.75));

            images[rowImage1, colImage1].BeginAnimation(Canvas.LeftProperty, widthAnimation1);
            images[rowImage1, colImage1].BeginAnimation(Canvas.TopProperty, heightAnimation1);

            // hiệu ứng hoạt hình khi di chuyển mảnh ghép 2 đến vị trí của mảnh ghép 1
            DoubleAnimation widthAnimation2
                = new DoubleAnimation(colImage1 * widthDisplayedImage, TimeSpan.FromSeconds(0.75));
            DoubleAnimation heightAnimation2
                = new DoubleAnimation(rowImage1 * heightDisplayedImage, TimeSpan.FromSeconds(0.75));

            images[rowImage2, colImage2].BeginAnimation(Canvas.LeftProperty, widthAnimation2);
            images[rowImage2, colImage2].BeginAnimation(Canvas.TopProperty, heightAnimation2);
            // hoán đổi dữ liệu của 2 mảnh ghép trên mảng 2 chiều
            Image tempImage = images[rowImage1, colImage1];
            images[rowImage1, colImage1] = images[rowImage2, colImage2];
            images[rowImage2, colImage2] = tempImage;
            // hoán đổi vị trí hàng, cột của 2 mảnh ghép
            int temp = rowImage1;
            rowImage1 = rowImage2;
            rowImage2 = temp;

            temp = colImage1;
            colImage1 = colImage2;
            colImage2 = temp;
        }

        /// <summary>
        /// xáo trộn các mảnh ghép của trò chơi
        /// </summary>
        private void shuffleImage()
        {
            // bộ sinh số ngãu nhiên
            Random random = new Random();
            // lưu lại vị trí của mảnh ghép cuối
            rowLastImage = sizeBoard - 1;
            colLastImage = sizeBoard - 1;
            lastImage = images[rowLastImage, colLastImage];
            // số lần xáo trộn
            int times = 30 * sizeBoard;

            // xáo trộn các mảnh ghép
            for (int i = 0; i < times; i++)
            {
                // phát sinh ngẫu nhiên hướng di chuyển ( hướng ngang, dọc thuộc [-1;2))
                // hướng ngang: -1 sang trái, 0 đứng yên, 1 sang phải
                // hướng dọc: -1 lên trên, 0 đứng yên, 1 xuống dưới
                int rngRow = random.Next(-1, 2);
                int rngCol = random.Next(-1, 2);
                // tính vị trí mới cho mảnh ghép cuối
                int newRow = rowLastImage + rngRow;
                int newCol = colLastImage + rngCol;
                // hoán đổi vị trí nếu vị trí mới hợp lệ
                if (newRow >= 0 && newRow < sizeBoard)
                {
                    images[rowLastImage, colLastImage] = images[newRow, colLastImage];
                    images[newRow, colLastImage] = lastImage;
                    rowLastImage = newRow;
                }
                // hoán đổi vị trí nếu vị trí mới hợp lệ
                if (newCol >= 0 && newCol < sizeBoard)
                {
                    images[rowLastImage, colLastImage] = images[rowLastImage, newCol];
                    images[rowLastImage, newCol] = lastImage;
                    colLastImage = newCol;
                }

            }
        }

    }

}
