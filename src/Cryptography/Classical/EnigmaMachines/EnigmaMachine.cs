using Ocluse.LiquidSnow.Extensions;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using Ocluse.LiquidSnow.Cryptography.Classical.EnigmaMachines.Internals;

namespace Ocluse.LiquidSnow.Cryptography.Classical.EnigmaMachines
{
    ///<summary>
    ///Provides functionality for performing cryptographic operations by simulating the Enigma Machine
    ///</summary>
    /// <remarks>
    /// The Enigma machine was used by the German army during World War 2 for top secret communication.
    /// While this class does it's best to simulate the behaviour of the physical device,
    /// there may still be a few places it falls short. Configuration is necessary to obtain desirable behaviour.
    /// </remarks>
    public partial class EnigmaMachine : ClassicalAlgorithm
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of an Enigma Machine with the provided rotors
        /// </summary>
        public EnigmaMachine(Alphabet alphabet, EnigmaWheel stator, EnigmaWheel reflector, IEnumerable<Rotor> rotors) : base(alphabet)
        {
            Rotors = new ObservableCollection<Rotor>(rotors);
            Alphabet = alphabet;
            Stator = stator;
            Reflector = reflector;
            Rotors.CollectionChanged += OnRotorsChanged;
            Rotors.AddRange(rotors);
        }

        private void OnRotorsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    var rotor = (Rotor)item;
                    rotor.HitNotch += OnRotorHitNotch;
                }
            }

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    var rotor = (Rotor)item;
                    rotor.HitNotch -= OnRotorHitNotch;
                }
            }
        }

        /// <summary>
        /// Creates a builder for a <see cref="EnigmaMachine"/>
        /// </summary>
        /// <returns></returns>
        public static IEnigmaMachineBuilder Create()
        {
            return new EnigmaMachineBuilder();
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets rotors of the Enigma Machine. 
        /// </summary>
        /// <remarks>
        /// The rotors are the moving parts of the machine. Current traverses through the rotors when a key is pressed and goes through the wiring, 
        /// changing contact points from one rotor to the next. This action scrambles the letters
        /// </remarks>
        public ObservableCollection<Rotor> Rotors { get; private set; }

        ///// <summary>
        ///// Gets or sets a value indicating whether the rotors should be reset to the key position after each run.
        ///// </summary>
        //public bool AutoReset { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating if double stepping should be simulated.
        /// </summary>
        public bool DoubleStep { get; set; }

        /// <summary>
        /// Gets or sets the ETW of the machine.
        /// </summary>
        /// <remarks>
        /// This is the first 'wheel' that the electric current flows into before heading to the actual rotors.
        /// It is fixed and does not rotate.
        /// </remarks>
        public EnigmaWheel Stator { get; set; }

        /// <summary>
        /// Gets or sets the reflector of the machine
        /// </summary>
        /// <remarks>
        /// Reflects the electric current back through the wheels. This action is what enables the Enigma encryption to be reversable.
        /// </remarks>
        public EnigmaWheel Reflector { get; set; }

        /// <summary>
        /// Gets or sets the machine's plugboard.
        /// </summary>
        /// <remarks>
        /// The plugboard switch allows for further scrambling by substituting character pairs.
        /// </remarks>
        public Plugboard? Plugboard { get; set; }

        /// <summary>
        /// Gets the current rotation of each of the rotors.
        /// </summary>
        /// <remarks>
        /// Returns an array of integers, with each representing the index of rotation of the currently visible character through the window.
        /// </remarks>
        public int[] RotorConfig
        {
            get
            {
                if (Alphabet == null)
                {
                    throw new InvalidOperationException("Alphabet is null");
                }

                var list = new List<int>();

                foreach (var window in Windows)
                {
                    list.Add(Alphabet.IndexOf(window));
                }

                return list.ToArray();
            }
        }

        /// <summary>
        /// Returns a <see cref="char"/> array of the characters currently visible through the window.
        /// </summary>
        public char[] Windows
        {
            get
            {
                if (Rotors == null)
                    throw new NullReferenceException("Machine has no rotors");
                var list = new List<char>();

                foreach (var rotor in Rotors)
                {
                    list.Add(rotor.Window);
                }

                return list.ToArray();
            }
        }

        #endregion



        #region Private Methods

        private void OnRotorHitNotch(Rotor rotor, bool forward)
        {
            var index = Rotors.IndexOf(rotor);

            if (index == -1) throw new ArgumentException("The rotor that hit the notch was not found", nameof(rotor));

            if (index == Rotors.Count - 1) return;//the last rotor, no need to rotate

            //Perform any necessary double stepping
            if (DoubleStep && index + 1 >= Rotors.Count)
            {
                if (Rotors[index + 1].IsTurnOver())
                {
                    Rotors[index + 1].Rotate();
                }
            }

            index++; //Rotate the next rotor;
            Rotors[index].Rotate(forward);
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Realigns the rotors to a provided key
        /// </summary>
        public void ResetRotors(string key)
        {
            if (key.Length < Rotors.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(key), "The length of the key must match the number of rotors");
            }
            int i = 0;
            foreach (var rotor in Rotors)
            {
                var window = key[i];
                rotor.Reset(window);
                i++;
            }
        }

        #endregion

        #region Logic Methods

        /// <inheritdoc/>
        /// <remarks>
        /// This methods call <see cref="ResetRotors(string)"/> before and after running the algorithm using the provided <paramref name="key"/>.
        /// To run the algorithm without resetting the positions of the rotors, call <see cref="Run(string)"/>.
        /// Also not that the value of <paramref name="forward"/> is ignored.
        /// </remarks>
        public override string Run(string input, string key, bool forward)
        {
            ResetRotors(key);

            string output = Run(input);

            ResetRotors(key);

            return output.ToString();
        }

        /// <summary>
        /// Runs the algorithm, without changing the rotor positions.
        /// </summary>
        /// <returns>The output of the run.</returns>
        public string Run(string input)
        {
            var output = new StringBuilder();

            foreach (var c in input)
                output.Append(Run(c));

            return output.ToString();
        }

        /// <summary>
        /// A special run for the Enigma. Simulates a key press and returns a result.
        /// </summary>
        /// <param name="input">The character that has been depressed</param>
        /// <returns>The output, i.e the character that will be lit in the lamp</returns>
        public virtual char Run(char input)
        {
            //Rotate the FastRotor
            Rotors[0].Rotate();

            //Get the index of the letter in the alphabet:
            int index = Alphabet.IndexOf(input);

            //Pass the current to the Stator:
            index = Stator.GetPath(index, true);

            //Pass the current through successive rotors:
            for (int i = 0; i < Rotors.Count; i++)
            {
                index = Rotors[i].GetPath(index, true);
            }

            //Pass the current the Reflector
            index = Reflector.GetPath(index, true);

            //Pass through the rotors in reverse:
            for (int i = Rotors.Count - 1; i > -1; i--)
            {
                var rotor = Rotors[i];
                index = rotor.GetPath(index, false);
            }

            //Pass the current through the Stator:
            index = Stator.GetPath(index, false);

            var result = Alphabet[index];

            //perform plugboard simulation:
            if (Plugboard != null)
            {
                result = Plugboard.Simulate(result);
            }
            return result;

        }
        #endregion

        #region IO Methods
        /// <summary>
        /// Saves the current enigma machine to the specified stream using a specific codec.
        /// </summary>
        /// <remarks>
        /// This method saves the alphabet and the configuration of the rotors, including the ETw and UKW
        /// </remarks>
        /// <param name="stream">The stream to save the machine options to</param>
        public void Save(Stream stream)
        {
            EnigmaMachineLoader.Save(this, stream);
        }

        /// <summary>
        /// Loads an enigma machine configuration from the provided stream.
        /// </summary>
        /// <param name="stream">The stream containing the Enigma Machine configuration</param>
        /// <returns>An <see cref="EnigmaMachine"/> represented by the configuration in the <paramref name="stream"/></returns>
        /// <exception cref="FormatException">When the stream is not a valid enigma machine configuration.</exception>
        public static EnigmaMachine Load(Stream stream)
        {
            return EnigmaMachineLoader.Load(stream);
        }
        #endregion
    }
}