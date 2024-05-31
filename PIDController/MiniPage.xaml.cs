using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PIDController
{
    /// <summary>
    /// MiniPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MiniPage : Window
    {
        public MiniPage()
        {
            InitializeComponent();
            this.WindowStyle = WindowStyle.None;
            this.AllowsTransparency = true;
        }

        private void Btn_Back_Click(object sender, RoutedEventArgs e)
        {
            PIDController.MainWindow main = ((MainWindow)Application.Current.MainWindow);
            Button btn = (Button)sender;

            this.Close();
            main.WindowState = WindowState.Normal;
            main.MinModeBtn.Content = "Minimize";
            main.MinModeBtn.IsChecked = false;
        }

        public void Click_MinPlayPause(object sender, EventArgs e)
        {
            PIDController.MainWindow main = ((MainWindow)Application.Current.MainWindow);
            main.SendButtonClick(null, null);
        }

        public void Click_MinPrev(object sender, EventArgs e)
        {
            PIDController.MainWindow main = ((MainWindow)Application.Current.MainWindow);
            main.PrevButtonClick(null, null);
        }

        public void Click_MinNext(object sender, EventArgs e)
        {
            PIDController.MainWindow main = ((MainWindow)Application.Current.MainWindow);
            main.NextButtonClick(null, null);

        }

        private void Window_MouseLeftBtnDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
