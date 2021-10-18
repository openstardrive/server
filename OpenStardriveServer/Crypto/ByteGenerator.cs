using System;
using System.Security.Cryptography;

namespace OpenStardriveServer.Crypto
{
    public interface IByteGenerator
    {
        byte[] Generate(int numberOfBytes);
        string GenerateAsBase64(int numberOfBytes);
    }
    
    public class ByteGenerator : IByteGenerator
    {
        private static readonly RNGCryptoServiceProvider byteGenerator = new RNGCryptoServiceProvider();

        public byte[] Generate(int numberOfBytes)
        {
            var bytes = new byte[numberOfBytes];
            byteGenerator.GetBytes(bytes);
            return bytes;
        }

        public string GenerateAsBase64(int numberOfBytes)
        {
            return Convert.ToBase64String(Generate(numberOfBytes));
        }
    }
}
