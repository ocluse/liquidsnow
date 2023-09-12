namespace Ocluse.LiquidSnow.Steganography.Media.Internals
{
    internal class Channel
    {
        public byte[] Data { get; set; }

        public Channel(byte[] data)
        {
            Data = data;
        }
    }
}
