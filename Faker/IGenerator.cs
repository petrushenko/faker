using System;

namespace Generators
{
    public interface IGenerator
    {
        object Generate();

        Type GetGenerationType();
    }
}