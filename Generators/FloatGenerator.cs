using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Faker;

namespace Generators
{
    public class FloatGenerator : IGenerator
    {
        private static readonly Random Random = new Random();
        public object Generate()
        {
            double mantissa = (Random.NextDouble() * 2.0) - 1.0;
            // choose -149 instead of -126 to also generate subnormal floats (*)
            double exponent = Math.Pow(2.0, Random.Next(-126, 128));
            return (float)(mantissa * exponent);
        }

        public Type GetGenerationType()
        {
            return typeof(float);
        }
    }
}
