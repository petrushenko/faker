using System;

namespace Faker
{
    public class Int32Generator : IGenerator
    {
        private readonly Random _random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

        public object Generate()
        {
            return _random.Next(1, int.MaxValue);
        }

        public Type GetGenerationType()
        {
            return typeof(int);
        }
    }
}