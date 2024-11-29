using System.Text;

namespace Ocluse.LiquidSnow.Cryptography.Classical.Ciphers;

/// <summary>
/// A class that runs the Playfair algorithm.
/// </summary>
public class Playfair : ClassicalAlgorithm
{
    /// <summary>
    /// Creates a new instance of the <see cref="Playfair"/> algorithm.
    /// </summary>
    public Playfair(Alphabet alphabet) : base(alphabet)
    {

    }

    /// <summary>
    /// Gets or sets the <see cref="PreferredOrientation"/> to be used in case of conflicts.
    /// </summary>
    public PreferredOrientation PreferredOrientation { get; set; }
    = PreferredOrientation.Horizontal;

    ///<inheritdoc/>
    public override string Run(string input, string key, bool forward)
    {
        var keyTable = new Alphabet(Alphabet.ToString())
        {
            Dimensions = Alphabet.Dimensions
        };

        var distinct = key.Distinct().ToList();

        for (int i = 0; i < distinct.Count; i++)
        {
            keyTable.Move(distinct[i], i);
        }

        if (input.Length % 2 != 0) input += Alphabet[^1];

        var multiplier = forward ? 1 : -1;
        var output = new StringBuilder();

        //Let the ciphering begin:
        for (int i = 0; i < input.Length; i += 2)
        {
            var a = keyTable.DimensionsOf(input[i]);
            var b = keyTable.DimensionsOf(input[i + 1]);

            var newA = new Dimensions(b.X, a.Y);
            var newB = new Dimensions(a.X, b.Y);

            if (a.X == b.X || a.Y == b.Y)
            {
                newA = a;
                newB = b;

                //SamePoint
                if (a == b)
                {
                    newA.X = PreferredOrientation == PreferredOrientation.Horizontal ?
                        newA.X += multiplier : newA.X;
                    newA.Y = PreferredOrientation == PreferredOrientation.Vertical ?
                        newA.Y += multiplier : newA.Y;

                    newB = new Dimensions(newA.X, newA.Y);
                }

                //SameCol
                else if (a.X == b.X)
                {
                    newA.Y += multiplier;
                    newB.Y += multiplier;
                }

                //SameRow
                else
                {
                    newA.X += multiplier;
                    newB.X += multiplier;
                }

                newA.Limit(keyTable.Dimensions);
                newB.Limit(keyTable.Dimensions);
            }

            output.Append(keyTable[newA]);
            output.Append(keyTable[newB]);
        }

        return output.ToString();
    }
}
