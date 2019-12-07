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
            durationGame = duration;
            remainTime = remaintime;
            sizeBoard = size;

        }

        int durationGame;
        int remainTime;
        int sizeBoard;

        BindingList<HighScore> highScores = new BindingList<HighScore>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (durationGame != 0)
            {
                int score = computeScore(durationGame, remainTime, sizeBoard);
                scoreTextBlock.Text = score.ToString();
            }

            loadData();
            highScoreListView.ItemsSource = highScores;


        }

        public int computeScore(int durationGame, int remainTime, int size)
        {
            int scorePerSecond = (size - 2) * 10 + (size - durationGame) * 5;

            return scorePerSecond * remainTime;
        }

        public class HighScore : INotifyPropertyChanged
        {

            public String Name { get; set; }
            public int Score { get; set; }

            public HighScore(String name, int score)
            {
                this.Name = name;
                this.Score = score;

            }

            public event PropertyChangedEventHandler PropertyChanged;

        }

        public void loadData()
        {
            int count;

            StreamReader reader = new StreamReader("scoreTable.txt");

            string line = reader.ReadLine();

            try
            {
                count = int.Parse(line);
            }
            catch (Exception)
            {
                count = 0;
            }

            for (int i = 0; i < count; i++)
            {
                line = reader.ReadLine();

                String[] tokens = line.Split(new String[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                try
                {
                    HighScore score
                        = new HighScore(tokens[0], int.Parse(tokens[1]));
                    highScores.Add(score);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            reader.Close();
        }


        public void sortScore()
        {

            List<HighScore> orderScores = highScores.OrderByDescending(score => score.Score).ToList();
            int count = orderScores.Count - orderScores.Count / 11;
            highScores.Clear();
            for (int i = 0; i < count; i++)
            {
                highScores.Add(orderScores[i]);
            }

        }


        private void addScoreButton_Click(object sender, RoutedEventArgs e)
        {
            String namePlayer = "Smart Player";
            try
            {
                if (!(namePlayerTextBox.Text.Equals(String.Empty)))
                {
                    namePlayer = namePlayerTextBox.Text;
                }
            }
            catch (Exception) { }

            HighScore score;

            try
            {
                score = new HighScore(namePlayer, int.Parse(scoreTextBlock.Text));
            }
            catch (Exception)
            {
                score = new HighScore(namePlayer, 0);
            }
            highScores.Add(score);
            sortScore();
            addScoreButton.IsEnabled = false;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            StreamWriter writer = new StreamWriter("scoreTable.txt");
            writer.WriteLine(highScores.Count);
            
            for (int i = 0; i < highScores.Count; i++)
            {
                writer.Write($"{highScores[i].Name}|");
                writer.WriteLine($"{highScores[i].Score}");
            }

            writer.Close();
        }
    }
}
