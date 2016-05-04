using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using NServiceBus;
using NServiceBus.MessageMutator;
using NServiceBus.Unicast.Messages;

namespace Models
{
    public class MessageEncryptor : IMutateTransportMessages
    {
        private Rijndael _myRijndael = Rijndael.Create();

        public void MutateIncoming(TransportMessage transportMessage)
        {
            // transportMessage.Body = transportMessage.Body.Reverse().ToArray();

            using (Rijndael rijAlg = Rijndael.Create())
            {
                rijAlg.Key = _myRijndael.Key;
                rijAlg.IV = _myRijndael.IV;

                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);
                byte[] encrypted;
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(transportMessage.Body);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
                transportMessage.Body = encrypted;
            }
        }

        public void MutateOutgoing(LogicalMessage logicalMessage, TransportMessage transportMessage)
        {
            //transportMessage.Body = transportMessage.Body.Reverse().ToArray();
            byte [] decryptedContent;

            using (Rijndael rijAlg = Rijndael.Create())
            {
                rijAlg.Key = _myRijndael.Key;
                rijAlg.IV = _myRijndael.IV;

                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                var bytesAsString = "";

                using (MemoryStream msDecrypt = new MemoryStream(transportMessage.Body))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            bytesAsString = srDecrypt.ReadToEnd();
                        }
                    }
                }
                decryptedContent = Encoding.ASCII.GetBytes(bytesAsString);
            }
            transportMessage.Body = decryptedContent;
        }
    }
}
