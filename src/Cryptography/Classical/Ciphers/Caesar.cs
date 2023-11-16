using System.Text;

namespace Ocluse.LiquidSnow.Cryptography.Classical.Ciphers
{
    /// <summary>
    /// An class that encrypts and decrypts output using the Caesar Cipher
    /// </summary>
    public class Caesar : ClassicalAlgorithm
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Caesar"/>
        /// </summary>
        public Caesar(Alphabet alphabet) : base(alphabet)
        {

        }

        ///<inheritdoc/>
        ///<remarks>
        /// For the <see cref="Caesar"/> cipher, only the first element of the key is used to determine the steps.
        ///</remarks>
        public override string Run(string input, string key, bool forward)
        {
            int index = Alphabet.IndexOf(key.First());

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("Key does not exist in the alphabet");
            }

            int steps = index * (forward ? 1 : -1);
            StringBuilder output = new StringBuilder();

            foreach (var i in input)
            {
                output.Append(Alphabet.WrapChar(i, steps));
            }

            return output.ToString();
        }
    }
}
