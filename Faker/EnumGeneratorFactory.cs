using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faker
{
    class EnumGeneratorFactory : IGenericGeneratorFactory
    {
        public IGenerator GetGenerator(Type[] genericType)
        {
            var enumGenericGeneratorType = typeof(EnumGenerator<>);
            var enumGeneratorType = enumGenericGeneratorType.MakeGenericType(genericType);
            return (IGenerator)Activator.CreateInstance(enumGeneratorType);
        }
    }
}
