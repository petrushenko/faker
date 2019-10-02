using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faker.Tests
{
    class FooIntGenerator : IGenerator
    {
        public object Generate()
        {
            return 123;
        }

        public Type GetGenerationType()
        {
            return typeof(int);
        }
    }
}
