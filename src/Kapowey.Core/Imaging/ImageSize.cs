namespace Kapowey.Core.Imaging
{
    [Serializable]
    public sealed record ImageSize
    {
        public short Height { get; init; }

        public short Width { get; init; }
        
        public ImageSize() : this(80,80)
        {}

        public ImageSize(short height, short width)
        {
            Height = height;
            Width = width;
        }
    }
}