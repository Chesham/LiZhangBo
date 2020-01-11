namespace LiZhangBo
{
    abstract class CommandConfigurations : PropertyChangedNotifiable
    {
        // 可執行檔位置
        abstract public string Executable { get; }
        // 來源位置
        public virtual string SourcePath
        {
            get => sourcePath;
            set => SetProperty(ref sourcePath, value);
        }
        // 目標位置
        public virtual string TargetPath
        {
            get => targetPath;
            set => SetProperty(ref targetPath, value);
        }

        public virtual string SizeLimit
        {
            get => isFileSizeLimited ? sizeLimit : null;
            set => SetProperty(ref sizeLimit, value);
        }

        public virtual bool IsFileSizeLimited
        {
            get => isFileSizeLimited;
            set => SetProperty(ref isFileSizeLimited, value, nameof(IsFileSizeLimited), nameof(SizeLimit));
        }

        public virtual bool IsMaxBitrateLimited
        {
            get => isMaxBitrateLimited;
            set => SetProperty(ref isMaxBitrateLimited, value, nameof(IsMaxBitrateLimited), nameof(BitrateLimit));
        }

        public virtual string BitrateLimit
        {
            get => isMaxBitrateLimited ? bitrateLimit : null;
            set => SetProperty(ref bitrateLimit, value);
        }

        abstract public VideoConfigurations VideoConfiguration { get; set; }

        abstract public AudioConfigurations AudioConfiguration { get; set; }

        bool isMaxBitrateLimited = true;

        bool isFileSizeLimited = true;

        string bitrateLimit = "15M";

        string sizeLimit = "60M";

        string sourcePath;

        string targetPath;
    }
}
