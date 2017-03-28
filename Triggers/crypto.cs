using System;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace ConfigureOneFlag
{
    /// <summary>
    /// Cryptography class providing for the decryption of server connection string culled from server registry
    /// </summary>
    class crypto
    {
        private Random random;
        private byte[] key;
        private RijndaelManaged rm;
        private UTF8Encoding encoder;
        public void DecryptCS(string cipherText, string sKey)
        {
            random = new Random();
            rm = new RijndaelManaged();
            encoder = new UTF8Encoding();
            key = Convert.FromBase64String(sKey);
            var cryptogram = Convert.FromBase64String(cipherText);

            if (cryptogram.Length < 17)
            {
                Triggers.logEvent = "A cryptographic error occurred decrypting server connection string";
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Error, 234);
                return;
            }

            var vector = cryptogram.Take(16).ToArray();
            var buffer = cryptogram.Skip(16).ToArray();
            DatabaseFactory.connectionString = encoder.GetString(Decrypt(buffer, vector));
        }
        private byte[] Encrypt(byte[] buffer, byte[] vector)
        {
            var encryptor = rm.CreateEncryptor(key, vector);
            return Transform(buffer, encryptor);
        }

        private byte[] Decrypt(byte[] buffer, byte[] vector)
        {
            var decryptor = rm.CreateDecryptor(key, vector);
            return Transform(buffer, decryptor);
        }

        private byte[] Transform(byte[] buffer, ICryptoTransform transform)
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
