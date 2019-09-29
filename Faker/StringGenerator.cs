using System;
using System.Text;

namespace Faker
{
    public class StringGenerator : IGenerator
    {
        private readonly Random _random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
        public object Generate()
        {
            var builder = new StringBuilder();
            var size = _random.Next(1, 100);
            for (var i = 0; i < size; i++)
            {
                var symbol = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * _random.NextDouble() + 65)));
                builder.Append(symbol);
            }

            return builder.ToString();
        }

        public Type GetGenerationType()
        {
            return typeof(string);
        }
    }
}