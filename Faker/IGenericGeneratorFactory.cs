using System;

namespace Faker
{
    public interface IGenericGeneratorFactory
    {
        IGenericGenerator GetGenerator(Type[] genericType);
    }
}