using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faker.Tests
{
    public class MyClass
    {
        private int _privateField;
        private int _privateProperty { get; }

        public int PublicField;
        public int PublicProperty { get; }
        public MyClass InnerSameClass { get; set; }
        public MyOtherClass InnerClass { get; set; }
        public MyClass(int privateProperty, int privateField, int publicField, int publicProperty)
        {
            _privateProperty = privateProperty;
            _privateField = privateField;
            PublicField = publicField;
            PublicProperty = publicProperty;
        }
    }
}
