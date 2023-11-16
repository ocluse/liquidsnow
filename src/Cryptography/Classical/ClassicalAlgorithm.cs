using Ocluse.LiquidSnow.Cryptography.Classical.DictionaryAttack;

namespace Ocluse.LiquidSnow.Cryptography.Classical
{
    /// <summary>
    /// The base class for all the classical algorithms.
    /// </summary>
    public abstract class ClassicalAlgorithm : DictionaryAttacker
    {
        /// <summary>
        /// Gets or sets the <see cref="Classical.Alphabet"/> scope of the algorithm.
        /// </summary>
        public virtual Alphabet Alphabet { get; set; }

        /// <summary>
        /// The base constructor for the <see cref="ClassicalAlgorithm"/>
        /// </summary>
        /// <param name="alphabet"></param>
        public ClassicalAlgorithm(Alphabet alphabet)
        {
            Alphabet = alphabet;
        }

        /// <summary>
        /// Encrypts the input text.
        /// </summary>
        /// <returns>The ciphertext</returns>
        public virtual string Encrypt(string input, string key)
        {
            return Run(input, key, true);
        }

        /// <summary>
        /// Decrypts the input text.
        /// </summary>
        /// <returns>The plaintext.</returns>
        public virtual string Decrypt(string input, string key)
        {
            return Run(input, key, false);
        }

        /// <summary>
        /// Runs the algorithm.
        /// </summary>
        /// <param name="input">The text to encrypt or decrypt.</param>
        /// <param name="key">The key to use when running the algorithm.</param>
        /// <param name="forward">If true, encrypts the <paramref name="input"/>, otherwise, it is decrypted.</param>
        /// <returns>A ciphertext, if <paramref name="forward"/> was true, otherwise, a plaintext</returns>
        public abstract string Run(string input, string key, bool forward);

        #region Hack
        ///<inheritdoc/>
        public override IEnumerable<AttackPossibility> Hack(string input, IEnumerable<string> keyDictionary, IEnumerable<string>? languageDictionary = null, uint matchLength = 3)
        {
            return HackAsync(input, keyDictionary, languageDictionary, matchLength).GetAwaiter().GetResult();
        }

        ///<inheritdoc/>
        public override Task<IEnumerable<AttackPossibility>> HackAsync(string input, IEnumerable<string> keyDictionary, IEnumerable<string>? languageDictionary = null, uint matchLength = 3, CancellationToken cancellationToken = default, IProgress<float>? progress = null)
        {
            return Task.Run(() =>
            {
                List<AttackPossibility> result = [];
                float index = 0;
                float size = keyDictionary.Count();
                foreach (var key in keyDictionary)
                {
                    //Check if we have been cancelled;
                    cancellationToken.ThrowIfCancellationRequested();

                    //Run the decryption
                    var output = Decrypt(input, key);
                    if (CheckDictionary(output, languageDictionary ?? keyDictionary, matchLength))
                    {
                        var possibility = new AttackPossibility(key, output);
                        result.Add(possibility);
                    }

                    index++;
                    //Report on progress:
                    progress?.Report(index / size);
                }

                return (IEnumerable<AttackPossibility>)result;
            });
        }
    }

    #endregion
}