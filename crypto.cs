using System;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace ConfigureOneFlag
{
    /// <summary>
    /// Cryptography class providing for the decryption of encrypted strings.  Key passed in is the base64 ENCODED version for extra obfuscation.
    /// Using defaults to maintain FIPS-197 AES compliance/interoperability:  Key size - 256 bits, Block size - 128 bits
    /// </summary>
    class crypto
    {
        private static Random random;
        private static byte[] key;
        private static RijndaelManaged rm;
        private static UTF8Encoding encoder;
        public static string DecryptCS(string cipherText, string sKey)
        {
            random = new Random();
            rm = new RijndaelManaged();
            encoder = new UTF8Encoding();
            key = Convert.FromBase64String(sKey);       //cipher encrypted with non-base64 version of key; convert from base64 for decryption
            var cryptogram = Convert.FromBase64String(cipherText);

            if (cryptogram.Length < 17)
            {
                Triggers.logEvent = "A cryptographic error occurred decrypting cipher";
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Error, 234);
                return "ERR";
            }
            
            var vector = cryptogram.Take(16).ToArray();
            var buffer = cryptogram.Skip(16).ToArray();
            return encoder.GetString(Decrypt(buffer, vector));
        }
        private byte[] Encrypt(byte[] buffer, byte[] vector)
        {
            var encryptor = rm.CreateEncryptor(key, vector);
            return Transform(buffer, encryptor);
        }

        private static byte[] Decrypt(byte[] buffer, byte[] vector)
        {
            var decryptor = rm.CreateDecryptor(key, vector);
            return Transform(buffer, decryptor);
        }

        private static byte[] Transform(byte[] buffer, ICryptoTransform transform)
        {
            var stream = new MemoryStream();
            using (var cs = new CryptoStream(stream, transform, CryptoStreamMode.Write))
            {
                cs.Write(buffer, 0, buffer.Length);
            }

            return stream.ToArray();
        }
    }
}
