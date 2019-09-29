using System;

namespace Faker
{
    class ArrayGeneratorFactory : IGenericGeneratorFactory
    {
        public IGenerator GetGenerator(Type[] genericType)
        {
            var arrayGenericGeneratorType = typeof(ArrayGenerator<>);
            var arrayGeneratorType = arrayGenericGeneratorType.MakeGenericType(genericType);
            return (IGenerator)Activator.CreateInstance(arrayGeneratorType);
        }
    }
}
