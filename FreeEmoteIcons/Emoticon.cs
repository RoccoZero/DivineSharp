namespace FreeEmoteIcons
{
    internal sealed class Emoticon
    {
        public string FileName;
        public string ImgName;
        public int MsPerFrame;
        public int FrameCount;
        public int Value;
        public bool Loaded;

        public Emoticon(string fileName, string imgName, int value, int msPerFrame)
        {
            FileName = fileName;
            ImgName = imgName;
            Value = value;
            MsPerFrame = msPerFrame;
        }


    }
}
