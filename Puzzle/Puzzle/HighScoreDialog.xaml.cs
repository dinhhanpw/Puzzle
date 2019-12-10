using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace Puzzle
{
    /// <summary>
    /// Interaction logic for HighScoreDialog.xaml
    /// </summary>
    public partial class HighScoreDialog : Window
    {
        public HighScoreDialog(int duration, int remaintime, int size)
        {
            InitializeComponent();
            // lấy các giá trị thời gian tối đa, thời gian còn lại, kích cỡ
            durationGame = duration;
            remainTime = remaintime;
            sizeBoard = size;
        }

        int durationGame;
        int remainTime;
        int sizeBoard;
        // danh sách điểm cao
        BindingList<HighScore> highScores = new BindingList<HighScore>();


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // thời gian tối đa là 0 tương ứng với người chơi đang ở chế độ chỉ được xem
            // thời gian đối đa khác 0, tính toán điểm số
            if (durationGame != 0)
            {
                // tính, hiển thị điểm số của người chơi
                int score = computeScore(durationGame, remainTime, sizeBoard);
                scoreTextBlock.Text = score.ToString();
            }

            // tải lên tên và điểm cao được lưu lại trước đó
            loadData();
            // đặt nguồn cho ListView để hiển thị danh sách điểm cao
            highScoreListView.ItemsSource = highScores;
        }

        /// <summary>
        /// tính toán điểm số của người chơi
        /// </summary>
        /// <param name="durationGame"></param>
        /// <param name="remainTime"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public int computeScore(int durationGame, int remainTime, int size)
        {
            // điếm số trên mỗi giây còn lại = điểm số theo kích cỡ + điểm số theo độ khó
            // điểm số theo kích cỡ: 3 => 10, 4 => 20, 5 => 30
            // điếm số theo độ khó: dễ => 0, trung bình => 5, khó => 10
            int scorePerSecond = (size - 2) * 10 + (size - durationGame) * 5;
            // trả về điểm số
            return scorePerSecond * remainTime;
        }

        /// <summary>
        /// lớp lưu thông tin điểm số
        /// </summary>
        public class HighScore : INotifyPropertyChanged
        {
            // tên người chơi
            public String Name { get; set; }
            // điểm số đạt được
            public int Score { get; set; }

            public HighScore(String name, int score)
            {
                this.Name = name;
                this.Score = score;
            }

            public event PropertyChangedEventHandler PropertyChanged;

        }

        /// <summary>
        /// tải dữ liệu điểm số từ file
        /// </summary>
        public void loadData()
        {
            int count;

            StreamReader reader;
            string line;
            try
            {
                // đọc số lượng điểm số được lưu lại
                reader = new StreamReader("./data/scoreTable.txt");
                line = reader.ReadLine();
                count = int.Parse(line);
            }
            catch (Exception)
            {
                return;
            }

            for (int i = 0; i < count; i++)
            {
                line = reader.ReadLine();
                String[] tokens = line.Split(new String[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                try
                {
                    // đọc tên người chơi và số điểm đạt được
                    HighScore score
                        = new HighScore(tokens[0], int.Parse(tokens[1]));
                    // thêm vào danh sách điểm số
                    highScores.Add(score);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            reader.Close();
        }

        /// <summary>
        /// sắp xếp danh sách điểm số trước khi hiển thị
        /// </summary>
        public void sortScore()
        {
            // danh sách được sắp xếp giảm dẫn theo điểm số từ danh sách điểm số gốc
            List<HighScore> orderScores = highScores.OrderByDescending(score => score.Score).ToList();
            int count = orderScores.Count;
            // luôn hiển thị 10 người chơi có điểm cao nhất
            if (count > 10) count = 10;
            // dọn dẹp danh sách điểm số cũ
            highScores.Clear();

            // thêm các điểm số theo thứ tự được sắp xếp
            for (int i = 0; i < count; i++)
            {
                highScores.Add(orderScores[i]);
            }

        }

        /// <summary>
        /// thêm một điểm số đạt được vào danh sách
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addScoreButton_Click(object sender, RoutedEventArgs e)
        {
            String namePlayer = "Smart Player";
            try
            {
                // lấy tên người chơi, nếu người chơi không nhập tên mắc định sẽ là: Smart Player
                if (!(namePlayerTextBox.Text.Equals(String.Empty)))
                {
                    namePlayer = namePlayerTextBox.Text;
                }
            }
            catch (Exception) { }

            HighScore score = new HighScore(namePlayer, int.Parse(scoreTextBlock.Text));
            // thêm điểm số vừa đạt được vào danh sách
            highScores.Add(score);
            // sắp xếp lại danh sách
            sortScore();
            // đặt nút Add ở trạng thái không thể bấm
            addScoreButton.IsEnabled = false;
        }

        /// <summary>
        /// lưu lại danh sách điểm số trước khi thoát khỏi hộp thoại
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            StreamWriter writer = new StreamWriter("./data/scoreTable.txt");
            // ghi lại số lượng điểm số lưu trong danh sách
            writer.WriteLine(highScores.Count);
            
            // ghi lại tên người chơi và điểm số đạt được
            for (int i = 0; i < highScores.Count; i++)
            {
                writer.Write($"{highScores[i].Name}|");
                writer.WriteLine($"{highScores[i].Score}");
            }

            writer.Close();
        }
    }
}
