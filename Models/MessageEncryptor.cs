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

            byte[] decryptedContent;

            using (_myRijndael)
            {
                _myRijndael.Padding = PaddingMode.PKCS7;
                _myRijndael.Key = _myRijndael.Key;
                _myRijndael.IV = _myRijndael.IV;

                var decryptor = _myRijndael.CreateDecryptor(_myRijndael.Key, _myRijndael.IV);

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

        public void MutateOutgoing(LogicalMessage logicalMessage, TransportMessage transportMessage)
        {
            //transportMessage.Body = transportMessage.Body.Reverse().ToArray();

            using (_myRijndael)
            {
                _myRijndael.Padding = PaddingMode.PKCS7;
                _myRijndael.Key = _myRijndael.Key;
                _myRijndael.IV = _myRijndael.IV;

                var encryptor = _myRijndael.CreateEncryptor(_myRijndael.Key, _myRijndael.IV);
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
    }
}
