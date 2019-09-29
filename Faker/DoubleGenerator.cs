using System;

namespace Faker
{
    public class DoubleGenerator : IGenerator
    {
        private readonly Random _random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

        public object Generate()
        {
            double result;
            do
            {
                result = _random.NextDouble();
            } while (Math.Abs(Math.Abs(result - 0)) < 0.0000001);
            return _random.NextDouble();
        }

        public Type GetGenerationType()
        {
            return typeof(double);
        }
    }
}