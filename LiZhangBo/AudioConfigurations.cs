namespace LiZhangBo
{
    abstract class AudioConfigurations : PropertyChangedNotifiable
    {
        public bool Enabled
        {
            get => enabled;
            set => SetProperty(ref enabled, value);
        }

        bool enabled = false;
    }
}
