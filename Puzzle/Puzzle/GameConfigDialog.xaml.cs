using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for GameConfigDialog.xaml
    /// </summary>
    public partial class GameConfigDialog : Window
    {
        public int Size { set; get; }
        public String ImagePath { set; get; }
        public TimeSpan DurationGame { set; get; }
        public GameConfigDialog()
        {
            InitializeComponent();
        }

        int minutes;
        const int MinDuration = 1;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            size3Radio.IsChecked = true;
        }

        private void browerButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "Image files(*.png; *.jpeg; *.jpg)| *.png; *.jpeg; *.jpg";
            if (dialog.ShowDialog() == true)
            {
                ImagePath = dialog.FileName;
                nameImageTextBlock.Text = ImagePath.Substring(ImagePath.LastIndexOf("\\") + 1);
            }
        }

        private void size3Radio_Checked(object sender, RoutedEventArgs e)
        {
            Size = 3;
            minutes = MinDuration;
            easyGameRadio.IsChecked = true;
        }

        private void size4Radio_Checked(object sender, RoutedEventArgs e)
        {
            Size = 4;
            minutes = MinDuration + 1;
            easyGameRadio.IsChecked = true;
        }

        private void size5Radio_Checked(object sender, RoutedEventArgs e)
        {
            Size = 5;
            minutes = MinDuration + 2;
            easyGameRadio.IsChecked = true;
        }


        private void easyGameRadio_Checked(object sender, RoutedEventArgs e)
        {
            DurationGame = new TimeSpan(0, minutes + 2, 0);
        }

        private void mediumGameRadio_Checked(object sender, RoutedEventArgs e)
        {
            DurationGame = new TimeSpan(0, minutes + 1, 0);
        }

        private void hardGameRadio_Checked(object sender, RoutedEventArgs e)
        {
            DurationGame = new TimeSpan(0, minutes, 0);
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            if (ImagePath == null)
            {
                MessageBox.Show("Image must not be null!", "Invalid Value", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DialogResult = true;
            Close();
        }


    }
}
