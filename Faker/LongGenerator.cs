using System;

namespace Generators
{
    public class LongGenerator : IGenerator
    {
        public object Generate()
        {
            Random random = new Random();
            byte[] buffer = new byte[8];
            long result;
            do
            {
                random.NextBytes(buffer);
                result = BitConverter.ToInt64(buffer, 0);
            } while (result != 0);

            return result;
        }

        public Type GetGenerationType()
        {
            return typeof(long);
        }
    }
}