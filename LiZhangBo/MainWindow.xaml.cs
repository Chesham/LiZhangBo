using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace LiZhangBo
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        FFMpegConfigurations Configurations { get; set; } = new FFMpegConfigurations();

        OperatingStates OperatingState { get; set; } = new OperatingStates();

        StatusBarModel Status { get; set; } = new StatusBarModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = Configurations;
            StartBtn.DataContext = OperatingState;
            ConsolePanel.DataContext = OperatingState;
            StatusPanel.DataContext = Status;
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
            var sourcePath = Configurations.SourcePath;
            var targetPath = Configurations.TargetPath;
            try
            {
                if (!File.Exists(sourcePath))
                    throw new FileNotFoundException("來源路徑檔案不存在", sourcePath);
                if (!File.Exists(targetPath))
                    Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
                var startAt = DateTime.UtcNow;
                var timeout = TimeSpan.FromMinutes(5);
                var cts = (state.Cts = new CancellationTokenSource(timeout));
                var config = Configurations;
                var videoConfig = config.VideoConfiguration;
                var elapsedEventHandler = new ElapsedEventHandler((_, e_) =>
                {
                    Status.Status = $"{DateTime.UtcNow - startAt:hh\\:mm\\:ss} processing ...";
                });
                var timer = new System.Timers.Timer
                {
                    AutoReset = true,
                    Interval = TimeSpan.FromSeconds(1).TotalMilliseconds,
                    Enabled = true
                };
                timer.Elapsed += elapsedEventHandler;
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
                var cancelDelegation = cts.Token.Register(() => proc.Kill());
                var onExited = new EventHandler((sender_, e_) =>
                {
                    try
                    {
                        state.Task = null;
                        state.Cts = null;
                        cancelDelegation.Dispose();
                    }
                    catch (Exception)
                    {
                        // never throw
                    }
                    finally
                    {
                        timer.Stop();
                        Status.IsIndeterminate = false;
                        if (!(sender_ is Exception) && !cts.IsCancellationRequested)
                        {
                            Status.Value = Status.Maximum;
                            Status.Status = $"{DateTime.UtcNow - startAt:hh\\:mm\\:ss\\.fff} done";
                        }
                        else
                        {
                            Status.Value = Status.Minimum;
                            if (sender_ is Exception)
                                Status.Status = (sender_ as Exception).Message;
                            else if (cts.IsCancellationRequested)
                                Status.Status = "processing cancelled";
                        }
                    }
                });
                state.Task = Task.Run(() =>
                {
                    try
                    {
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
                        Status.IsIndeterminate = true;
                        elapsedEventHandler(this, null);
                    }
                    catch (Exception ex)
                    {
                        onExited(ex, EventArgs.Empty);
                        MessageBox.Show(ex.StackTrace, ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                });
            }
            catch (Exception ex)
            {
                Status.Status = ex.Message;
                MessageBox.Show(ex.StackTrace, ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
