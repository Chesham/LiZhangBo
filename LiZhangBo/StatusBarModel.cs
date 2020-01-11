using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiZhangBo
{
    class StatusBarModel : PropertyChangedNotifiable
    {
        public string Status
        {
            get => status;
            set => SetProperty(ref status, value);
        }

        public bool IsIndeterminate
        {
            get => isIndeterminate;
            set => SetProperty(ref isIndeterminate, value);
        }

        public int Minimum
        {
            get => minimum;
            set => SetProperty(ref minimum, value);
        }

        public int Maximum
        {
            get => maximum;
            set => SetProperty(ref maximum, value);
        }

        public int Value
        {
            get => value;
            set => SetProperty(ref this.value, value);
        }

        int value = 0;

        int maximum = 100;

        int minimum = 0;

        bool isIndeterminate = false;

        string status = "Ready";
    }
}
