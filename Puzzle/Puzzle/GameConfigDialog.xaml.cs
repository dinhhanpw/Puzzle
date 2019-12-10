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
        // kích cỡ bàn chơi
        public int Size { set; get; }
        // đường dẫn hình ảnh
        public String ImagePath { set; get; }
        // thời lượng trò chơi
        public TimeSpan DurationGame { set; get; }
        public GameConfigDialog()
        {
            InitializeComponent();
        }

        int minutes;
        // thời lượng thấp nhất ( kích cỡ: 3, mức độ khó)
        const int MinDuration = 1;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // đặt ở kích cỡ 3x3
            size3Radio.IsChecked = true;
        }

        /// <summary>
        /// chọn hình ảnh
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void browerButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            // bộ lọc chỉ cho phép chọn file hình ảnh
            dialog.Filter = "Image files(*.png; *.jpeg; *.jpg)| *.png; *.jpeg; *.jpg";
            if (dialog.ShowDialog() == true)
            {
                // lấy hình đường dẫn hình ảnh
                ImagePath = dialog.FileName;
                // hiển thị tên hình ảnh
                nameImageTextBlock.Text = ImagePath.Substring(ImagePath.LastIndexOf("\\") + 1);
            }
        }

        /// <summary>
        /// đặt sự kiện cho nút 3x3 - đặt kích cỡ bàn chơi là 3x3
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void size3Radio_Checked(object sender, RoutedEventArgs e)
        {
            // đặt kich cỡ
            Size = 3;
            // đặt thời lượng (mức độ khó)
            minutes = MinDuration;
            // đưa về mức độ dễ (thời lượng sẽ được cộng thêm)
            easyGameRadio.IsChecked = true;
        }

        /// <summary>
        /// đặt sự kiện cho nút 4x4 - đặt kích cỡ bàn chơi là 4x4
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void size4Radio_Checked(object sender, RoutedEventArgs e)
        {
            // đặt kích cỡ
            Size = 4;
            // đặt thời lượng ( mức đồ khó)
            minutes = MinDuration + 1;
            // đưa về mức độ dễ (thời lượng sẽ được cộng thêm)
            easyGameRadio.IsChecked = true;
        }

        /// <summary>
        /// đặt sự kiện cho nút 5x5 - đặt kích cỡ bàn chơi là 5x5
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void size5Radio_Checked(object sender, RoutedEventArgs e)
        {
            // đặt kích cỡ
            Size = 5;
            // đặt thời lượng (ở mức độ khó)
            minutes = MinDuration + 2;
            // đưa về mức độ dễ (thời lượng sẽ đươc cộng thềm)
            easyGameRadio.IsChecked = true;
        }

        /// <summary>
        /// đặt sự kiện cho nút Easy - thay đổi thời lượng trò chơi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void easyGameRadio_Checked(object sender, RoutedEventArgs e)
        {
            // thời lượng trò chơi cộng thềm 2 phút
            DurationGame = new TimeSpan(0, minutes + 2, 0);
        }

        /// <summary>
        /// đặt sự kiện cho nút Medium - thay đỗi thời lượng trò chơi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mediumGameRadio_Checked(object sender, RoutedEventArgs e)
        {
            // thời lượng trò chơi cộng thềm 1 phút
            DurationGame = new TimeSpan(0, minutes + 1, 0);
        }

        /// <summary>
        /// đặt sự kiên cho nút Difficult - thay đổi thời lượng trò chơi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hardGameRadio_Checked(object sender, RoutedEventArgs e)
        {
            // thời lượng trò chơi không được cộng thêm
            DurationGame = new TimeSpan(0, minutes, 0);
        }

        /// <summary>
        /// đặt sự kiên cho nút Ok - xác nhận tạo trò chơi mới
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            // kiểm tra đã chọn hình ảnh trước khi trả về
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
