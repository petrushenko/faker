using System;

namespace Faker
{
    public interface IGenericGenerator
    {
        object Generate();

        Type GetGenerationType();
    }
}