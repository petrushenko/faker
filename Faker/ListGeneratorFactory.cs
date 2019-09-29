using System;

namespace Faker
{
    public class ListGeneratorFactory : IGenericGeneratorFactory
    {
        public IGenerator GetGenerator(Type[] genericType)
        {
            var listGenericGeneratorType = typeof(ListGenerator<>);
            var listGeneratorType = listGenericGeneratorType.MakeGenericType(genericType);
            return (IGenerator) Activator.CreateInstance(listGeneratorType);
        }
    }
}