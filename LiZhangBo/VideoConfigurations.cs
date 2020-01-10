using System.Collections.ObjectModel;

namespace LiZhangBo
{
    abstract class VideoConfigurations : PropertyChangedNotifiable
    {
        public bool Enabled
        {
            get => enabled;
            set => SetProperty(ref enabled, value);
        }

        public bool Is2Passing
        {
            get => is2Passing;
            set => SetProperty(ref is2Passing, value);
        }

        abstract public ObservableCollection<string> SupportedCodecs { get; }

        abstract public string Codec { get; set; }

        protected bool enabled = true;

        protected bool is2Passing = true;
    }
}
