using System;

namespace Faker
{
    public class Int32Generator : IGenerator
    {
        public object Generate()
        {
            var random = new Random();
            return random.Next(1, int.MaxValue);
        }

        public Type GetGenerationType()
        {
            return typeof(int);
        }
    }
}