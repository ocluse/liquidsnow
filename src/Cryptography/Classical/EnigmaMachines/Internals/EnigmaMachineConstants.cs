using System.Text;

namespace Ocluse.LiquidSnow.Cryptography.Classical.EnigmaMachines.Internals;

internal static class EnigmaMachineLoader
{
    const string InvalidFileErrorMessage = "The provided input does not contain a valid Enigma Machine configuration.";
    const string FormatIdentifier = "EGMC";
    const string Alphabet = "alph";
    const string ETW = "ETW ";
    const string UKW = "UKW ";
    const string Index = "indx";
    const string Wire = "wire";
    const string Rotors = "ROTS";
    const string Notch = "ntch";

    public static void Save(EnigmaMachine machine, Stream stream)
    {
        //header
        using var writer = new BinaryWriter(stream);

        //Header
        writer.Write(FormatIdentifier);

        //Alphabet
        writer.Write(Alphabet);
        writer.Write(machine.Alphabet.Count);
        writer.Write(Alphabet.ToString());

        //Stator Indexing:
        writer.Write(ETW);
        writer.Write(Index);
        writer.Write(machine.Stator.Indexing.Count);
        writer.Write(machine.Stator.Indexing.ToString());

        //Stator Wiring
        writer.Write(Wire);
        writer.Write(machine.Stator.Wiring.Count);
        writer.Write(machine.Stator.Wiring.ToString());

        //Reflector Indexing
        writer.Write(UKW);
        writer.Write(Index);
        writer.Write(machine.Reflector.Indexing.Count);
        writer.Write(machine.Reflector.Indexing.ToString());

        //Reflector Wiring
        writer.Write(Wire);
        writer.Write(machine.Reflector.Wiring.Count);
        writer.Write(machine.Reflector.Wiring.ToString());

        //Rotors
        writer.Write(Rotors);
        writer.Write(machine.Rotors.Count);

        foreach (var rotor in machine.Rotors)
        {
            //Indexing
            writer.Write(Index);
            writer.Write(rotor.Indexing.Count);
            writer.Write(rotor.Indexing.ToString());

            //Wiring
            writer.Write(Wire);
            writer.Write(rotor.Wiring.Count);
            writer.Write(rotor.Wiring.ToString());

            //Turnover Notch
            writer.Write(Notch);
            writer.Write(rotor.TurnOver.Count);
            foreach (var notch in rotor.TurnOver)
            {
                writer.Write(notch);
            }
        }
    }

    public static EnigmaMachine Load(Stream stream)
    {
        static FormatException InvalidConfiguration()
        {
            return new FormatException(InvalidFileErrorMessage);
        }

        using var reader = new BinaryReader(stream);

        //HEADER
        var buffer = reader.ReadString();
        if (buffer != FormatIdentifier) throw InvalidConfiguration();


        //ALPHABET
        buffer = reader.ReadString();
        if (buffer != Alphabet) throw InvalidConfiguration();
        var len = reader.ReadInt32();
        buffer = reader.ReadString();
        var alphabet = new Alphabet(buffer);

        //STATOR AND REFLECTOR
        var list = new string[] { ETW, UKW };
        var wheels = new List<EnigmaWheel>(2);
        foreach (var item in list)
        {
            buffer = reader.ReadString();
            if (buffer != item) throw InvalidConfiguration();
            buffer = reader.ReadString();
            if (buffer != Index) throw InvalidConfiguration();
            len = reader.ReadInt32();
            buffer = reader.ReadString();
            var indexing = new Alphabet(buffer);

            buffer = reader.ReadString();
            if (buffer != Wire) throw InvalidConfiguration();
            len = reader.ReadInt32();
            buffer = reader.ReadString();
            var wiring = new Alphabet(buffer);

            var wheel = new EnigmaWheel(indexing, wiring);
            wheels.Add(wheel);
        }

        //ROTORS
        buffer = reader.ReadString();
        if (buffer != Rotors) throw InvalidConfiguration();

        len = reader.ReadInt32();

        var builder = new StringBuilder();
        var rotors = new List<Rotor>();
        while (true)
        {
            builder.Clear();
            buffer = reader.ReadString();
            if (buffer != Index)
                throw InvalidConfiguration();
            len = reader.ReadInt32();
            buffer = reader.ReadString();
            var indexing = new Alphabet(buffer);

            buffer = reader.ReadString();
            if (buffer != Wire)
                throw InvalidConfiguration();
            len = reader.ReadInt32();
            buffer = reader.ReadString();
            var wiring = new Alphabet(buffer);
            var wheel = new EnigmaWheel(indexing, wiring);

            buffer = reader.ReadString();
            if (buffer != Notch)
                throw InvalidConfiguration();

            len = reader.ReadInt32();
            for (int t = 0; t < len; t++)
            {
                builder.Append(reader.ReadChar());
            }

            var rotor = new Rotor(indexing, wiring, builder.ToString());
            rotors.Add(rotor);

            if (reader.PeekChar() == -1) break;
        }

        //BUILDER
        return EnigmaMachine.Create()
            .WithAlphabet(alphabet)
            .WithRotors(rotors)
            .WithStator(wheels[0])
            .WithReflector(wheels[1])
            .Build();
    }
}
