using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace PasswordRepository.Helpers
{
    public class Encrypter
    {
        private const string SALT = "TeamAwesomeAlwaysEatsAtThisIsEat";
        //private const string SALT = "RiviaCintraVengerberg";
        //KEYSIZE divided by 8
        private const string INITVECTOR = "ThisIsInItVector";
        //private const string INITVECTOR = "NetRunnersSystems";
        private const int KEYSIZE = 256;
        
        public static string EncryptString(string text)
        {
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(INITVECTOR);
            byte[] textBytes= Encoding.UTF8.GetBytes(text);
            PasswordDeriveBytes password = new PasswordDeriveBytes(SALT,null);
            byte[] keyBytes = password.GetBytes(KEYSIZE/8);

            RijndaelManaged symetricKey = new RijndaelManaged();
            symetricKey.Mode = CipherMode.CBC;

            ICryptoTransform encryptor = symetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(textBytes, 0, textBytes.Length);
            cryptoStream.FlushFinalBlock();

            byte[] data = memoryStream.ToArray();

            memoryStream.Close();
            cryptoStream.Close();

            return Convert.ToBase64String(data);
        }

        public static string DecryptString(string cipherText)
        {
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(INITVECTOR);
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(SALT, null);
            byte[] keyBytes = password.GetBytes(KEYSIZE / 8);

            RijndaelManaged symetricKey = new RijndaelManaged();
            symetricKey.Mode = CipherMode.CBC;

            ICryptoTransform decryptor = symetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            MemoryStream memory = new MemoryStream(cipherBytes);
            CryptoStream crypto = new CryptoStream(memory, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherBytes.Length];
            int decryptedByteCount = crypto.Read(plainTextBytes, 0, plainTextBytes.Length);
            memory.Close();
            crypto.Close();

            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);

        }

    }
}