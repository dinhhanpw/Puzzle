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
        public TimeSpan GameTime { set; get; }
        public GameConfigDialog()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

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
        }

        private void size4Radio_Checked(object sender, RoutedEventArgs e)
        {
            Size = 4;
        }

        private void size5Radio_Checked(object sender, RoutedEventArgs e)
        {
            Size = 10;
        }


        private void easyGameRadio_Checked(object sender, RoutedEventArgs e)
        {
            GameTime = new TimeSpan(0, 10, 0);
        }

        private void mediumGameRadio_Checked(object sender, RoutedEventArgs e)
        {
            GameTime = new TimeSpan(0, 8, 0);
        }

        private void hardGameRadio_Checked(object sender, RoutedEventArgs e)
        {
            GameTime = new TimeSpan(0, 5, 0);
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
