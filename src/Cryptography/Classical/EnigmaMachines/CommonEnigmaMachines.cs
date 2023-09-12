using System;
using System.Collections.Generic;
using System.Text;

namespace Ocluse.LiquidSnow.Cryptography.Classical.EnigmaMachines
{
    /// <summary>
    /// Common Enigma Machines including some that have been used historically.
    /// </summary>
    public class CommonEnigmaMachines
    {
        /// <summary>
        /// A133 is a special variant of EnigmaB, that was delivered to the Swedish SGS on 6 April 1925.
        /// It uses 28 letters, with letters Å, Ä and Ö common in the Swedish language. It lacks letter W though.
        /// </summary>
        public static EnigmaMachine A133
        {
            get =>
                new EnigmaMachine(new Alphabet("ABCDEFGHIJKLMNOPQRSTUVXYZÅÄÖ"), CommonEnigmaWheels.A133_ETW, CommonEnigmaWheels.A133_UKW, new Rotor[]
                    {
                        CommonEnigmaWheels.A133_I,
                        CommonEnigmaWheels.A133_II,
                        CommonEnigmaWheels.A133_III
                    })
                {
                    DoubleStep = true
                };
        }

        /// <summary>
        /// Enigma D, or Commercial Enigma A26,  was introduced in 1926 and served as the basis for most
        /// of the later machines.
        /// </summary>
        public static EnigmaMachine A26
        {
            get =>
                new EnigmaMachine(new Alphabet("ABCDEFGHIJKLMNOPQRSTUVWXYZ"), CommonEnigmaWheels.A26_ETW, CommonEnigmaWheels.A26_UKW, new Rotor[]
                {
                        CommonEnigmaWheels.A26_I,
                        CommonEnigmaWheels.A26_II,
                        CommonEnigmaWheels.A26_III
                })
                {
                    DoubleStep = true
                };
        }

        /// <summary>
        /// The Enigma I was the main machine used by the German Army and Air Force, <i>Luftwaffe</i>. UKW_B was the 
        /// standard reflector.
        /// </summary>
        public static EnigmaMachine Enigma_I
        {
            get =>
                new EnigmaMachine(new Alphabet("ABCDEFGHIJKLMNOPQRSTUVWXYZ"), CommonEnigmaWheels.E1_ETW, CommonEnigmaWheels.E1_UKW_B, new Rotor[]
                {
                    CommonEnigmaWheels.E1_I,
                    CommonEnigmaWheels.E1_II,
                    CommonEnigmaWheels.E1_III
                })
                {
                    DoubleStep = true
                };
        }

        /// <summary>
        /// Immediately after the War in 1945, some captured Enigma-I machines were used by the former 
        /// Norwegian Police Security Service: <i>Overvaakingspolitiet.</i> They modified the wiring
        /// and the UKW. The ETW and position of turnover notches were left unaltered.
        /// </summary>
        public static EnigmaMachine Norenigma
        {
            get =>
                new EnigmaMachine(new Alphabet("ABCDEFGHIJKLMNOPQRSTUVWXYZ"), CommonEnigmaWheels.NE_ETW, CommonEnigmaWheels.NE_UKW, new Rotor[]
                {
                        CommonEnigmaWheels.NE_I,
                        CommonEnigmaWheels.NE_II,
                        CommonEnigmaWheels.NE_III
                })
                {
                    DoubleStep = true
                };
        }

        /// <summary>
        /// In the late 1980s, a strange Enigma machine was discovered in the house of a former intelligence officer,
        /// who used to work for a special unit. It was a standard Enigma I with the UKW changed.
        /// The wheels were marked with the letter <b>S</b>, which probably means <i>Sondermaschine</i>(special machine)
        /// </summary>
        public static EnigmaMachine Sondermaschine
        {
            get =>
                new EnigmaMachine(new Alphabet("ABCDEFGHIJKLMNOPQRSTUVWXYZ"), CommonEnigmaWheels.SE_ETW, CommonEnigmaWheels.SE_UKW, new Rotor[]{
                        CommonEnigmaWheels.SE_I,
                        CommonEnigmaWheels.SE_II,
                        CommonEnigmaWheels.SE_III
                    })
                {
                    DoubleStep = true
                };
        }

        /// <summary>
        /// The M1, M2 and M3 Enigma machines were used by the German Navy(<i>Kriegsmarine</i>). They
        /// are basically compatible with Enigma I.
        /// </summary>
        public static EnigmaMachine Enigma_M3
        {
            get =>
                new EnigmaMachine(new Alphabet("ABCDEFGHIJKLMNOPQRSTUVWXYZ"), CommonEnigmaWheels.M3_ETW, CommonEnigmaWheels.M3_UKW_B, new Rotor[]
                {
                        CommonEnigmaWheels.M3_I,
                        CommonEnigmaWheels.M3_II,
                        CommonEnigmaWheels.M3_III
                })
                {
                    DoubleStep = true
                };
        }

        /// <summary>
        /// This was a further development of the M3 and was used exclusively by the U-boat division of the 
        /// Kriegsmarine. It actually featured four rotors, unlike all the other Enigma Machines. Its extra wheels had 
        /// two notches, instead of the standard one.
        /// </summary>
        public static EnigmaMachine Enigma_M4
        {
            get =>
                new EnigmaMachine(new Alphabet("ABCDEFGHIJKLMNOPQRSTUVWXYZ"), CommonEnigmaWheels.M4_ETW, CommonEnigmaWheels.M4_UKW_B, new Rotor[]
                {
                        CommonEnigmaWheels.M4_I,
                        CommonEnigmaWheels.M4_II,
                        CommonEnigmaWheels.M4_III,
                        CommonEnigmaWheels.M4_IV
                })
                {
                    DoubleStep = true
                };
        }

        /// <summary>
        /// Creates an enigma machine with a random configuration, whose alphabet is the ASCII character set.
        /// </summary>
        public static EnigmaMachine RandomASCII
        {
            get => EnigmaMachine.Create().Build();
        }
    }
}
