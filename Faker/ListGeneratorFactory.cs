using System;
using Faker;

namespace Faker
{
    public class ListGeneratorFactory : IGenericGeneratorFactory
    {
        public IGenericGenerator GetGenerator(Type[] genericType)
        {
            var listGenericGeneratorType = typeof(ListGenerator<>);
            var listGeneratorType = listGenericGeneratorType.MakeGenericType(genericType);
            return (IGenericGenerator) Activator.CreateInstance(listGeneratorType);
        }
    }
}