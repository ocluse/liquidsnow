namespace Ocluse.LiquidSnow.Steganography.Media.Internals
{
    internal struct WaveFormat
    {
        public short AudioFormat { get; set; }

        public short NumChannels { get; set; }

        public int SampleRate { get; set; }

        public short BitsPerSample { get; set; }

        public readonly int ByteRate => SampleRate * NumChannels * BitsPerSample / 8;

        public readonly int BytesPerSample => BitsPerSample / 8;

        public readonly short BlockAlign => (short)(NumChannels * BitsPerSample / 8);
    }
}
