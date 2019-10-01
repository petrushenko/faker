using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faker.Tests
{
    public class FooStringGenerator : IGenerator
    {
        public object Generate()
        {
            var str = "TEST";
            return str;
        }

        public Type GetGenerationType()
        {
            return typeof(string);
        }
    }
}
