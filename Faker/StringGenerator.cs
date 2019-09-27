using System;
using System.Text;

namespace Faker
{
    public class StringGenerator : IGenerator
    {
        public object Generate()
        {
            var builder = new StringBuilder();
            var random = new Random();
            var size = random.Next(1,
                100);
            for (var i = 0;
                i < size;
                i++)
            {
                var symbol = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
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