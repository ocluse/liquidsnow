namespace Ocluse.LiquidSnow.Cryptography.Classical.EnigmaMachines.Internals
{

    internal class EnigmaMachineBuilder : IEnigmaMachineBuilder
    {
        private int _rotorCount = 3, _notchCount = 1;
        private Alphabet _alphabet = Alphabet.ASCII;
        private List<Rotor> _rotors;
        private EnigmaWheel _stator;
        private EnigmaWheel _reflector;

        public EnigmaMachineBuilder()
        {
            var alphabet = Alphabet.ASCII;
            _stator = new EnigmaWheel(alphabet.ToString(), alphabet.ToString());

            var shuffle = new Alphabet(_alphabet.ToString());

            shuffle.Shuffle();

            _reflector = new EnigmaWheel(_alphabet.ToString(), shuffle.ToString());
            _reflector.Reflect();

            _rotors = [];
            for (int i = 0; i < _rotorCount; i++)
            {
                string notches = "";

                for (int n = 0; n < _notchCount; n++)
                {
                    char c;
                    do
                    {
                        int rnd = CryptoUtility.Random(0, _alphabet.Count);
                        c = _alphabet[rnd];
                    } while (notches.Contains(c));

                    notches += c;
                }

                shuffle.Shuffle();

                var rotor = new Rotor(_alphabet.ToString(), shuffle.ToString(), notches);
                _rotors.Add(rotor);
            }
        }

        IEnigmaMachineBuilder IEnigmaMachineBuilder.WithNotchCount(int count)
        {
            _notchCount = count;
            return this;
        }


        IEnigmaMachineBuilder IEnigmaMachineBuilder.WithRotorCount(int count)
        {
            _rotorCount = count;
            return this;
        }


        IEnigmaMachineBuilder IEnigmaMachineBuilder.WithStator(EnigmaWheel stator)
        {
            _stator = stator;
            return this;
        }


        IEnigmaMachineBuilder IEnigmaMachineBuilder.WithReflector(EnigmaWheel reflector)
        {
            _reflector = reflector;
            return this;
        }


        IEnigmaMachineBuilder IEnigmaMachineBuilder.WithRotors(IEnumerable<Rotor> rotors)
        {
            _rotors = new List<Rotor>(rotors);
            return this;
        }


        IEnigmaMachineBuilder IEnigmaMachineBuilder.WithAlphabet(Alphabet alphabet)
        {
            _alphabet = alphabet;
            return this;
        }


        EnigmaMachine IEnigmaMachineBuilder.Build()
        {
            return new EnigmaMachine(_alphabet, _stator, _reflector, _rotors);
        }
    }
}
