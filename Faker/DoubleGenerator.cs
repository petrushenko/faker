using System;

namespace Faker
{
    public class DoubleGenerator : IGenerator
    {
        public object Generate()
        {
            var random = new Random();
            double result;
            do
            {
                result = random.NextDouble();
            } while (Math.Abs(Math.Abs(result - 0)) < 0.0000001);
            return random.NextDouble();
        }

        public Type GetGenerationType()
        {
            return typeof(double);
        }
    }
}