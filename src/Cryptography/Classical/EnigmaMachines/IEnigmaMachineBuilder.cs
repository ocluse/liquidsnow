namespace Ocluse.LiquidSnow.Cryptography.Classical.EnigmaMachines;

/// <summary>
/// A builder for <see cref="EnigmaMachine"/>.
/// </summary>
public interface IEnigmaMachineBuilder
{
    /// <summary>
    /// Builds an <see cref="EnigmaMachine"/> with the provided parameters.
    /// </summary>
    /// <remarks>
    /// If no parameters were provided, an ASCII based machine will be built.
    /// </remarks>
    EnigmaMachine Build();

    /// <summary>
    /// The alphabet to be used, in the machine and by the builder when creating the rotors and stators.
    /// When not provided, the default <see cref="Alphabet.ASCII"/> is used.
    /// </summary>
    /// <param name="alphabet"></param>
    /// <returns></returns>
    IEnigmaMachineBuilder WithAlphabet(Alphabet alphabet);

    /// <summary>
    /// If provided, the rotors will be created with the specified notches.
    /// The default is 1.
    /// Has no effect if <see cref="WithRotors(IEnumerable{Rotor})"/> was called.
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    IEnigmaMachineBuilder WithNotchCount(int count);

    /// <summary>
    /// When provided, the machine will use the reflector and not create it's own.
    /// If not provided, a random reflector is created from the alphabet.
    /// </summary>
    /// <param name="reflector"></param>
    /// <returns></returns>
    IEnigmaMachineBuilder WithReflector(EnigmaWheel reflector);

    /// <summary>
    /// When provided, the Enigma Machine will be created with that number of rotors.
    /// The default is 3.
    /// Has no effect if <see cref="WithRotors(IEnumerable{Rotor})"/> was called.
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    IEnigmaMachineBuilder WithRotorCount(int count);

    /// <summary>
    /// WHen provided, the machine will use the rotors and not create its own.
    /// If not provided, the builder will create random rotors based on the alphabet
    /// and other provided parameters.
    /// </summary>
    /// <param name="rotors"></param>
    /// <returns></returns>
    IEnigmaMachineBuilder WithRotors(IEnumerable<Rotor> rotors);

    /// <summary>
    /// When provided, the machine will use the stator and not create its own.
    /// If not provided, a default stator is created from the alphabet.
    /// </summary>
    /// <param name="stator"></param>
    /// <returns></returns>
    IEnigmaMachineBuilder WithStator(EnigmaWheel stator);
}