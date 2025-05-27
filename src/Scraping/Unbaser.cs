namespace Ocluse.LiquidSnow.Scraping;

internal class Unbaser
{
    private readonly int _base;
    private readonly int _selector;

    private static readonly Dictionary<int, string> Alphabet = new()
        {
            { 52, "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOP" },
            { 54, "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQR" },
            { 62, "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ" },
            { 95, @" !\""#\$%&\\'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~" }
        };

    public Unbaser(int baseValue)
    {
        _base = baseValue;
        _selector =
            _base > 62 ? 95
            : _base > 54 ? 62
            : _base > 52 ? 54
            : 52;
    }

    public int Unbase(string value)
    {
        if (_base >= 2 && _base < 36)
        {
            try
            {
                return Convert.ToInt32(value, _base);
            }
            catch
            {
                return 0;
            }
        }
        else
        {
            var dict = Alphabet[_selector].ToDictionary(c => c, c => Alphabet[_selector].IndexOf(c));
            var returnVal = 0;

            foreach (var cipher in value.Reverse())
            {
                returnVal += (int)(Math.Pow(_base, value.Length - 1 - value.IndexOf(cipher)) * dict[cipher]);
            }

            return returnVal;
        }
    }
}