namespace LiZhangBo
{
    sealed class FFMpegConfigurations : CommandConfigurations
    {
        public override string Executable => @"libs\ffmpeg\x64\bin\ffmpeg.exe";

        public string Seek
        {
            get => seek;
            set => SetProperty(ref seek, value);
        }

        public string To
        {
            get => to;
            set => SetProperty(ref to, value);
        }

        public override VideoConfigurations VideoConfiguration
        {
            get => videoConfiguration;
            set => SetProperty(ref videoConfiguration, value as FFMpegVideoConfigurations);
        }

        public override AudioConfigurations AudioConfiguration
        {
            get => audioConfiguration;
            set => SetProperty(ref audioConfiguration, value as FFMpegAudioConfigurations);
        }

        FFMpegAudioConfigurations audioConfiguration = new FFMpegAudioConfigurations();

        FFMpegVideoConfigurations videoConfiguration = new FFMpegVideoConfigurations();

        string seek;

        string to;
    }
}
