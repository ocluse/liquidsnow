using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ocluse.LiquidSnow.Cryptography.Classical.DictionaryAttack
{
    /// <summary>
    /// A class that can perform dictionary attacks on specified inputs using a provided dictionary.
    /// </summary>
    public abstract class DictionaryAttacker
    {
        #region Dictionary Methods

        /// <summary>
        /// Checks the dictionary if the provided <paramref name="input"/> is a valid possibility.
        /// </summary>
        /// <param name="input">The text to be validated</param>
        /// <param name="languageDictionary">The dictionary to look through for a match</param>
        /// <param name="depth">The number of characters to match before determining valid possibility</param>
        /// <returns>True if an item in the dictionary matches the input by at least the specified <paramref name="depth"/></returns>
        protected bool CheckDictionary(string input, IEnumerable<string> languageDictionary, uint depth)
        {
            //Hello my name is regret
            //I'm pretty sure we have met

            if (input.Length == 0) return true;

            foreach (var word in languageDictionary)
            {
                if (word.Length < 3) continue;
                if (input.Contains(word))
                {
                    var nextInput = input.Replace(word, "");
                    if (nextInput.Length < 3) return true;
                    else return CheckDictionary(nextInput, languageDictionary, depth - 1);
                }
                continue;
            }
            return false;
        }
        #endregion

        #region Abstract Methods

        ///<inheritdoc cref="HackAsync(string, IEnumerable{string}, IEnumerable{string}?, uint, CancellationToken, IProgress{float}?)"/>
        public abstract IEnumerable<AttackPossibility> Hack(string input, IEnumerable<string> keyDictionary, IEnumerable<string>? languageDictionary = null, uint matchLength = 3);

        /// <summary>
        /// When implemented, performs a dictionary attack on the specified input.
        /// </summary>
        /// <param name="input">The input to attack</param>
        /// <param name="keyDictionary">The items used as the keys to produce outputs</param>
        /// <param name="languageDictionary">The items used to evaluate if an output is a valid possibility.</param>
        /// <param name="matchLength">The number of characters to match on an output before it is considered valid</param>
        /// <param name="cancellationToken">The cancellation token that will request cancellation of the attack</param>
        /// <param name="progress">If provided, reports on the overall progress of the process</param>
        /// <remarks>
        /// When a <paramref name="languageDictionary"/> is not provided, the <paramref name="keyDictionary"/> is used instead.
        /// </remarks>
        /// <returns>A list of all possible attacks</returns>
        public abstract Task<IEnumerable<AttackPossibility>> HackAsync(string input, IEnumerable<string> keyDictionary, IEnumerable<string>? languageDictionary = null, uint matchLength = 3, CancellationToken cancellationToken = default, IProgress<float>? progress = null);

        #endregion
    }
}