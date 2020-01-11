using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace LiZhangBo
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        FFMpegConfigurations Configurations { get; set; } = new FFMpegConfigurations();

        OperatingStates OperatingState { get; set; } = new OperatingStates();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = Configurations;
            StartBtn.DataContext = OperatingState;
            ConsolePanel.DataContext = OperatingState;
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

        private void OnStart(object sender, RoutedEventArgs e)
        {
            var state = OperatingState;
            if (state.Task != null)
            {
                // cancellation
                state.Cts.Cancel();
                return;
            }
            var timeout = TimeSpan.FromMinutes(5);
            var cts = (state.Cts = new CancellationTokenSource(timeout));
            var config = Configurations;
            var videoConfig = config.VideoConfiguration;
            var onExited = new EventHandler((_, e_) =>
            {
                try
                {
                    state.Task = null;
                    state.Cts = null;
                }
                catch (Exception)
                {
                    // never throw
                }
            });
            state.Task = Task.Run(() =>
            {
                try
                {
                    var seek = config.Seek.ParseToTimeSpan();
                    var to = config.To.ParseToTimeSpan();
                    if (seek != null && to == null || seek == null && to != null)
                        throw new ArgumentNullException();
                    var sizeLimit = seek == null ? null : config.SizeLimit.ParseSize() * 8 / (to - seek).Value.TotalSeconds;
                    if (sizeLimit.HasValue)
                        sizeLimit = Math.Round(sizeLimit.Value);
                    var bitrateLimit = config.BitrateLimit.ParseSize();
                    if (sizeLimit > bitrateLimit)
                        sizeLimit = bitrateLimit;
                    var sizeLimitSwitch = sizeLimit == null ? string.Empty : $"{sizeLimit}".WithSwitch("-b:v");
                    var videoSettings = videoConfig.Enabled ? $"-c:v {videoConfig.Codec}{(videoConfig.Is2Passing ? " -pass 1" : string.Empty)}{(seek == null ? string.Empty : sizeLimitSwitch)}" : "-vn";
                    var args = $@"-y {config.Seek.WithSwitch("-ss")}{config.To.WithSwitch("-to")} -i ""{config.SourcePath}"" {videoSettings}{(config.AudioConfiguration.Enabled ? string.Empty : " -an")} ""{config.TargetPath}""";
                    var proc = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = config.Executable,
                            Arguments = args,
                            RedirectStandardError = true,
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            StandardOutputEncoding = Encoding.UTF8,
                            StandardErrorEncoding = Encoding.UTF8,
                        },
                        EnableRaisingEvents = true
                    };
                    cts.Token.Register(() => proc.Kill());
                    var procOutput = new StringBuilder();
                    proc.OutputDataReceived += (_, e_) =>
                    {
                        procOutput.AppendLine(e_.Data);
                    };
                    var procError = new StringBuilder();
                    proc.ErrorDataReceived += (_, e_) =>
                    {
                        procError.AppendLine(e_.Data);
                        state.ConsoleOutput = procError.ToString();
                    };
                    proc.Exited += onExited;
                    proc.Start();
                    proc.BeginOutputReadLine();
                    proc.BeginErrorReadLine();
                }
                catch (Exception ex)
                {
                    onExited(this, EventArgs.Empty);
                    MessageBox.Show(ex.StackTrace, ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }

        private void OnConsoleOutputChanged(object sender, TextChangedEventArgs e)
        {
            var target = sender as TextBox;
            if (target == null)
                return;
            target?.ScrollToEnd();
        }
    }
}
