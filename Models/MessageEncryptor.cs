using System.IO;
using System.Security.Cryptography;
using System.Text;
using NServiceBus;
using NServiceBus.MessageMutator;
using NServiceBus.Unicast.Messages;

namespace Models
{
    public class MessageEncryptor : IMutateTransportMessages
    {
        private readonly Rijndael _myRijndael = Rijndael.Create();

        public void MutateIncoming(TransportMessage transportMessage)
        {
            // transportMessage.Body = transportMessage.Body.Reverse().ToArray();

            using (var rijAlg = Rijndael.Create())
            {
                rijAlg.Key = _myRijndael.Key;
                rijAlg.IV = _myRijndael.IV;

                var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);
                byte[] encrypted;
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
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

            using (var rijAlg = Rijndael.Create())
            {
                rijAlg.Key = _myRijndael.Key;
                rijAlg.IV = _myRijndael.IV;

                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                var bytesAsString = "";

                using (var msDecrypt = new MemoryStream(transportMessage.Body))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
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
