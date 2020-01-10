using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LiZhangBo
{
    sealed class FFMpegVideoConfigurations : VideoConfigurations
    {
        public override ObservableCollection<string> SupportedCodecs => new ObservableCollection<string>(supportedCodecs);

        public override string Codec
        {
            get => codec;
            set => SetProperty(ref codec, value);
        }

        public FFMpegVideoConfigurations()
        {
            codec = supportedCodecs.FirstOrDefault();
        }

        ISet<string> supportedCodecs = new HashSet<string>()
        {
            "libx264",
        };

        string codec;
    }
}
