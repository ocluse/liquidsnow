using System;
using System.Collections.Generic;
using System.Text;

namespace Ocluse.LiquidSnow.Cryptography.Classical.EnigmaMachines
{
    /// <summary>
    /// Provides the wheels configurations used for popular Enigma machine variants.
    /// </summary>
    internal class CommonEnigmaWheels
    {
        #region Enigma B - A133
        /// <summary>
        /// The default configuration of the ETW of Enigma B - A133
        /// </summary>
        public static EnigmaWheel A133_ETW
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVXYZÅÄÖ", "ABCDEFGHIJKLMNOPQRSTUVXYZÅÄÖ");
        }
        /// <summary>
        /// The default configuration of the UKW of Enigma B - A133
        /// </summary>
        public static EnigmaWheel A133_UKW
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVXYZÅÄÖ", "LDGBÄNCPSKJAVFZHXUIÅRMQÖOTEY");
        }
        /// <summary>
        /// The default configuration of the the first rotor of Enigma B - A133
        /// </summary>
        public static Rotor A133_I
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVXYZÅÄÖ", "PSBGÖXQJDHOÄUCFRTEZVÅINLYMKA", 'Ä');
        }
        /// <summary>
        /// The default configuration of the second rotor of Enigma B - A133
        /// </summary>
        public static Rotor A133_II
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVXYZÅÄÖ", "CHNSYÖADMOTRZXBÄIGÅEKQUPFLVJ", 'Ä');
        }
        /// <summary>
        /// The default configuration of the third rotor of Enigma B - A133
        /// </summary>
        public static Rotor A133_III
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVXYZÅÄÖ", "ÅVQIAÄXRJBÖZSPCFYUNTHDOMEKGL", 'Ä');
        }
        #endregion

        #region Enigma D - A26
        /// <summary>
        /// The default configuration of the ETW of Enigma D - A26
        /// </summary>
        public static EnigmaWheel A26_ETW
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "QWERTZUIOASDFGHJKPYXCVBNML");
        }
        /// <summary>
        /// The default configuration of the UKW of Enigma D - A26
        /// </summary>
        public static EnigmaWheel A26_UKW
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "IMETCGFRAYSQBZXWLHKDVUPOJN");
        }
        /// <summary>
        /// The default configuration of rotor I of Enigma D - A26
        /// </summary>
        public static Rotor A26_I
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "LPGSZMHAEOQKVXRFYBUTNICJDW", 'Y');
        }
        /// <summary>
        /// The default configuration of rotor II of Enigma D - A26
        /// </summary>
        public static Rotor A26_II
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "SLVGBTFXJQOHEWIRZYAMKPCNDU", 'E');
        }
        /// <summary>
        /// The default configuration of rotor III of Enigma D - A26
        /// </summary>
        public static Rotor A26_III
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "CJGDPSHKTURAWZXFMYNQOBVLIE", 'N');
        }

        #endregion

        #region Enigma I
        /// <summary>
        /// The default configuration of the ETW of Enigma I
        /// </summary>
        public static EnigmaWheel E1_ETW
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        }
        /// <summary>
        /// The default configuration of the one of the UKW of Enigma I
        /// </summary>
        public static EnigmaWheel E1_UKW_A
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "EJMZALYXVBWFCRQUONTSPIKHGD");
        }
        /// <summary>
        /// The default configuration of the one of the UKW of Enigma I
        /// </summary>
        public static EnigmaWheel E1_UKW_B
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "YRUHQSLDPXNGOKMIEBFZCWVJAT");
        }
        /// <summary>
        /// The default configuration of the one of the UKW of Enigma I
        /// </summary>
        public static EnigmaWheel E1_UKW_C
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "FVPJIAOYEDRZXWGCTKUQSBNMHL");
        }
        /// <summary>
        /// The default configuration of rotor I of Enigma I
        /// </summary>
        public static Rotor E1_I
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "EKMFLGDQVZNTOWYHXUSPAIBRCJ", 'Q');
        }
        /// <summary>
        /// The default configuration of rotor II of Enigma I
        /// </summary>
        public static Rotor E1_II
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "AJDKSIRUXBLHWTMCQGZNPYFVOE", 'E');
        }
        /// <summary>
        /// The default configuration of rotor III of Enigma I
        /// </summary>
        public static Rotor E1_III
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "BDFHJLCPRTXVZNYEIWGAKMUSQO", 'V');
        }
        /// <summary>
        /// The default configuration of rotor IV of Enigma I
        /// </summary>
        public static Rotor E1_IV
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "ESOVPZJAYQUIRHXLNFTGKDCMWB", 'J');
        }
        /// <summary>
        /// The default configuration of rotor V of Enigma I
        /// </summary>
        public static Rotor E1_V
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "VZBRGITYUPSDNHLXAWMJQOFECK", 'Z');
        }

        #endregion

        #region Norenigma
        /// <summary>
        /// The default configuration of ETW of Norenigma (Norway Enigma)
        /// </summary>
        public static EnigmaWheel NE_ETW
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        }
        /// <summary>
        /// The default configuration of UKW of Norenigma (Norway Enigma)
        /// </summary>
        public static EnigmaWheel NE_UKW
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "MOWJYPUXNDSRAIBFVLKZGQCHET");
        }
        /// <summary>
        /// The default configuration of rotor I of Norenigma (Norway Enigma)
        /// </summary>
        public static Rotor NE_I
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "WTOKASUYVRBXJHQCPZEFMDINLG", 'Q');
        }
        /// <summary>
        /// The default configuration of rotor II of Norenigma (Norway Enigma)
        /// </summary>
        public static Rotor NE_II
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "GJLPUBSWEMCTQVHXAOFZDRKYNI", 'E');
        }
        /// <summary>
        /// The default configuration of rotor III of Norenigma (Norway Enigma)
        /// </summary>
        public static Rotor NE_III
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "JWFMHNBPUSDYTIXVZGRQLAOEKC", 'V');
        }
        /// <summary>
        /// The default configuration of rotor IV of Norenigma (Norway Enigma)
        /// </summary>
        public static Rotor NE_IV
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "FGZJMVXEPBWSHQTLIUDYKCNRAO", 'J');
        }
        /// <summary>
        /// The default configuration of rotor V of Norenigma (Norway Enigma)
        /// </summary>
        public static Rotor NE_V
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "HEJXQOTZBVFDASCILWPGYNMURK", 'Z');
        }

        #endregion

        #region Sondermaschine
        /// <summary>
        /// The default configuration of ETW of Sondermaschine
        /// </summary>
        public static EnigmaWheel SE_ETW
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        }
        /// <summary>
        /// The default configuration of UKW of Sondermaschine
        /// </summary>
        public static EnigmaWheel SE_UKW
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "CIAGSNDRBYTPZFULVHEKOQXWJM");
        }
        /// <summary>
        /// The default configuration of rotor I of Sondermaschine
        /// </summary>
        public static Rotor SE_I
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "VEOSIRZUJDQCKGWYPNXAFLTHMB", 'Q');
        }
        /// <summary>
        /// The default configuration of rotor II of Sondermaschine
        /// </summary>
        public static Rotor SE_II
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "UEMOATQLSHPKCYFWJZBGVXINDR", 'E');
        }
        /// <summary>
        /// The default configuration of rotor III of Sondermaschine
        /// </summary>
        public static Rotor SE_III
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "TZHXMBSIPNURJFDKEQVCWGLAOY", 'V');
        }
        #endregion

        #region Enigma M3
        /// <summary>
        /// The default configuration of ETW of Enigma M3
        /// </summary>
        public static EnigmaWheel M3_ETW
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        }
        /// <summary>
        /// The default configuration of UKW B of Enigma M3
        /// </summary>
        public static EnigmaWheel M3_UKW_B
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "YRUHQSLDPXNGOKMIEBFZCWVJAT");
        }
        /// <summary>
        /// The default configuration of UKW C of Enigma M3
        /// </summary>
        public static EnigmaWheel M3_UKW_C
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "FVPJIAOYEDRZXWGCTKUQSBNMHL");
        }
        /// <summary>
        /// The default configuration of rotor I of Enigma M3
        /// </summary>
        public static Rotor M3_I
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "EKMFLGDQVZNTOWYHXUSPAIBRCJ", 'Q');
        }
        /// <summary>
        /// The default configuration of rotor II of Enigma M3
        /// </summary>
        public static Rotor M3_II
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "AJDKSIRUXBLHWTMCQGZNPYFVOE", 'E');
        }
        /// <summary>
        /// The default configuration of rotor III of Enigma M3
        /// </summary>
        public static Rotor M3_III
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "BDFHJLCPRTXVZNYEIWGAKMUSQO", 'V');
        }
        /// <summary>
        /// The default configuration of rotor IV of Enigma M3
        /// </summary>
        public static Rotor M3_IV
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "ESOVPZJAYQUIRHXLNFTGKDCMWB", 'J');
        }
        /// <summary>
        /// The default configuration of rotor V of Enigma M3
        /// </summary>
        public static Rotor M3_V
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "VZBRGITYUPSDNHLXAWMJQOFECK", 'Z');
        }
        /// <summary>
        /// The default configuration of rotor VI of Enigma M3
        /// </summary>
        public static Rotor M3_VI
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "JPGVOUMFYQBENHZRDKASXLICTW", "ZM");
        }
        /// <summary>
        /// The default configuration of rotor VII of Enigma M3
        /// </summary>
        public static Rotor M3_VII
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "NZJHGRCXMYSWBOUFAIVLPEKQDT", "ZM");
        }
        /// <summary>
        /// The default configuration of rotor VIII of Enigma M3
        /// </summary>
        public static Rotor M3_VIII
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "FKQHTLXOCBJSPDZRAMEWNIUYGV", "ZM");
        }
        #endregion

        #region Enigma M4
        /// <summary>
        /// The default configuration of ETW of Enigma M4
        /// </summary>
        public static EnigmaWheel M4_ETW
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        }
        /// <summary>
        /// The default configuration of UKW Beta of Enigma M4
        /// </summary>
        public static EnigmaWheel M4_UKW_Beta
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "LEYJVCNIXWPBQMDRTAKZGFUHOS");
        }
        /// <summary>
        /// The default configuration of UKW Gamma of Enigma M4
        /// </summary>
        public static EnigmaWheel M4_UKW_Gamma
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "FSOKANUERHMBTIYCWLQPZXVGJD");
        }
        /// <summary>
        /// The default configuration of UKW B of Enigma M4
        /// </summary>
        public static EnigmaWheel M4_UKW_B
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "ENKQAUYWJICOPBLMDXZVFTHRGS");
        }
        /// <summary>
        /// The default configuration of UKW C of Enigma M4
        /// </summary>
        public static EnigmaWheel M4_UKW_C
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "RDOBJNTKVEHMLFCWZAXGYIPSUQ");
        }
        /// <summary>
        /// The default configuration of rotor I of Enigma M4
        /// </summary>
        public static Rotor M4_I
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "EKMFLGDQVZNTOWYHXUSPAIBRCJ", 'Q');
        }
        /// <summary>
        /// The default configuration of rotor II of Enigma M4
        /// </summary>
        public static Rotor M4_II
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "AJDKSIRUXBLHWTMCQGZNPYFVOE", 'E');
        }
        /// <summary>
        /// The default configuration of rotor III of Enigma M4
        /// </summary>
        public static Rotor M4_III
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "BDFHJLCPRTXVZNYEIWGAKMUSQO", 'V');
        }
        /// <summary>
        /// The default configuration of rotor IV of Enigma M4
        /// </summary>
        public static Rotor M4_IV
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "ESOVPZJAYQUIRHXLNFTGKDCMWB", 'J');
        }
        /// <summary>
        /// The default configuration of rotor V of Enigma M4
        /// </summary>
        public static Rotor M4_V
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "VZBRGITYUPSDNHLXAWMJQOFECK", 'Z');
        }
        /// <summary>
        /// The default configuration of rotor VI of Enigma M4
        /// </summary>
        public static Rotor M4_VI
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "JPGVOUMFYQBENHZRDKASXLICTW", "ZM");
        }
        /// <summary>
        /// The default configuration of rotor VII of Enigma M4
        /// </summary>
        public static Rotor M4_VII
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "NZJHGRCXMYSWBOUFAIVLPEKQDT", "ZM");
        }
        /// <summary>
        /// The default configuration of rotor VIII of Enigma M4
        /// </summary>
        public static Rotor M4_VIII
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "FKQHTLXOCBJSPDZRAMEWNIUYGV", "ZM");
        }
        #endregion
    }
}
