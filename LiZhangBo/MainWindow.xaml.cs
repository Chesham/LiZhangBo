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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LiZhangBo
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        FFMpegConfigurations Configurations { get; set; } = new FFMpegConfigurations();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = Configurations;
        }

        private void SelectSourcePath(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            var result = dlg.ShowDialog();
            if (!result ?? true)
                return;
            Configurations.SourcePath = dlg.FileName;
        }

        private void SelectTargetPath(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog();
            var result = dlg.ShowDialog();
            if (!result ?? true)
                return;
            Configurations.TargetPath = dlg.FileName;
        }
    }
}
