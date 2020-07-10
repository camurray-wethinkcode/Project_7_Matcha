using System;

namespace Matcha.API.Helpers
{
    public interface IToken
    {
        public string GenerateToken(int length);
    }

    public class Token : IToken
    {
        public string GenerateToken(int length)
        {
            var rnd = new Random();
            var bit_count = length * 6;
            var byte_count = (bit_count + 7) / 8;

            var randBytes = new byte[byte_count];
            rnd.NextBytes(randBytes);

            return Convert.ToBase64String(randBytes);
        }
    }
}
