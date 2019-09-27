using System;

namespace Faker
{
    public class Int32Generator : IGenerator
    {
        public object Generate()
        {
            Random random = new Random();
            return random.Next(1, Int32.MaxValue);
        }

        public Type GetGenerationType()
        {
            return typeof(int);
        }
    }
}