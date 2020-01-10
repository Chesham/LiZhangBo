using System.Threading;
using System.Threading.Tasks;

namespace LiZhangBo
{
    class OperatingStates : PropertyChangedNotifiable
    {
        public string StartButtonText => task == null ? "開始" : "取消";

        public Task Task
        {
            get => task;
            set => SetProperty(ref task, value, nameof(StartButtonText));
        }

        public CancellationTokenSource Cts { get; set; }

        public string ConsoleOutput
        {
            get => consoleOutput;
            set => SetProperty(ref consoleOutput, value);
        }

        string consoleOutput;

        Task task;
    }
}
