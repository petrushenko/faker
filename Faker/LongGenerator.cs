using System;

namespace Faker
{
    public class LongGenerator : IGenerator
    {
        public object Generate()
        {
            var random = new Random();
            var buffer = new byte[8];
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