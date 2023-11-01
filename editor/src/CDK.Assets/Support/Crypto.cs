using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace CDK.Assets.Support
{
    public class Crypto
    {
        public static byte[] Encrypt(byte[] Input, string key)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);

            var aes = new RijndaelManaged
            {
                KeySize = 256,
                BlockSize = 128,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                Key = keyBytes
            };
            var ivBytes = new byte[16];
            Array.Copy(keyBytes, ivBytes, 16);
            aes.IV = ivBytes;

            var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] xBuff = null;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                {
                    cs.Write(Input, 0, Input.Length);
                }

                xBuff = ms.ToArray();
            }
            return xBuff;
        }

        public static byte[] Decrypt(byte[] Input, string key)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);

            var aes = new RijndaelManaged
            {
                KeySize = 256,
                BlockSize = 128,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                Key = keyBytes
            };
            var ivBytes = new byte[16];
            Array.Copy(keyBytes, ivBytes, 16);

            var decrypt = aes.CreateDecryptor();
            byte[] xBuff = null;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
                {
                    cs.Write(Input, 0, Input.Length);
                }

                xBuff = ms.ToArray();
            }

            return xBuff;
        }

        public static string GetHash(byte[] input, int fixedLength = 0)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                var hashBytes = sha1.ComputeHash(input);
                var hashBuf = new StringBuilder();
                foreach (var b in hashBytes)
                {
                    var hex = b.ToString("x2");
                    hashBuf.Append(hex);
                }
                if (fixedLength != 0)
                {
                    if (hashBuf.Length > fixedLength)
                    {
                        return hashBuf.ToString().Substring(0, fixedLength);
                    }
                    else
                    {
                        while (hashBuf.Length < fixedLength)
                        {
                            hashBuf.Append('0');
                        }
                    }
                }
                return hashBuf.ToString();
            }
        }
    }
}
