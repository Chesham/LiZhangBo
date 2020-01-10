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
            get => sizeLimit;
            set => SetProperty(ref sizeLimit, value);
        }

        abstract public VideoConfigurations VideoConfiguration { get; set; }

        abstract public AudioConfigurations AudioConfiguration { get; set; }

        string sizeLimit;

        string sourcePath;

        string targetPath;
    }
}
