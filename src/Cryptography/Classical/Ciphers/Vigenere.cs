using System.Text;

namespace Ocluse.LiquidSnow.Cryptography.Classical.Ciphers
{
    /// <summary>
    /// A class that can encrypt and decrypt the Vigenère cipher.
    /// </summary>
    /// <remarks>
    /// The <see cref="Vigenere"/> is similar to the <see cref="Caesar"/> but it is superior in that instead of using only a one char key, 
    /// it utilizes a longer key padded to fit the input, making it ever so harder to crack.
    /// </remarks>
    public class Vigenere : ClassicalAlgorithm
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Vigenere"/> algorithm.
        /// </summary>
        public Vigenere(Alphabet alphabet) : base(alphabet)
        {

        }

        ///<inheritdoc/>
        public override string Run(string input, string key, bool forward)
        {
            var output = new StringBuilder();

            int indexer = 0;
            foreach (var c in input)
            {
                if (indexer == key.Length) indexer = 0;
                var steps = Alphabet.IndexOf(key[indexer]) * (forward ? 1 : -1);

                output.Append(Alphabet.WrapChar(c, steps));
                indexer++;
            }

            return output.ToString();
        }
    }
}
