using System;
using System.Text;

namespace Generators
{
    public class StringGenerator : IGenerator
    {
        public object Generate()
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            int size = random.Next(1,
                100);
            for (int i = 0;
                i < size;
                i++)
            {
                char symbol = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
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