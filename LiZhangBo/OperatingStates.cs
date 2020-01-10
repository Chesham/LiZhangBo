using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        Task task;
    }
}
